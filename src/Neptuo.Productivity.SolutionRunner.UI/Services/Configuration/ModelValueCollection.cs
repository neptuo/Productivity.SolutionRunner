using Neptuo.Collections.Specialized;
using Neptuo.Converters;
using Neptuo.PresentationModels;
using Neptuo.PresentationModels.TypeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public class ModelValueCollection : IKeyValueCollection
    {
        private readonly IModelDefinition modelDefinition;
        private readonly IConverterRepository converters;

        public ReflectionModelValueProvider<ISettings> ValueProvider { get; }
        public IEnumerable<string> Keys { get; }

        public ModelValueCollection(ReflectionModelValueProvider<ISettings> valueProvider, IModelDefinition modelDefinition)
            : this(valueProvider, modelDefinition, Converts.Repository)
        { }

        public ModelValueCollection(ReflectionModelValueProvider<ISettings> valueProvider, IModelDefinition modelDefinition, IConverterRepository converters)
        {
            Ensure.NotNull(valueProvider, "valueProvider");
            Ensure.NotNull(modelDefinition, "modelDefinition");
            Ensure.NotNull(converters, "converters");
            this.modelDefinition = modelDefinition;
            this.converters = converters;

            ValueProvider = valueProvider;
            Keys = modelDefinition.Fields.Select(f => f.Identifier);
        }

        public IKeyValueCollection Add(string key, object value)
        {
            ValueProvider.TrySetValue(key, value);
            return this;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (ValueProvider.TryGetValue(key, out object rawValue))
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
