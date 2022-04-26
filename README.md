# FileProviders.WebDav
[![Nuget](https://img.shields.io/nuget/v/patagona.FileProviders.WebDav)](https://www.nuget.org/packages/patagona.FileProviders.WebDav/)

A IFileProvider implementation using WebDAV

Right now, this implementation is likely incomplete and **might even be insecure** (especially in regards to path traversal attacks), so use it at your own risk and audit the few lines of code before using it yourself.

## Example usage in ASP.NET Core
appSettings.json
```json
{
    "WebDav": {
        "BaseUri": "https://nextcloud.example.com/remote.php/dav/files/demo/demo-data/",
        "User": "demo",
        "Password": "secretsecretsecret"
    }
}
```

Startup.cs:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    [...]
    services.Configure<WebDavConfiguration>(Configuration.GetSection("WebDav"));
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<WebDavConfiguration> webDavConfig)
{
    [...]

    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/data",
        FileProvider = new WebDavFileProvider(webDavConfig)
    });
    
    [...]
}
```

After starting the web app, all files and directories in the WebDAV directory should be accessible at `/data`.