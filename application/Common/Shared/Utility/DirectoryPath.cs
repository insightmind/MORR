using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MORR.Shared.Utility
{
    /// <summary>
    ///     Encapsulates a directory path.
    /// </summary>
    public sealed class DirectoryPath
    {
        private readonly string value;

        /// <summary>
        ///     Creates a new <see cref="DirectoryPath" /> with the provided value.
        /// </summary>
        /// <param name="value">The value to create the <see cref="DirectoryPath" /> with.</param>
        public DirectoryPath(string value)
        {
            if (!TryGetDirectoryPath(value, out this.value))
            {
                throw new ArgumentException($"The specified value \"{value}\" is not a valid directory path.");
            }
        }

        /// <summary>
        ///     Verifies the provided string and gets a local path from it.
        /// </summary>
        /// <param name="path">The path to verify.</param>
        /// <param name="result">The local path if the provided <paramref name="path" /> is valid.</param>
        /// <returns><see langword="true" /> if the provided <paramref name="path" /> is valid, <see langword="false" /> otherwise.</returns>
        private bool TryGetDirectoryPath(string path, [NotNullWhen(true)] out string? result)
        {
            result = null;

            if (!Uri.TryCreate(path, UriKind.Absolute, out var parsedUri) || !parsedUri.IsFile ||
                !string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                return false;
            }

            result = parsedUri.LocalPath;
            return true;
        }

        /// <summary>
        ///     A <see cref="string" /> representation of the directory path.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the directory path.</returns>
        public override string ToString()
        {
            return value;
        }
    }
}