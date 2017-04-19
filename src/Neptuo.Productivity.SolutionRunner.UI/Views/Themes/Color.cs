using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Views.Themes
{
    [MarkupExtensionReturnType(typeof(System.Windows.Media.Color))]
    public class Color : MarkupExtension
    {
        private static List<Color> items = new List<Color>();

        public static void UpdateAll()
        {
            foreach (Color item in items)
                item.Update();
        }

        private SolidColorBrush targetObject;
        private DependencyProperty targetProperty;

        public System.Windows.Media.Color Dark { get; set; }
        public System.Windows.Media.Color Light { get; set; }

        public void Update()
        {
            if (targetObject == null || targetProperty == null)
                return;

            targetObject.SetValue(targetProperty, GetValue());
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlTypeResolver xamlTypeResolver = (IXamlTypeResolver)serviceProvider.GetService(typeof(IXamlTypeResolver));
            IProvideValueTarget provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            targetObject = provideValueTarget.TargetObject as SolidColorBrush;
            targetProperty = provideValueTarget.TargetProperty as DependencyProperty;
            items.Add(this);

            return GetValue();
        }

        private object GetValue()
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
