using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Properties
{
    partial class Settings : ISettings
    {
        public FileSearchMode GetFileSearchMode()
        {
            FileSearchMode result;
            if (Converts.Try(FileSearchMode, out result))
                return result;

            return Services.Searching.FileSearchMode.StartsWith;
        }

        public int GetFileSearchCount()
        {
            int value = FileSearchCount;
            if (value == 0)
                value = 10;

            return value;
        }

        public string[] GetHiddenMainApplications()
        {
            if (String.IsNullOrEmpty(HiddenMainApplications))
                return new string[0];

            return HiddenMainApplications.Split(Path.PathSeparator);
        }

        public Version GetAutoSelectApplicationMinimalVersion()
        {
            if (String.IsNullOrEmpty(AutoSelectApplicationMinimalVersion))
                return null;

            Version result;
            if (Version.TryParse(AutoSelectApplicationMinimalVersion, out result))
                return result;

            return null;
        }

        public void SetAutoSelectApplicationMinimalVersion(Version version)
        {
            if (version == null)
                AutoSelectApplicationMinimalVersion = null;
            else
                AutoSelectApplicationMinimalVersion = version.ToString(2);
        }
    }
}