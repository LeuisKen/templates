# EMG Templates

This repository contains a set of templates to be used when creating .NET projects from the `dotnet new` command line interface.

You can use [this guide](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new) to understand the options available to you when generating a project using the templates

## Available templates

The current version of the package contains the following templates.

| Name                            | Short name                 |
| ------------------------------- | -------------------------- |
| ASP.NET Core API                | emg-web-api                |
| Batch JobProcessor              | emg-batch-jobprocessor     |
| Hosted Service                  | emg-hosted-service         |
| Lambda Event Function           | emg-lambda-event           |
| Lambda RequestResponse Function | emg-lambda-requestresponse |
| Test Project                    | emg-test-lib               |
| Windows Service                 | emg-windows-service        |
| gRPC Service                    | emg-grpc-service           |
| MassTransit Consumer            | emg-masstransit-consumer   |

### ASP.NET Core API

```
dotnet new emg-web-api
```

This template creates a REST API hosted by a ASP.NET Core application.

The following packages are added by default:

- `EMG.AspNetCore.Authentication.JWT`
- `EMG.Extensions.Logging.Loggly`

You can customize the application by providing the following parameters

- `--configure-aws` adds the basic setup for AWS services
- `--nybus` adds support for sending messages with Nybus v1
- `--nybus-bridge` adds support for sending messages via the NybusBridge to Nybus v0 applications
- `--wcf-discovery` adds support for discovering WCF services

### Batch JobProcessor

```
dotnet new emg-batch-jobprocessor
```

This template creates a console application that can be used to process AWS Batch jobs.

### Hosted Service

```
dotnet new emg-hosted-service
```

This template creates a Hosted Service application. The runtime is selected depending on which workloads are selected.

The application can be customized with the following parameters:

- `--add-nybus Current` adds support for hosting Nybus v1.x handlers
- `--add-nybus Legacy` adds support for hosting Nybus v0.x handlers. Requires .NET Framework 4.8
- `--add-wcf` adds support for hosting WCF services. Requires .NET Framework 4.8
- `--add-aws` adds basic setup to AWS services
- `--force-net48` forces the runtime to be .NET Framework 4.8 even if the application could run on .NET Core 3.1

### Lambda Event Function

```
dotnet new emg-lambda-event
```

This template creates a AWS Lambda function that reacts to event and does not return a response.

### Lambda RequestResponse Function

```
dotnet new emg-lambda-requestresponse
```

This template creates a AWS Lambda function that reacts to event and then returns a response.

### Test Project

```
dotnet new emg-test-lib
```

This template creates a unit-test project using NUnit 3.x.

It also has references to

- `AutoFixture`
- `Moq`
- `AutoFixture.NUnit3`
- `AutoFixture.AutoMoq`
- `AutoFixture.Idioms`

The project will contain a specialization of `AutoDataAttribute` that configures the Moq glue library for AutoFixture.

### Windows Service

```
dotnet new emg-windows-service
```

This template creates a Windows Service application based on TopShelf.

The application can be customized with the following parameters:

- `--add-nybus Current` adds support for hosting Nybus v1.x handlers
- `--add-nybus Legacy` adds support for hosting Nybus v0.x handlers
- `--add-wcf` adds support for hosting WCF services
- `--add-aws` adds basic setup to AWS services

If your application needs to be hosted in a Docker container, please use the `emg-hosted-service` with the same parameters.

### gRPC Service

```
dotnet new emg-grpc-service
```

This template creates a gRPC Server application with loggly support.

### MassTransit Consumer

```
dotnet new emg-masstransit-consumer
```

This template creates a Hosted Service application in .NET8 with preconfigured RabbitMq and a purpose to receive messages.


## Installation

You can install the templates in this package by running the following command:

```
dotnet new -i EMG.Templates
```

Alternatively, you can check out the repository and install the package by running the following command in the folder where you checked out the repository:

```
dotnet new -i .\content\
```

## Update to the latest version

To update the templates to the latest version, simply reinstall the package as specified in the section above.

## Uninstall

To uninstall the templates, you can use the command suggested by the `dotnet new -u` command.

Example:

```
  C:\Development\EMG\Templates\content
    Templates:
      [EMG] Batch JobProcessor (emg-batch-jobprocessor) C#
      [EMG] Lambda Event Function (emg-lambda-event) C#
      [EMG] Hosted Service (emg-hosted-service) C#
      [EMG] Lambda RequestResponse Function (emg-lambda-requestresponse) C#
      [EMG] Test Project (emg-test-lib) C#
      [EMG] ASP.NET Core API (emg-web-api) C#
      [EMG] Windows Service (emg-windows-service) C#
      [EMG] gRPC Service (emg-grpc-service) C#
	  [EMG] MassTransit Consumer (emg-masstransit-consumer) C#
    Uninstall Command:
      dotnet new -u C:\Development\EMG\Templates\content
```
