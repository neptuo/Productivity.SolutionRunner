using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class PatternMatcherFactory
    {
        public Func<IFileModel, bool> Create(string searchPattern, FileSearchMode mode)
        {
            if (searchPattern == null)
                searchPattern = String.Empty;

            searchPattern = searchPattern.ToLowerInvariant();

            Func<IFileModel, bool> filter = null;
            switch (mode)
            {
                case FileSearchMode.StartsWith:
                    filter = f => IsNameStartedWith(f, searchPattern);
                    break;
                case FileSearchMode.Contains:
                    string[] parts = searchPattern.Split(' ');
                    filter = f => IsPathSearchMatched(f, parts);
                    break;
                default:
                    throw Ensure.Exception.NotSupportedSearchMode(mode);
            }

            return filter;
        }

        private bool IsNameStartedWith(IFileModel file, string searchPattern)
        {
            return file.Name.ToLowerInvariant().StartsWith(searchPattern);
        }

        private bool IsPathSearchMatched(IFileModel file, string[] searchPattern)
        {
            string pathMatch = file.Path.ToLowerInvariant();
            for (int i = 0; i < searchPattern.Length; i++)
            {
                int currentIndex = pathMatch.IndexOf(searchPattern[i]);
                if (currentIndex == -1)
                    return false;

                pathMatch = pathMatch.Substring(currentIndex + searchPattern[i].Length);
            }

            return true;
        }

        public interface IFileModel
        {
            string Name { get; }
            string Path { get; }
        }
    }
}
