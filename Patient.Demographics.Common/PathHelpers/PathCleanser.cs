namespace Patient.Demographics.Common.PathHelpers
{
    public class PathCleanser
    {
        public string CleanseRelativePath(string path)
        {
            return path?.Trim().Trim('/', '\\');
        }
    }
}
