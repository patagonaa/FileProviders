using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebDav;

namespace FileProviders.WebDav
{
    public class WebDavFileProvider : IFileProvider, IDisposable
    {
        protected readonly WebDavClient _client;
        protected readonly Uri _baseUri;

        public WebDavFileProvider(IOptions<WebDavConfiguration> options)
        {
            var config = options.Value;
            _baseUri = new Uri(config.BaseUri.TrimEnd('/') + '/');
            var clientParams = new WebDavClientParams
            {
                BaseAddress = _baseUri,
                Credentials = new NetworkCredential(config.User, config.Password)
            };
            _client = new WebDavClient(clientParams);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var uri = CheckAndGetAbsoluteUri(subpath);

            var parameters = new PropfindParameters
            {
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("depth", "1")
                }
            };
            var result = _client.Propfind(uri, parameters).Result;
            if (result.StatusCode == 404)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            if (!result.IsSuccessful)
            {
                throw new WebException("WebDav error " + result.StatusCode + " while listing directory");
            }

            var thisResource = result.Resources.Single(x => Uri.UnescapeDataString(uri.LocalPath.TrimEnd('/')) == Uri.UnescapeDataString(x.Uri.TrimEnd('/')));
            if (!thisResource.IsCollection)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            return new WebDavDirectoryContents(_client, result.Resources.Where(x => x != thisResource).ToList());
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var uri = CheckAndGetAbsoluteUri(subpath);

            var parameters = new PropfindParameters
            {
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("depth", "0")
                }
            };

            var result = _client.Propfind(uri, parameters).Result;
            if (result.StatusCode == 404)
            {
                return new NotFoundFileInfo(subpath);
            }

            if (!result.IsSuccessful)
            {
                throw new WebException("WebDav error " + result.StatusCode + " while listing directory");
            }

            return new WebDavFileInfo(_client, result.Resources.Single());
        }

        protected Uri CheckAndGetAbsoluteUri(string relativeUriString)
        {
            var escapedUri = string.Join("/", relativeUriString.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Uri.EscapeDataString(x)));
            var absoluteUri = new Uri(_baseUri, escapedUri);

            if (!_baseUri.IsBaseOf(absoluteUri))
            {
                throw new ArgumentException($"URI '{relativeUriString}' is outside base path. This might indicate a possible path traversal attack attempt!");
            }

            return absoluteUri;
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
