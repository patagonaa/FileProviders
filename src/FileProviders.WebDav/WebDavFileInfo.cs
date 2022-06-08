using FileProviders.Async;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebDav;

namespace FileProviders.WebDav
{
    class WebDavFileInfo : IFileInfo, IAsyncFileInfo
    {
        private readonly WebDavClient _client;
        private readonly WebDavResource _resource;

        public WebDavFileInfo(WebDavClient client, WebDavResource resource)
        {
            _client = client;
            _resource = resource;
        }

        public bool Exists => true;

        public long Length => _resource.ContentLength ?? -1;

        public string PhysicalPath => null;

        public string Name => Uri.UnescapeDataString(Path.GetFileName(_resource.Uri.TrimEnd('/')));

        public DateTimeOffset LastModified => _resource.LastModifiedDate ?? DateTimeOffset.MinValue;

        public bool IsDirectory => _resource.IsCollection;

        public Stream CreateReadStream()
        {
            var result = Task.Run(() => _client.GetProcessedFile(_resource.Uri)).Result;
            if (!result.IsSuccessful)
            {
                throw new WebException("WebDav error " + result.StatusCode + " while getting file");
            }
            return result.Stream;
        }

        public async Task<Stream> CreateReadStreamAsync(CancellationToken cancellationToken = default)
        {
            var result = await _client.GetProcessedFile(_resource.Uri, new GetFileParameters { CancellationToken = cancellationToken });
            if (!result.IsSuccessful)
            {
                throw new WebException("WebDav error " + result.StatusCode + " while getting file");
            }
            return result.Stream;
        }
    }
}
