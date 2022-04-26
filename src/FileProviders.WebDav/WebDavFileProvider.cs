﻿using Microsoft.Extensions.FileProviders;
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
        private readonly WebDavClient _client;
        private readonly Uri _baseUri;

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

            var thisResource = result.Resources.Single(x => x.Uri.TrimEnd('/') == uri.AbsolutePath.TrimEnd('/'));
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

        public bool DeleteFile(string subpath)
        {
            var uri = CheckAndGetAbsoluteUri(subpath);

            var result = _client.Delete(uri).Result;
            if (result.StatusCode == 404)
            {
                return false;
            }

            if (!result.IsSuccessful)
            {
                throw new WebException("WebDav error " + result.StatusCode + " while deleting file");
            }

            return true;
        }

        private Uri CheckAndGetAbsoluteUri(string relativeUriString)
        {
            var relativeUri = new Uri(relativeUriString.TrimStart('/'), UriKind.Relative);
            var absoluteUri = new Uri(_baseUri, relativeUri);

            if (!_baseUri.IsBaseOf(absoluteUri))
            {
                throw new ArgumentException($"URI '{relativeUri}' is outside base path. This might indicate a possible path traversal attack attempt!");
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