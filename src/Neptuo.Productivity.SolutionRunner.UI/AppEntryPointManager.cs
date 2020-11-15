//using Microsoft.VisualBasic.ApplicationServices;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Neptuo.Productivity.SolutionRunner
//{
//    public class AppEntryPointManager : WindowsFormsApplicationBase
//    {
//        private App app;

//        public AppEntryPointManager()
//        {
//            IsSingleInstance = true;
//        }

//        protected override bool OnStartup(StartupEventArgs e)
//        {
//            // First time app is launched
//            app = new App();
//            app.InitializeComponent();
//            app.Run();
//            return false;
//        }

//        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
//        {
//            // Subsequent launches
//            base.OnStartupNextInstance(eventArgs);
//            app.Activate();
//        }
//    }
//}
