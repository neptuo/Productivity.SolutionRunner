using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Neptuo.Productivity.SolutionRunner
{
    public abstract class AppSingleInstace
    {
        private static void NotifyOther()
            => Win32.PostMessage((IntPtr)Win32.HwndBroadcast, Win32.WmActivate, IntPtr.Zero, IntPtr.Zero);

        public void Run()
        {
            using (Mutex mutex = new Mutex(true, ApplicationId.ToString(), out var isNew))
            {
                if (isNew)
                {
                    ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
                    OnStartup();
                }
                else
                {
                    NotifyOther();
                }
            }
        }
        
        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == Win32.WmActivate)
                OnStartupNextInstance();
        }

        protected Guid ApplicationId { get; set; }

        protected abstract void OnStartup();
        protected abstract void OnStartupNextInstance();

        internal class Win32
        {
            public static readonly IntPtr HwndBroadcast = (IntPtr)0xffff;
            public static readonly int WmActivate = RegisterWindowMessage("WM_SOLUTIONRUNNER_ACTIVATE");

            [DllImport("user32")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImport("user32")]
            public static extern int RegisterWindowMessage(string message);
        }
    }
}