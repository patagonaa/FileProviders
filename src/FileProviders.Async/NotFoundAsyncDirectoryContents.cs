using System.Collections.Generic;
using System.Threading;

namespace FileProviders.Async
{
    /// <summary>
    /// Represents a non-existing directory
    /// </summary>
    public class NotFoundAsyncDirectoryContents : IAsyncDirectoryContents
    {
        /// <summary>
        /// A shared instance of <see cref="NotFoundDirectoryContents"/>
        /// </summary>
        public static NotFoundAsyncDirectoryContents Singleton { get; } = new NotFoundAsyncDirectoryContents();

        /// <summary>
        /// Always false.
        /// </summary>
        public bool Exists => false;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator to an empty collection.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async IAsyncEnumerator<IAsyncFileInfo> GetAsyncEnumerator(CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            yield break;
        }
    }
}
