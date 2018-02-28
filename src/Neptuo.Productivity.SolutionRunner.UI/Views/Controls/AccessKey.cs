using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// Gets a property indicating whether access key styles are visible.
        /// </summary>
        public static DependencyProperty IsKeyboardCuesProperty
        {
            get
            {
                if (isKeyboardCuesProperty == null)
                {
                    Type type = typeof(KeyboardNavigation);
                    FieldInfo fieldInfo = type.GetField("ShowKeyboardCuesProperty", BindingFlags.Static | BindingFlags.NonPublic);
                    isKeyboardCuesProperty = (DependencyProperty)fieldInfo?.GetValue(null);

                    if (isKeyboardCuesProperty == null)
                        throw Ensure.Exception.InvalidOperation("Missing framework property 'ShowKeyboardCuesProperty'.");
                }

                return isKeyboardCuesProperty;
            }
        }

        public static void SetIsKeyboardCues(DependencyObject targetObject, bool value)
        {
            Ensure.NotNull(targetObject, "targetObject");
            targetObject.SetValue(IsKeyboardCuesProperty, value);
        }

        public static bool GetIsKeyboardCues(DependencyObject targetObject)
        {
            Ensure.NotNull(targetObject, "targetObject");
            return (bool)targetObject.GetValue(IsKeyboardCuesProperty);
        }

        private static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached(
            "AccessKeyInstance",
            typeof(Instance),
            typeof(AccessKey),
            new PropertyMetadata(null)
        );

        private static Instance GetInstance(Window window)
        {
            Ensure.NotNull(window, "window");

            Instance instance = (Instance)window.GetValue(InstanceProperty);
            if (instance == null)
            {
                instance = new Instance(window);
                window.SetValue(InstanceProperty, instance);
            }

            return instance;
        }

        /// <summary>
        /// Binds an access key handling for a <paramref name="window"/>. When <c>alt</c> is released, a <paramref name="handler"/> is raised.
        /// Multi registration for single window is ok.
        /// </summary>
        /// <param name="window">A window to bind access key handling for.</param>
        /// <param name="handler">A handler to be raised.</param>
        public static void AddPressedHandler(Window window, EventHandler<AccessKeyPressedEventArgs> handler)
        {
            Ensure.NotNull(handler, "handler");

            Instance instance = GetInstance(window);
            instance.AddPressedHandler(handler);
        }

        /// <summary>
        /// Removes a <paramref name="handler"/> from access key handling.
        /// If the <paramref name="handler"/> is a last handler, complete custom handling is removed.
        /// </summary>
        /// <param name="window">A window to remove handling from.</param>
        /// <param name="handler">A handler to remove.</param>
        public static void RemovePressedHandler(Window window, EventHandler<AccessKeyPressedEventArgs> handler)
        {
            Ensure.NotNull(window, "window");
            Ensure.NotNull(handler, "handler");

            Instance instance = (Instance)window.GetValue(InstanceProperty);
            if (instance != null)
            {
                if (instance.RemovePressedHandler(handler))
                    window.ClearValue(InstanceProperty);
            }
        }

        /// <summary>
        /// Binds an access key handling for a <paramref name="window"/>. Each time a key in combination with <c>alt</c> is released, a <paramref name="handler"/> is raised.
        /// Multi registration for single window is ok.
        /// </summary>
        /// <param name="window">A window to bind access key handling for.</param>
        /// <param name="handler">A handler to be raised.</param>
        public static void AddPressingHandler(Window window, EventHandler<AccessKeyPressingEventArgs> handler)
        {
            Ensure.NotNull(handler, "handler");

            Instance instance = GetInstance(window);
            instance.AddPressingHandler(handler);
        }

        /// <summary>
        /// Removes a <paramref name="handler"/> from access key handling.
        /// If the <paramref name="handler"/> is a last handler, complete custom handling is removed.
        /// </summary>
        /// <param name="window">A window to remove handling from.</param>
        /// <param name="handler">A handler to remove.</param>
        public static void RemovePressingHandler(Window window, EventHandler<AccessKeyPressingEventArgs> handler)
        {
            Ensure.NotNull(window, "window");
            Ensure.NotNull(handler, "handler");

            Instance instance = (Instance)window.GetValue(InstanceProperty);
            if (instance != null)
            {
                if (instance.RemovePressingHandler(handler))
                    window.ClearValue(InstanceProperty);
            }
        }

        private class Instance
        {
            private readonly Window window;
            private EventHandler<AccessKeyPressedEventArgs> pressedHandler;
            private EventHandler<AccessKeyPressingEventArgs> pressingHandler;

            private List<Key> keys = new List<Key>();

            public Instance(Window window)
            {
                Ensure.NotNull(window, "window");
                this.window = window;

                AccessKeyManager.AddAccessKeyPressedHandler(window, OnAccessKeyPressed);

                window.PreviewKeyDown += OnPreviewKeyDown;
                window.PreviewKeyUp += OnPreviewKeyUp;
                window.LostFocus += OnLostFocus;
                window.Activated += OnActivated;
                window.Deactivated += OnDeactivated;

                if (!IsAccessKeyDown())
                    SetIsKeyboardCues(window, false);
            }

            private void OnActivated(object sender, EventArgs e)
            {
                if (!IsAccessKeyDown())
                    SetIsKeyboardCues(window, false);
            }

            private void OnDeactivated(object sender, EventArgs e)
            {
                SetIsKeyboardCues(window, false);
                keys.Clear();
            }

            private void OnLostFocus(object sender, RoutedEventArgs e)
            {
                SetIsKeyboardCues(window, false);
                keys.Clear();
            }

            private void OnPreviewKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.System && IsAccessKeyDown())
                {
                    if (!GetIsKeyboardCues(window))
                        keys.Clear();
                }
            }

            private void OnAccessKeyPressed(object sender, System.Windows.Input.AccessKeyPressedEventArgs e)
            {
                string rawKey = e.Key;
                if (Int32.TryParse(rawKey, out int index))
                    rawKey = $"D{index}";

                if (Enum.TryParse(rawKey, out Key pressed) && pressed != Key.None)
                {
                    AccessKeyPressingEventArgs args = new AccessKeyPressingEventArgs(keys, pressed);
                    if (pressingHandler != null)
                        pressingHandler(window, args);

                    if (!args.IsCancelled)
                        keys.Add(pressed);

                    e.Handled = true;
                }
            }

            private void OnPreviewKeyUp(object sender, KeyEventArgs e)
            {
                if (!IsAccessKeyDown())
                {
                    SetIsKeyboardCues(window, false);
                    e.Handled = true;

                    if (keys.Count > 0)
                    {
                        if (pressedHandler != null)
                            pressedHandler(window, new AccessKeyPressedEventArgs(keys));

                        keys.Clear();
                    }

                    return;
                }
            }

            internal void AddPressedHandler(EventHandler<AccessKeyPressedEventArgs> handler)
            {
                Ensure.NotNull(handler, "handler");
                pressedHandler += handler;
            }

            internal bool RemovePressedHandler(EventHandler<AccessKeyPressedEventArgs> handler)
            {
                Ensure.NotNull(handler, "handler");
                pressedHandler -= handler;
                return TryDetachEvents();
            }

            internal void AddPressingHandler(EventHandler<AccessKeyPressingEventArgs> handler)
            {
                Ensure.NotNull(handler, "handler");
                pressingHandler += handler;
            }

            internal bool RemovePressingHandler(EventHandler<AccessKeyPressingEventArgs> handler)
            {
                Ensure.NotNull(handler, "handler");
                pressingHandler -= handler;
                return TryDetachEvents();
            }

            private bool TryDetachEvents()
            {
                if (pressedHandler == null && pressingHandler == null)
                {
                    window.PreviewKeyDown -= OnPreviewKeyDown;
                    window.PreviewKeyUp -= OnPreviewKeyUp;
                    window.LostFocus -= OnLostFocus;
                    window.Deactivated -= OnDeactivated;
                    return true;
                }

                return false;
            }

            private bool IsAccessKeyDown() 
                => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        }
    }
}
