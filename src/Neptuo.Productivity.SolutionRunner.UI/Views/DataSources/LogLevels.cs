using Neptuo.Logging;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Neptuo.Productivity.SolutionRunner.Views.DataSources
{
    public class LogLevels : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new List<NameValueViewModel>()
            {
                new NameValueViewModel("Debug", LogLevel.Debug),
                new NameValueViewModel("Information", LogLevel.Info),
                new NameValueViewModel("Warning", LogLevel.Warning),
                new NameValueViewModel("Error", LogLevel.Error)
            };
        }
    }
}
