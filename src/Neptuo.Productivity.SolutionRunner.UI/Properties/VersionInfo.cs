using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.UI
{
    public class VersionInfo
    {
        internal const string Version = "1.6.3";
        internal const string Preview = null;//"-beta1";

        public static Version GetVersion()
        {
            return new Version(Version);
        }
    }
}