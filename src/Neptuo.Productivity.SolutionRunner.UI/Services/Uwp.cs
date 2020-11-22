using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public static class Uwp
    {
        private const long NoPackageError = 15700;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        private static bool? isContainerized;

        public static bool Is()
        {
            Ensure();
            return isContainerized.Value;
        }

        private static void Ensure()
        {
            if (isContainerized == null)
            {
                isContainerized = false;

                if (Environment.OSVersion.Version.Major >= 10)
                {
                    int length = 0;
                    int result = GetCurrentPackageFullName(ref length, new StringBuilder(0));
                    isContainerized = result != NoPackageError;
                }
            }
        }
    }
}
