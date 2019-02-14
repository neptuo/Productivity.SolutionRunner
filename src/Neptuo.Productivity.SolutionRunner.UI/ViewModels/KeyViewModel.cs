using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class KeyViewModel : ObservableModel
    {
        private Key key;
        public Key Key
        {
            get { return key; }
            set
            {
                if (key != value)
                {
                    key = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ModifierKeys modifier;
        public ModifierKeys Modifier
        {
            get { return modifier; }
            set
            {
                if (modifier != value)
                {
                    modifier = value;
                    RaisePropertyChanged();
                }
            }
        }

        public KeyViewModel()
        { }

        public KeyViewModel(Key key, ModifierKeys modifier)
        {
            Key = key;
            Modifier = modifier;
        }
    }
}
