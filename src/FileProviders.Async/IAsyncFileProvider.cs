using Microsoft.Extensions.Primitives;
using System.Threading;
using System.Threading.Tasks;

namespace FileProviders.Async
{
    /// <summary>
    /// A read-only file provider abstraction.
    /// </summary>
    public interface IAsyncFileProvider
    {
        /// <summary>
        /// Locate a file at the given path.
        /// </summary>
        /// <param name="subpath">Relative path that identifies the file.</param>
        /// <returns>The file information. Caller must check Exists property.</returns>
        Task<IAsyncFileInfo> GetFileInfoAsync(string subpath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enumerate a directory at the given path, if any.
        /// </summary>
        /// <param name="subpath">Relative path that identifies the directory.</param>
        /// <returns>Returns the contents of the directory.</returns>
        Task<IAsyncDirectoryContents> GetDirectoryContentsAsync(string subpath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="IChangeToken"/> for the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">Filter string used to determine what files or folders to monitor. Example: **/*.cs, *.*, subFolder/**/*.cshtml.</param>
        /// <returns>An <see cref="IChangeToken"/> that is notified when a file matching <paramref name="filter"/> is added, modified or deleted.</returns>
        IChangeToken Watch(string filter);
    }
}
