# Some demo code for .NET Core apps using SteelToe.

Working:
 - SteelToe VCAP config using `IOptions<T>`.

To be done:
 - Service discovery.

## So what does it do?

This thing pretends to be a blog. It loads markdown files from an Azure storage instance. One of the requirements is a connectionstring to an Azure blob storage.

The blob storage service needs a container called `Posts` where the markdown is stored and a container name `Content` where an index is stored for all posts. New stuff is added in `index.json` in the `Content` container. Make sure the slug matches a markdown file name.

## Azure Storage requirements

Two containers:

 - Content
 - Posts

Content contains an `index.json` file that gathers all the markdown files (so that you can create a list without enumerating the storage).

### Index.json

    [
        {
            "title": "Test post",
            "created": "2019-01-01", // yyyy-mm-dd
            "description": "This is a small test post to prove it works.",
            "slug": "test-post",
            "tags": [
                "test post"
            ]
        }
    ]

Add more as needed.

### *.md blogposts

Plain text `.md` files containing Markdown.

## Services

It contains only two services, a back-end service to access the storage and a front-end to display everything.

### blog-content

To run locally (using powershell):

    $env:ASPNETCORE_ENVIRONMENT="development"
    ${env:vcap:services:user-provided:0:name}="content-storage"
    ${env:vcap:services:user-provided:0:credentials:connectionString}="((your storage connectionstring here))"
    dotnet run

To run on Cloud Foundry: Create a user provided service called `content-storage`.

    cf cups -p content-storage '{ "connectionString": "((your storage connectionstring here))" }'

Then push the `blog-content` source to Cloud Foundry using the provided manifest.

    cf push

### blog-ui

(TBD)