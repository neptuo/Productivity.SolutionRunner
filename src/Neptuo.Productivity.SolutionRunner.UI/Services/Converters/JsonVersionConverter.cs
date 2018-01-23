using Neptuo.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Converters
{
    public class JsonVersionConverter : TwoWayConverter<Version, JToken>
    {
        public override bool TryConvertFromOneToTwo(Version sourceValue, out JToken targetValue)
        {
            if (sourceValue == null)
            {
                targetValue = null;
                return true;
            }

            targetValue = new JValue(sourceValue.ToString(3));
            return true;
        }

        public override bool TryConvertFromTwoToOne(JToken sourceValue, out Version targetValue)
        {
            if (sourceValue is JValue value)
            {
                if (value.Value == null)
                {
                    targetValue = null;
                    return true;
                }

                if (Version.TryParse(value.ToString(), out targetValue))
                    return true;
            }

            targetValue = null;
            return false;
        }
    }
}
