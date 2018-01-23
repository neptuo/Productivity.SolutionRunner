using Neptuo.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Converters
{
    public class JsonReadOnlyListConverter : TwoWayConverter<IReadOnlyList<string>, JToken>
    {
        public override bool TryConvertFromOneToTwo(IReadOnlyList<string> sourceValue, out JToken targetValue)
        {
            if (sourceValue == null)
            {
                targetValue = null;
                return true;
            }

            targetValue = new JArray(sourceValue);
            return true;
        }

        public override bool TryConvertFromTwoToOne(JToken sourceValue, out IReadOnlyList<string> targetValue)
        {
            if (sourceValue is JArray array)
            {
                targetValue = new List<string>(array.Select(value => value.ToString()));
                return true;
            }

            targetValue = null;
            return false;
        }
    }
}
