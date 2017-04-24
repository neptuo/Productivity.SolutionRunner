using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public static class KeyboardNavigation
    {
        private static DependencyProperty showKeyboardCuesProperty;

        public static DependencyProperty ShowKeyboardCuesProperty
        {
            get
            {
                if (showKeyboardCuesProperty == null)
                {
                    Type type = typeof(System.Windows.Input.KeyboardNavigation);
                    FieldInfo fieldInfo = type.GetField("ShowKeyboardCuesProperty", BindingFlags.Static | BindingFlags.NonPublic);
                    showKeyboardCuesProperty = (DependencyProperty)fieldInfo.GetValue(null);
                }

                return showKeyboardCuesProperty;
            }
        }
    }
}
