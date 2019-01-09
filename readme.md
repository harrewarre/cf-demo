# Some demo code for .NET Core apps using SteelToe.

Working:
 - SteelToe VCAP config using `IOptions<T>`.

To be done:
 - Service discovery.

## So what does it do?

This thing pretends to be a blog. It loads markdown files from an Azure storage instance. One of the requirements is a connectionstring to an Azure blob storage.

The blob storage service needs a container called `Posts` where the markdown is stored. Add new stuff in `index.json` in the `Content` container. Make sure the slug matches a markdown file name.

### blog-content

To run locally (using powershell):

    $env:ASPNETCORE_ENVIRONMENT="development"
    ${env:vcap:services:user-provided:0:name}="content-storage"
    ${env:vcap:services:user-provided:0:credentials:connectionString}="((your storage connectionstring here))"
    dotnet run

To run on Cloud Foundry: Create a user provided service called `content-storage`.

    cf cups -p content-storage '{ "connectionString": "((your storage connectionstring here))" }'

### blog-ui

(TBD)