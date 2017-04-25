using Neptuo;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Views.Themes
{
    public class Resource : MarkupExtension
    {
        [ConstructorArgument("Type")]
        public ResourceType Type { get; set; }

        public Resource(ResourceType type)
        {
            Type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(Type);
        }

        public static object ProvideValue(ResourceType type)
        {
            switch (Settings.Default.ThemeMode)
            {
                case ThemeMode.Dark:
                    return ProvideDarkValue(type);
                case ThemeMode.Light:
                    return ProvideLightValue(type);
                default:
                    return null;
            }
        }

        private static object ProvideDarkValue(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.BackgroundBrush:
                    return GetBrushFromString("#1E1E1E");
                case ResourceType.ForegroundBrush:
                    return GetBrushFromString("#FFFFFF");
                case ResourceType.ActiveColor:
                    return GetColorFromString("#3683D3");
                case ResourceType.ActiveBrush:
                    return GetBrushFromString("#3683D3");
                case ResourceType.InactiveColor:
                    return GetColorFromString("#3F3F46");
                case ResourceType.InactiveBrush:
                    return GetBrushFromString("#3F3F46");
                case ResourceType.HoverBrush:
                    return GetBrushFromString("#663683D3");
                case ResourceType.TextBoxInactiveBrush:
                    return GetBrushFromString("#ABADB3");
                case ResourceType.TextBoxBackgroundBrush:
                    return GetBrushFromString("#3F3F46");
                case ResourceType.GrayBrush:
                    return GetBrushFromString("#777777");
                case ResourceType.LinkForegroundBrush:
                    return GetBrushFromString("#4A9AD4");
                default:
                    throw Ensure.Exception.NotSupported(type);
            }
        }

        private static object ProvideLightValue(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.BackgroundBrush:
                    return GetBrushFromString("#F5F5F5");
                case ResourceType.ForegroundBrush:
                    return GetBrushFromString("#1E1E1E");
                case ResourceType.ActiveColor:
                    return GetColorFromString("#569DE5");
                case ResourceType.ActiveBrush:
                    return GetBrushFromString("#569DE5");
                case ResourceType.InactiveColor:
                    return GetColorFromString("#FFFFFF");
                case ResourceType.InactiveBrush:
                    return GetBrushFromString("#FFFFFF");
                case ResourceType.HoverBrush:
                    return GetBrushFromString("#663683D3");
                case ResourceType.TextBoxInactiveBrush:
                    return GetBrushFromString("#ABADB3");
                case ResourceType.TextBoxBackgroundBrush:
                    return GetBrushFromString("#FFFFFF");
                case ResourceType.GrayBrush:
                    return GetBrushFromString("#777777");
                case ResourceType.LinkForegroundBrush:
                    return GetBrushFromString("#4A9AD4");
                default:
                    throw Ensure.Exception.NotSupported(type);
            }
        }

        private static Color GetColorFromString(string value)
        {
            return (Color)ColorConverter.ConvertFromString(value);
        }

        private static SolidColorBrush GetBrushFromString(string value)
        {
            return new SolidColorBrush(GetColorFromString(value));
        }
    }
}
