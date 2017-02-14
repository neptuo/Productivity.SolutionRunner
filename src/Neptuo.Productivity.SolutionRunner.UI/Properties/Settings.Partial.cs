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
    partial class Settings
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
    }
}