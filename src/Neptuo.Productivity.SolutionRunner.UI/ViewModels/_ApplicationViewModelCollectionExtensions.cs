using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public static class _ApplicationViewModelCollectionExtensions
    {
        public static IApplication Find(this IEnumerable<ApplicationViewModel> applications, IEnumerable<Key> keys)
        {
            ApplicationViewModel application = null;
            ApplicationCommandViewModel command = null;
            int index = 0;
            foreach (Key pressed in keys)
            {
                if (index == 0)
                {
                    foreach (ApplicationViewModel model in applications)
                    {
                        if (model.HotKey == pressed)
                        {
                            application = model;
                            break;
                        }
                    }

                    if (application == null)
                        break;
                }
                else if (index == 1)
                {
                    foreach (ApplicationCommandViewModel model in application.Commands)
                    {
                        if (model.HotKey == pressed)
                        {
                            command = model;
                            break;
                        }
                    }

                    if (command == null)
                    {
                        application = null;
                        break;
                    }
                }

                index++;
            }

            return (IApplication)command ?? application;
        }
    }
}
