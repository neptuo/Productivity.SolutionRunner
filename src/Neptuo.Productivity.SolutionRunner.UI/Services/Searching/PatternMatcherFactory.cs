using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public partial class PatternMatcherFactory
    {
        private readonly ILog log;

        public PatternMatcherFactory(ILogFactory logFactory)
        {
            Ensure.NotNull(logFactory, "logFactory");
            log = logFactory.Scope("PatternMatcher");
        }

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
            bool result = file.NameWithExtension.ToLowerInvariant().StartsWith(searchPattern);
            log.Debug("Path: '{0}'; Pattern: '{1}'; Mode: 'NameStartsWith'; Result: '{2}'.", file.Path, searchPattern, result);
            return result;
        }

        private bool IsPathSearchMatched(IFileModel file, string[] searchPattern)
        {
            bool result = true;
            string pathMatch = file.Path.ToLowerInvariant();
            for (int i = 0; i < searchPattern.Length; i++)
            {
                int currentIndex = pathMatch.IndexOf(searchPattern[i]);
                if (currentIndex == -1)
                {
                    result = false;
                    break;
                }

                pathMatch = pathMatch.Substring(currentIndex + searchPattern[i].Length);
            }

            log.Debug("Path: '{0}'; Pattern: '{1}'; Mode: 'Contains' Result: '{2}'.", file.Path, String.Join(" ", searchPattern), result);
            return result;
        }
    }
}
