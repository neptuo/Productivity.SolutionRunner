using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class UiBackgroundContext : ObservableModel, IBackgroundContext
    {
        private int currentCount;

        public int CurrentCount
        {
            get { return currentCount; }
            set
            {
                if (currentCount != value)
                {
                    currentCount = value;
                    IsWorking = currentCount != 0;
                }
            }
        }

        private bool isWorking;
        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                if (isWorking != value)
                {
                    isWorking = value;
                    RaisePropertyChanged();
                }
            }
        }


        public IDisposable Start() => new Disposable(this);


        private class Disposable : DisposableBase
        {
            private readonly UiBackgroundContext context;

            public Disposable(UiBackgroundContext context)
            {
                Ensure.NotNull(context, "context");
                this.context = context;
                this.context.CurrentCount++;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();
                context.CurrentCount--;
            }
        }
    }
}
