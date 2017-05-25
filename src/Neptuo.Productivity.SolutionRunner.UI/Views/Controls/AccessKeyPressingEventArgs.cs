using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public class AccessKeyPressingEventArgs : EventArgs
    {
        public IReadOnlyList<Key> PreviousKeys { get; private set; }
        public Key LastKey { get; private set; }

        public bool IsCancelled { get; set; }

        public AccessKeyPressingEventArgs(IReadOnlyList<Key> previousKeys, Key lastKey)
        {
            Ensure.NotNull(previousKeys, "previousKeys");
            PreviousKeys = previousKeys;
            LastKey = lastKey;
        }
    }
}
