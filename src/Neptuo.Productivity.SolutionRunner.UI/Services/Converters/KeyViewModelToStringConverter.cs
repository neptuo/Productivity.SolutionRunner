using Neptuo.Converters;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Converters
{
    public class KeyViewModelToStringConverter : ConverterBase<KeyViewModel, string>
    {
        public override bool TryConvert(KeyViewModel sourceValue, out string targetValue)
        {
            if(sourceValue == null)
            {
                targetValue = null;
                return true;
            }

            StringBuilder value = new StringBuilder();
            Action<string> append = (part) =>
            {
                if (value.Length > 0)
                    value.Append(" + ");

                value.Append(part);
            };

            if (sourceValue.Modifier.HasFlag(ModifierKeys.Control))
                append("Ctrl");

            if (sourceValue.Modifier.HasFlag(ModifierKeys.Shift))
                append("Shift");

            if (sourceValue.Modifier.HasFlag(ModifierKeys.Windows))
                append("Windows");

            if (sourceValue.Modifier.HasFlag(ModifierKeys.Alt))
                append("Alt");

            bool result = false;
            if (!modifierKeys.Contains(sourceValue.Key))
            {
                append(sourceValue.Key.ToString());
                result = true;
            }

            targetValue = value.ToString();
            return result;
        }

        private static readonly Key[] modifierKeys = new Key[] { Key.System, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin, Key.LeftAlt, Key.RightAlt };
    }
}
