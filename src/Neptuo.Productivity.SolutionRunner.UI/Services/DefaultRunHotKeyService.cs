using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Windows.HotKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public class DefaultRunHotKeyService : DisposableBase, IRunHotKeyService
    {
        private readonly INavigator navigator;
        private readonly INavigatorState state;
        private ComponentDispatcherHotkeyCollection hotKeys;
        private KeyViewModel runKey;

        public DefaultRunHotKeyService(INavigator navigator, INavigatorState state)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(state, "state");
            this.navigator = navigator;
            this.state = state;
            this.hotKeys = new ComponentDispatcherHotkeyCollection();
        }

        public bool IsSet
        {
            get { return runKey != null; }
        }

        public IRunHotKeyService Bind(Key key, ModifierKeys modifier)
        {
            if (runKey == null || runKey.Key != key || runKey.Modifier != modifier)
            {
                hotKeys.Add(key, modifier, OnRunHotKeyPressed);
                UnBind();

                runKey = new KeyViewModel(key, modifier);
            }

            return this;
        }

        public IRunHotKeyService UnBind()
        {
            if (runKey != null)
                hotKeys.Remove(runKey.Key, runKey.Modifier);

            runKey = null;
            return this;
        }

        private void OnRunHotKeyPressed(Key key, ModifierKeys modifier)
        {
            if (state.IsConfigurationOpened)
                navigator.OpenConfiguration();
            else
                navigator.OpenMain();
        }

        public KeyViewModel FindKeyViewModel()
        {
            return runKey;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            hotKeys.Dispose();
        }
    }
}
