using System.Collections.Generic;

namespace FileProviders.Async
{
    /// <summary>
    /// Represents a directory's content in the file provider.
    /// </summary>
    public interface IAsyncDirectoryContents : IAsyncEnumerable<IAsyncFileInfo>
    {
        /// <summary>
        /// True if a directory was located at the given path.
        /// </summary>
        bool Exists { get; }
    }
}
