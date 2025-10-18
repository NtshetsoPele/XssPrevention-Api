# Security.Sanitization Solution

## Overview

This solution provides middleware and services for sanitizing HTTP request headers and bodies in ASP.NET Core applications, targeting .NET 8. It helps protect against XSS and other injection attacks by sanitizing user input at the API boundary.

## Projects

- **Security.Sanitization**: Core library containing sanitization logic, middleware, and extension methods.
- **Security.Sanitization.Api**: Example ASP.NET Core Web API demonstrating how to use the sanitization features.

## Key Features

- **Request Header Sanitization**: Cleans potentially unsafe values in HTTP headers.
- **Request Body Sanitization**: Sanitizes JSON request bodies, with configurable deep or top-level sanitization.
- **HTML Input Sanitization**: Uses `HtmlInputSanitizer` to clean string inputs.
- **Easy Integration**: Extension methods for registering and using sanitization middleware.

## Usage

### 1. Register the Sanitizer

In your `Program.cs`:
- `builder.Services.AddRequestSanitizer( options => options.DeepSanitization = true );` // Enables deep sanitization of nested JSON

### 2. Add Middleware to the Pipeline
- `app.UseRequestHeadersAndBodySanitizers();` 

This adds both header and body sanitization to all incoming requests.

### 3. Test Endpoints

The sample API provides endpoints to test sanitization:

- `GET /test-injected-headers`: Returns sanitized request headers.
- `POST /test-injected-top-level-body`: Returns sanitized top-level JSON body.
- `POST /test-injected-nesting-body`: Returns sanitized nested JSON body.

### 4. Customization

- **Deep Sanitization**: Set `DeepSanitization` to `true` for recursive sanitization of all string values in JSON bodies.
- **Sanitizer Implementation**: The default sanitizer is `HtmlInputSanitizer`, but you can implement `IInputSanitizer` for custom logic.

## How It Works

- Middleware intercepts requests, reads headers and/or body, sanitizes string values, and replaces them before further processing.
- Uses extension methods for easy registration and mapping of test endpoints.

## Considerations

- Maximum payload limits should be enforced at the host or proxy level, especially when deep sanitization is enabled. This helps protect server resources and maintains high availability.
- Header count and size limits should also be configured to prevent excessive memory usage or denial-of-service attacks.
- The **Security.Sanitization.Api** shows how a subsequent middleware component can detect possible XSS injections. The consumer can then choose the appropriate response. See `app.AddMaliciousCodeReportingMiddleware();`.
- Toggle the **MALICE_REPORTING** preprocessor directive to enable or disable the XSS attack reporting.
- The **Security.Sanitization.Api.http** file represents a built-in client that can be used test the sample API. Match the port to what's configured in your local **launchSettings.json** file.

## Getting Started

1. Clone the repository.
2. Open the solution in Visual Studio 2022.
3. Build and run the `Security.Sanitization.Api` project.
4. Use tools like Postman to send requests to the test endpoints and observe sanitization in action.

## Requirements

- .NET 8 SDK
- Visual Studio 2022

## Extending

- Implement additional sanitizers by creating classes that implement `IInputSanitizer`.
- Add more middleware or extension methods as needed for your application's security requirements.