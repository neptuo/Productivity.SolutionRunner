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
    public class StringToKeyViewModelConverter : DefaultConverter<string, KeyViewModel>
    {
        public override bool TryConvert(string sourceValue, out KeyViewModel targetValue)
        {
            if (String.IsNullOrEmpty(sourceValue))
            {
                targetValue = null;
                return true;
            }

            ModifierKeys modifier = ModifierKeys.None;

            bool result = true;
            string[] parts = sourceValue.Split('+');
            for (int i = 0; i < parts.Length - 1; i++)
			{
                string part = parts[i].Trim();
                if (part == "Ctrl")
                    modifier |= ModifierKeys.Control;
                else if (part == "Shift")
                    modifier |= ModifierKeys.Shift;
                else if (part == "Windows")
                    modifier |= ModifierKeys.Windows;
                else if (part == "Alt")
                    modifier |= ModifierKeys.Alt;
                else
                    result = false;
			}

            if (result)
            {
                Key key;
                if (Enum.TryParse<Key>(parts[parts.Length - 1], out key))
                {
                    targetValue = new KeyViewModel(key, modifier);
                    return true;
                }
            }

            targetValue = null;
            return false;
        }
    }
}
