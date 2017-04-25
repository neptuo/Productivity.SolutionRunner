using Neptuo;
using Neptuo.Productivity.SolutionRunner.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public class SettingsExtension : MarkupExtension
    {
        public string Key { get; private set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public SettingsExtension(string key)
        {
            Ensure.NotNullOrEmpty(key, "key");
            EnsureKey(key);
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            DependencyProperty property = provideValueTarget.TargetProperty as DependencyProperty;

            object value = Settings.Default[Key];
            if (Converter != null)
                value = Converter.Convert(value, property?.PropertyType ?? typeof(object), ConverterParameter, Thread.CurrentThread.CurrentUICulture);
            
            return value;
        }

        [Conditional("DEBUG")]
        private static void EnsureKey(string key)
        {
            Debug.Assert(
                typeof(Settings).GetProperty(key) != null, 
                $"Missing settings property '{key}'."
            );
        }
    }
}
