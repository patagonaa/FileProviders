using FileProviders.Async;
using Microsoft.Extensions.FileProviders;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebDav;

namespace FileProviders.WebDav
{
    class WebDavDirectoryContents : IDirectoryContents, IAsyncDirectoryContents
    {
        private readonly WebDavClient _client;
        private readonly IReadOnlyCollection<WebDavResource> _resources;

        public bool Exists => true;

        public WebDavDirectoryContents(WebDavClient client, IReadOnlyCollection<WebDavResource> resources)
        {
            _client = client;
            _resources = resources;
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            foreach (var resource in _resources)
            {
                yield return new WebDavFileInfo(_client, resource);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async IAsyncEnumerator<IAsyncFileInfo> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var resource in _resources)
            {
                yield return await Task.FromResult(new WebDavFileInfo(_client, resource));
            }
        }
    }
}
