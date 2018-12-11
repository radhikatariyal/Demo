using System.Collections.Generic;
using System.IO;
using Patient.Demographics.Common.Validation;

namespace Patient.Demographics.Common.PathHelpers
{
    public class PathBuilder
    {
        private readonly PathCleanser _pathCleanser = new PathCleanser();

        public string Build(string rootPath, params string[] subsequentPaths)
        {
            ArgumentValidator.EnsureIsNotNullOrWhitespace(rootPath, nameof(rootPath));
            ArgumentValidator.EnsureIsNotNull(subsequentPaths, nameof(subsequentPaths));

            var pathSegments = new List<string> { rootPath };

            foreach (var subsequentPath in subsequentPaths)
            {
                var cleansedSubsequentPath = _pathCleanser.CleanseRelativePath(subsequentPath);
                if (!string.IsNullOrWhiteSpace(cleansedSubsequentPath))
                {
                    pathSegments.Add(cleansedSubsequentPath);
                }
            }

            return Path.Combine(pathSegments.ToArray());
        }
    }
}
