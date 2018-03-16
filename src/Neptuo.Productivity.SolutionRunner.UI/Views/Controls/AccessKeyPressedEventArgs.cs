using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public class AccessKeyPressedEventArgs : EventArgs
    {
        public IReadOnlyList<Key> Keys { get; private set; }

        public AccessKeyPressedEventArgs(IReadOnlyList<Key> keys)
        {
            Ensure.NotNull(keys, "keys");
            Keys = keys;
        }
    }
}
