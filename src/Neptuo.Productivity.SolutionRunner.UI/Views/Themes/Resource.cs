using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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
            switch (Type)
            {
                case ResourceType.BackgroundBrush:
                    break;
                case ResourceType.ForegroundBrush:
                    break;
                case ResourceType.ActiveColor:
                    break;
                case ResourceType.InactiveColor:
                    return System.Windows.Media.Color.FromRgb(63, 63, 70);
                case ResourceType.ActiveBrush:
                    break;
                case ResourceType.InactiveBrush:
                    break;
                case ResourceType.HoverBrush:
                    break;
                case ResourceType.TextBoxInactiveBrush:
                    break;
                case ResourceType.GrayBrush:
                    break;
                case ResourceType.LinkForegroundBrush:
                    break;
            }

            return null;
        }
    }
}
