using Neptuo.Collections.Specialized;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class AdditionalApplicationCollection : ICompositeModel, IEnumerable<AdditionalApplicationModel>
    {
        private List<AdditionalApplicationModel> items;

        public IEnumerable<AdditionalApplicationModel> Items
        {
            get { return items; }
        }

        public AdditionalApplicationCollection()
            : this(new List<AdditionalApplicationModel>())
        { }

        public AdditionalApplicationCollection(IEnumerable<AdditionalApplicationModel> items)
        {
            Ensure.NotNull(items, "items");
            this.items = new List<AdditionalApplicationModel>(items);
        }

        public void Load(ICompositeStorage storage)
        {
            List<AdditionalApplicationModel> items = LoadCollection(storage);
            if (items != null)
                this.items = items;
            else
                this.items.Clear();
        }

        private List<AdditionalApplicationModel> LoadCollection(ICompositeStorage storage)
        {
            List<AdditionalApplicationModel> result = new List<AdditionalApplicationModel>();

            int count = storage.Get("Count", 0);
            for (int i = 0; i < count; i++)
            {
                if (storage.TryGet(i.ToString(), out ICompositeStorage itemStorage))
                {
                    AdditionalApplicationModel model = LoadItem(itemStorage);
                    if (model != null)
                        result.Add(model);
                }
            }

            if (result.Count == 0)
                return null;

            return result;
        }

        private AdditionalApplicationModel LoadItem(ICompositeStorage storage)
        {
            string name = storage.Get<string>("Name");
            string path = storage.Get<string>("Path");
            string arguments = storage.Get<string>("Arguments", null);
            bool isAdministratorRequired = storage.Get<bool>("IsAdministratorRequired", false);
            Key hotKey = storage.Get("HotKey", Key.None);

            IReadOnlyList<AdditionalApplicationModel> commands = LoadCollection(storage);
            if (commands == null)
                return new AdditionalApplicationModel(name, path, arguments, isAdministratorRequired, hotKey);
            else
                return new AdditionalApplicationModel(name, path, arguments, isAdministratorRequired, hotKey, commands);
        }

        public void Save(ICompositeStorage storage)
        {
            SaveCollection(storage, items);
        }

        private void SaveCollection(ICompositeStorage storage, IReadOnlyList<AdditionalApplicationModel> models)
        {
            if (models == null || models.Count == 0)
                return;

            storage.Add("Count", models.Count);
            for (int i = 0; i < models.Count; i++)
            {
                AdditionalApplicationModel model = models[i];
                ICompositeStorage itemStorage = storage.Add(i.ToString());
                SaveItem(itemStorage, model);
            }
        }

        private void SaveItem(ICompositeStorage storage, AdditionalApplicationModel model)
        {
            storage.Add("Name", model.Name);
            storage.Add("Path", model.Path);
            storage.Add("Arguments", model.Arguments);
            storage.Add("IsAdministratorRequired", model.IsAdministratorRequired);

            if (model.HotKey != Key.None)
                storage.Add("HotKey", model.HotKey);

            SaveCollection(storage, model.Commands);
        }

        public IEnumerator<AdditionalApplicationModel> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
