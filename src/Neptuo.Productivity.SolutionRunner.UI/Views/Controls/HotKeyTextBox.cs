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
            new FrameworkPropertyMetadata(
                new KeyViewModel(Key.Tab, ModifierKeys.Shift), 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                OnKeyChanged
            )
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


        public static ModifierKeys GetAllowedModifiers(DependencyObject obj)
        {
            return (ModifierKeys)obj.GetValue(AllowedModifiersProperty);
        }

        public static void SetAllowedModifiers(DependencyObject obj, ModifierKeys value)
        {
            obj.SetValue(AllowedModifiersProperty, value);
        }

        public static readonly DependencyProperty AllowedModifiersProperty = DependencyProperty.RegisterAttached(
            "AllowedModifiers", 
            typeof(ModifierKeys), 
            typeof(HotKeyTextBox), 
            new PropertyMetadata(ModifierKeys.Windows | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)
        );


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
            else if (key == Key.Tab || key == Key.Return)
            {
                return;
            }
            else if (key == Key.System)
            {
                key = e.SystemKey;
            }

            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
                modifier |= ModifierKeys.Windows;

            modifier = FilterAllowedModifiers(textBox, modifier);

            if (BindKeyValue(textBox, key, modifier))
                SetKey(textBox, new KeyViewModel(key, modifier));
            else
                SetKey(textBox, null);

            e.Handled = true;
        }

        private static ModifierKeys FilterAllowedModifiers(TextBox textBox, ModifierKeys modifier)
        {
            ModifierKeys allowed = GetAllowedModifiers(textBox);

            if (modifier.HasFlag(ModifierKeys.Windows) && !allowed.HasFlag(ModifierKeys.Windows))
                modifier &= ~ModifierKeys.Windows;

            if (modifier.HasFlag(ModifierKeys.Control) && !allowed.HasFlag(ModifierKeys.Control))
                modifier &= ~ModifierKeys.Control;

            if (modifier.HasFlag(ModifierKeys.Shift) && !allowed.HasFlag(ModifierKeys.Shift))
                modifier &= ~ModifierKeys.Shift;

            if (modifier.HasFlag(ModifierKeys.Alt) && !allowed.HasFlag(ModifierKeys.Alt))
                modifier &= ~ModifierKeys.Alt;

            return modifier;
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
