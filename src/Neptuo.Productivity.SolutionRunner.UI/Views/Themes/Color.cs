using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Neptuo.Productivity.SolutionRunner.Views.Themes
{
    public class Color : MarkupExtension
    {
        public System.Windows.Media.Color Dark { get; set; }
        public System.Windows.Media.Color Light { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            switch (Settings.Default.ThemeMode)
            {
                case ThemeMode.Dark:
                    return Dark;
                case ThemeMode.Light:
                    return Light;
                default:
                    return null;
            }
        }
    }
}
