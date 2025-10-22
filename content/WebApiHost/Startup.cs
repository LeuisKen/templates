//#if (AddNybusBridge)
using Amazon.SimpleNotificationService;
using EMG.Common;
//#endif
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using EMG.AspNetCore.Authentication.JWT.Options;
using EMG.AspNetCore;
using System.Security.Claims;
using System.Threading.Tasks;
//#if (AddDiscoveryAdapter)
using EMG.Extensions.DependencyInjection.Discovery;
using System.ServiceModel;
//#endif
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//#if (AddNybus)
using Nybus;
//#endif

namespace WebApiHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            JwtEnabled = bool.Parse(Configuration["JWT:Enabled"]);
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment HostingEnvironment { get; }

        private bool JwtEnabled { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var mvc = services.AddMvc(config =>
            {
                config.EnableEndpointRouting = false;
                if (!JwtEnabled) return;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            if (JwtEnabled)
            {
                ConfigureJwt(mvc);
            }

            // Adds support for ASP.NET Core health checks: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2
            services.AddHealthChecks();
            //#if (AddNybus)

            // Configures Nybus to use RabbitMQ engine and fetch settings from the configuration values
            services.AddNybus(nybus =>
            {
                nybus.UseConfiguration(Configuration);

                nybus.UseRabbitMqBusEngine(rabbitMq =>
                {
                    rabbitMq.UseConfiguration();
                    rabbitMq.Configure(cfg => cfg.UnackedMessageCountLimit = 10);
                });
            });

            // Configures the NybusHostedService so that Nybus is started when the web application is started
            services.AddHostedService<NybusHostedService>();
            //#endif

            //#if (AddDiscoveryAdapter)
            services.
                ConfigureServiceDiscovery(Configuration.GetSection("Discovery")).
                ConfigureServiceDiscovery(o =>
                {
                    o.ConfigureDiscoveryAdapterBinding = binding =>
                    {
                        binding.Security.Mode = SecurityMode.None;
                    };
                });
            services.AddServiceDiscoveryAdapter();
            services.AddBindingCustomization(binding => binding.Security.Mode = SecurityMode.None);
            // To register a discoverable WCF service, uncomment the line below and replace 'IYourServiceContract' with the appropriate service interface:
            // services.DiscoverServiceUsingAdapter<IYourServiceContract>();
            //#endif
            //#if (AddNybusBridge || ConfigureAWS)

            // Configures AWS using the configuration values
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            //#endif
            //#if (AddNybusBridge)

            // Registers the SNS client
            services.AddAWSService<IAmazonSimpleNotificationService>();

            services.AddSingleton<INybusBridge, SnsNybusBridge>();
            //#endif
        }

        private void ConfigureJwt(IMvcBuilder mvc)
        {
           var options = new JwtOptions
            {
                AuthenticateUser = user =>
                {
                    ClaimsIdentity identity = null;

                    if (user.UserName == Configuration["JWT:Client:User"] && user.Password == Configuration["JWT:Client:Password"])
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, user.UserName) };
                        identity = new ClaimsIdentity(claims);
                    }

                    return Task.FromResult(identity);
                }
            };

            Configuration.GetSection("JWT").Bind(options);

            mvc.AddJwtAuthentication(options);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Requests matching "/health" are forwarded to the health check engine
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                AllowCachingResponses = false
            });

            // Uses JWT authentication flow
            if (JwtEnabled)
                app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
