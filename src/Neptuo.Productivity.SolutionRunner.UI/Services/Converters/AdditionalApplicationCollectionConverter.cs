using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.Formatters;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Converters
{
    public class AdditionalApplicationCollectionConverter : TwoWayConverter<AdditionalApplicationCollection, string>
    {
        public override bool TryConvertFromOneToTwo(AdditionalApplicationCollection sourceValue, out string targetValue)
        {
            if (sourceValue != null && sourceValue.Any())
            {
                CompositeModelFormatter formatter = new CompositeModelFormatter(
                    type => Activator.CreateInstance(type),
                    Factory.Getter(() => new JsonCompositeStorage())
                );

                targetValue = formatter.Serialize(sourceValue);
                return true;
            }

            targetValue = null;
            return true;
        }

        public override bool TryConvertFromTwoToOne(string sourceValue, out AdditionalApplicationCollection targetValue)
        {
            if (String.IsNullOrEmpty(sourceValue))
            {
                targetValue = new AdditionalApplicationCollection();
                return true;
            }

            CompositeModelFormatter formatter = new CompositeModelFormatter(
                type => Activator.CreateInstance(type),
                Factory.Getter(() => new JsonCompositeStorage())
            );

            targetValue = formatter.Deserialize<AdditionalApplicationCollection>(sourceValue);
            return true;
        }
    }
}
