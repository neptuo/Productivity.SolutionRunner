using Neptuo;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class PreferedApplicationCollection : ObservableCollection<IPreferedApplicationViewModel>
    {
        public PreferedApplicationCollection AddCollectionChanged(ObservableCollection<MainApplicationListViewModel> collection)
        {
            Ensure.NotNull(collection, "collection");
            collection.CollectionChanged += OnApplicationsChanged;
            AddRange(collection);
            return this;
        }

        public PreferedApplicationCollection AddCollectionChanged(ObservableCollection<AdditionalApplicationListViewModel> collection)
        {
            Ensure.NotNull(collection, "collection");
            collection.CollectionChanged += OnApplicationsChanged;
            AddRange(collection);
            return this;
        }

        protected override void InsertItem(int index, IPreferedApplicationViewModel item)
        {
            base.InsertItem(index, item);

            MainApplicationListViewModel main = item as MainApplicationListViewModel;
            if (main != null)
                main.PropertyChanged += OnMainPropertyChanged;
        }

        protected override void RemoveItem(int index)
        {
            MainApplicationListViewModel main = this[index] as MainApplicationListViewModel;
            if (main != null)
                main.PropertyChanged -= OnMainPropertyChanged;

            base.RemoveItem(index);
        }

        private void OnMainPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainApplicationListViewModel.IsEnabled))
            {
                MainApplicationListViewModel main = (MainApplicationListViewModel)sender;
                if (main.IsEnabled)
                    base.InsertItem(Count, main);
                else
                    base.RemoveItem(IndexOf(main));
            }
        }

        private void OnApplicationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddRange(e.NewItems.OfType<IPreferedApplicationViewModel>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (IPreferedApplicationViewModel application in e.OldItems.OfType<IPreferedApplicationViewModel>())
                        Remove(application);

                    break;
                default:
                    break;
            }
        }
    }
}
