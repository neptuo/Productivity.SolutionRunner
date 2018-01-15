using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Properties
{
    partial class Settings : ISettings
    {
        AdditionalApplicationCollection ISettings.AdditionalApplications
        {
            get
            {
                string rawValue = AdditionalApplications;
                if (String.IsNullOrEmpty(rawValue))
                    return new AdditionalApplicationCollection();

                return Converts
                    .To<string, AdditionalApplicationCollection>(AdditionalApplications);
            }
            set
            {
                if (value == null)
                    AdditionalApplications = null;
                else
                    AdditionalApplications = Converts.To<AdditionalApplicationCollection, string>(value);
            }
        }

        FileSearchMode ISettings.FileSearchMode
        {
            get
            {
                if (Converts.Try(FileSearchMode, out FileSearchMode result))
                    return result;

                return Services.Searching.FileSearchMode.StartsWith;
            }
            set
            {
                FileSearchMode = value.ToString();
            }
        }

        IReadOnlyList<string> ISettings.HiddenMainApplications
        {
            get
            {
                if (String.IsNullOrEmpty(HiddenMainApplications))
                    return new string[0];

                return HiddenMainApplications.Split(Path.PathSeparator);
            }
            set
            {
                if (value == null)
                    HiddenMainApplications = null;
                else
                    HiddenMainApplications = String.Join(Path.PathSeparator.ToString(), value);
            }
        }

        Version ISettings.AutoSelectApplicationMinimalVersion
        {
            get
            {
                if (String.IsNullOrEmpty(AutoSelectApplicationMinimalVersion))
                    return null;

                Version result;
                if (Version.TryParse(AutoSelectApplicationMinimalVersion, out result))
                    return result;

                return null;
            }
            set
            {
                if (value == null)
                    AutoSelectApplicationMinimalVersion = null;
                else
                    AutoSelectApplicationMinimalVersion = value.ToString(2);
            }
        }
    }
}