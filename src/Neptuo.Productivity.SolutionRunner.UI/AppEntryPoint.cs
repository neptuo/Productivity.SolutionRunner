using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner
{
    public class AppEntryPoint : AppSingleInstace
    {
        [STAThread]
        public static void Main(string[] args) => new AppEntryPoint().Run();

        public AppEntryPoint()
        {
#if DEBUG
            ApplicationId = new Guid("54d7d4aa-db22-4582-86c6-a70badbed65a");
#else
            ApplicationId = new Guid("8a8ec8b3-8da7-400a-864b-c992c3f66f31");
#endif
        }

        private App app;

        protected override void OnStartup()
        {
            app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartupNextInstance()
        {
            app.Activate();
        }
    }
}
