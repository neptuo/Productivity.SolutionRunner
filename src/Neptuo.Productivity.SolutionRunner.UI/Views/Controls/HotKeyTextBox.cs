using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public static class HotKeyTextBox
    {
        public static KeyViewModel GetKey(DependencyObject obj)
        {
            return (KeyViewModel)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, KeyViewModel value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached(
            "Key", 
            typeof(KeyViewModel), 
            typeof(HotKeyTextBox), 
            new PropertyMetadata(new KeyViewModel(Key.Tab, ModifierKeys.Shift), OnKeyChanged)
        );

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = (TextBox)d;
            textBox.IsReadOnly = true;
            textBox.IsReadOnlyCaretVisible = true;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            textBox.LostFocus += OnLostFocus;

            KeyViewModel viewModel = GetKey(textBox);
            if (viewModel != null)
                BindKeyValue(textBox, viewModel.Key, viewModel.Modifier);
        }

        private static void SetTextBoxValue(TextBox textBox, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                textBox.Text = String.Empty;
                textBox.IsReadOnlyCaretVisible = true;
            }
            else
            {
                textBox.Text = value;
                textBox.IsReadOnlyCaretVisible = false;
            }

        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (GetKey(textBox) == null)
                SetTextBoxValue(textBox, String.Empty);
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Key key = e.Key;
            ModifierKeys modifier = e.KeyboardDevice.Modifiers;

            if (key == Key.Back || key == Key.Delete)
            {
                SetTextBoxValue(textBox, String.Empty);
                SetKey(textBox, null);
                e.Handled = true;
                return;
            }
            else if (key == Key.Tab)
            {
                return;
            }
            else if (key == Key.System)
            {
                key = e.SystemKey;
            }

            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
                modifier |= ModifierKeys.Windows;

            if (BindKeyValue(textBox, key, modifier))
                SetKey(textBox, new KeyViewModel(key, modifier));
            else
                SetKey(textBox, null);

            e.Handled = true;
        }

        private static bool BindKeyValue(TextBox textBox, Key key, ModifierKeys modifier)
        {
            string value;
            if (Converts.Try(new KeyViewModel(key, modifier), out value))
            {
                SetTextBoxValue(textBox, value.ToString());
                return true;
            }

            SetTextBoxValue(textBox, String.Empty);
            return false;
        }
    }
}
