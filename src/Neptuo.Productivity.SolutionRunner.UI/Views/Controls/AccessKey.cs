using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    /// <summary>
    /// Custom implementation of AccessKeys with support for multiple keys.
    /// </summary>
    public static class AccessKey
    {
        private static DependencyProperty isKeyboardCuesProperty;

        /// <summary>
        /// Gets a property indincating whether access key styles are visible.
        /// </summary>
        public static DependencyProperty IsKeyboardCuesProperty
        {
            get
            {
                if (isKeyboardCuesProperty == null)
                {
                    Type type = typeof(System.Windows.Input.KeyboardNavigation);
                    FieldInfo fieldInfo = type.GetField("ShowKeyboardCuesProperty", BindingFlags.Static | BindingFlags.NonPublic);
                    isKeyboardCuesProperty = (DependencyProperty)fieldInfo.GetValue(null);
                }

                return isKeyboardCuesProperty;
            }
        }

        private static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached(
            "AccessKeyInstance",
            typeof(Instance),
            typeof(AccessKey),
            new PropertyMetadata(null)
        );

        /// <summary>
        /// Binds an access key handling for a <paramref name="window"/>. When <c>alt</c> is released, a <paramref name="handler"/> is raised.
        /// Multi registration for single window is ok.
        /// </summary>
        /// <param name="window">A window to bind access key handling for.</param>
        /// <param name="handler">A handler to be raised.</param>
        public static void AddHandler(Window window, EventHandler<AccessKeyEventArgs> handler)
        {
            Instance instance = (Instance)window.GetValue(InstanceProperty);
            if (instance == null)
            {
                instance = new Instance(window, handler);
                window.SetValue(InstanceProperty, instance);
            }
            else
            {
                instance.AddHandler(handler);
            }
        }

        /// <summary>
        /// Removes a <paramref name="handler"/> from access key handling.
        /// If the <paramref name="handler"/> is a last handler, complete custom handling is removed.
        /// </summary>
        /// <param name="window">A window to remove handling from.</param>
        /// <param name="handler">A handler to remove.</param>
        public static void RemoveHandler(Window window, EventHandler<AccessKeyEventArgs> handler)
        {
            Instance instance = (Instance)window.GetValue(InstanceProperty);
            if (instance != null)
            {
                if (instance.RemoveHandler(handler))
                    window.ClearValue(InstanceProperty);
            }
        }

        private class Instance
        {
            private readonly Window window;
            private EventHandler<AccessKeyEventArgs> handler;

            private List<Key> keys = new List<Key>();

            public Instance(Window window, EventHandler<AccessKeyEventArgs> handler)
            {
                Ensure.NotNull(window, "window");
                Ensure.NotNull(handler, "handler");
                this.window = window;
                this.handler = handler;

                AccessKeyManager.AddAccessKeyPressedHandler(window, OnAccessKeyPressed);

                window.PreviewKeyDown += OnPreviewKeyDown;
                window.PreviewKeyUp += OnPreviewKeyUp;
            }

            private void OnPreviewKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.System && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                {
                    if ((bool)window.GetValue(IsKeyboardCuesProperty) == false)
                        keys.Clear();
                }
            }

            private void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
            {
                string rawKey = e.Key;
                if (Int32.TryParse(rawKey, out int index))
                    rawKey = $"D{index}";

                if (Enum.TryParse(rawKey, out Key pressed) && pressed != Key.None)
                {
                    keys.Add(pressed);
                    e.Handled = true;
                }
            }

            private void OnPreviewKeyUp(object sender, KeyEventArgs e)
            {
                if (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt))
                {
                    window.SetValue(IsKeyboardCuesProperty, false);
                    e.Handled = true;

                    if (keys.Count > 0)
                    {
                        handler(window, new AccessKeyEventArgs(keys));
                        keys.Clear();
                    }

                    return;
                }
            }

            internal void AddHandler(EventHandler<AccessKeyEventArgs> handler)
            {
                Ensure.NotNull(handler, "handler");
                this.handler += handler;
            }

            internal bool RemoveHandler(EventHandler<AccessKeyEventArgs> handler)
            {
                Ensure.NotNull(handler, "handler");
                this.handler -= handler;
                if (handler.GetInvocationList().Length == 0)
                {
                    window.PreviewKeyDown -= OnPreviewKeyDown;
                    window.PreviewKeyUp -= OnPreviewKeyUp;
                    return true;
                }

                return false;
            }
        }
    }
}
