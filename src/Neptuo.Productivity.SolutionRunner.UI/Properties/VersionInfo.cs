﻿using System;
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
        internal const string Version = "1.11.0";
        internal const string Preview = null;

        public static Version GetVersion()
        {
            return new Version(Version);
        }
    }
}