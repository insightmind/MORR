using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MORR.Shared.Utility
{
    /// <summary>
    ///     Encapsulates a local file path.
    /// </summary>
    public sealed class FilePath
    {
        private readonly string value;

        /// <summary>
        ///     Creates a new <see cref="FilePath" /> with the provided value.
        /// </summary>
        /// <param name="value">The value to create the <see cref="FilePath" /> with.</param>
        /// <param name="skipValidation">
        ///     Whether to skip path validation. <see langword="true" /> to skip validation,
        ///     <see langword="false" /> otherwise.
        /// </param>
        public FilePath(string value, bool skipValidation = false)
        {
            if (skipValidation)
            {
                this.value = value;
            }
            else if (!TryGetLocalFilePath(value, out this.value))
            {
                throw new ArgumentException($"The specified value \"{value}\" is not a valid file path.");
            }
        }

        /// <summary>
        ///     Verifies the provided string and gets a local file path from it.
        /// </summary>
        /// <param name="path">The path to verify.</param>
        /// <param name="result">The local path if the provided <paramref name="path" /> is valid.</param>
        /// <returns><see langword="true" /> if the provided <paramref name="path" /> is valid, <see langword="false" /> otherwise.</returns>
        private bool TryGetLocalFilePath(string path, [NotNullWhen(true)] out string? result)
        {
            result = null;

            if (!Uri.TryCreate(path, UriKind.Absolute, out var parsedUri) || !parsedUri.IsFile ||
                string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                return false;
            }

            result = parsedUri.LocalPath;
            return true;
        }

        /// <summary>
        ///     A <see cref="string" /> representation of the file path.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the file path.</returns>
        public override string ToString()
        {
            return value;
        }

        public override bool Equals(object? obj) => obj is FilePath path && value == path.value;

        public override int GetHashCode() => HashCode.Combine(value);
    }
}