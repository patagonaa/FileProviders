using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileProviders.Async
{
    /// <summary>
    /// Represents a non-existing file.
    /// </summary>
    public class NotFoundAsyncFileInfo : IAsyncFileInfo
    {
        /// <summary>
        /// Initializes an instance of <see cref="NotFoundFileInfo"/>.
        /// </summary>
        /// <param name="name">The name of the file that could not be found</param>
        public NotFoundAsyncFileInfo(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool Exists => false;

        /// <summary>
        /// Always false.
        /// </summary>
        public bool IsDirectory => false;

        /// <summary>
        /// Returns <see cref="DateTimeOffset.MinValue"/>.
        /// </summary>
        public DateTimeOffset LastModified => DateTimeOffset.MinValue;

        /// <summary>
        /// Always equals -1.
        /// </summary>
        public long Length => -1;

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Always null.
        /// </summary>
        public string? PhysicalPath => null;

        /// <summary>
        /// Always throws. A stream cannot be created for non-existing file.
        /// </summary>
        /// <exception cref="FileNotFoundException">Always thrown.</exception>
        /// <returns>Does not return</returns>
        public Task<Stream> CreateReadStreamAsync(CancellationToken cancellationToken = default)
        {
            throw new FileNotFoundException($"File {Name} does not exist!");
        }
    }
}
