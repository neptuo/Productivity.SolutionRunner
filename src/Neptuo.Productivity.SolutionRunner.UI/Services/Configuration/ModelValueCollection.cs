using Neptuo.Collections.Specialized;
using Neptuo.Converters;
using Neptuo.PresentationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public class ModelValueCollection : IKeyValueCollection
    {
        private readonly IModelValueProvider valueProvider;
        private readonly IModelDefinition modelDefinition;
        private readonly IConverterRepository converters;

        public ModelValueCollection(IModelValueProvider valueProvider, IModelDefinition modelDefinition)
            : this(valueProvider, modelDefinition, Converts.Repository)
        { }

        public ModelValueCollection(IModelValueProvider valueProvider, IModelDefinition modelDefinition, IConverterRepository converters)
        {
            Ensure.NotNull(valueProvider, "valueProvider");
            Ensure.NotNull(modelDefinition, "modelDefinition");
            Ensure.NotNull(converters, "converters");
            this.valueProvider = valueProvider;
            this.modelDefinition = modelDefinition;
            this.converters = converters;

            Keys = modelDefinition.Fields.Select(f => f.Identifier);
        }

        public IEnumerable<string> Keys { get; }

        public IKeyValueCollection Add(string key, object value)
        {
            valueProvider.TrySetValue(key, value);
            return this;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (valueProvider.TryGetValue(key, out object rawValue))
            {
                if (rawValue == null)
                {
                    value = default(T);
                    return true;
                }

                if (converters.TryConvert(rawValue.GetType(), typeof(T), rawValue, out rawValue))
                {
                    value = (T)rawValue;
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}
