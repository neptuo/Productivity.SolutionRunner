using Neptuo.Activators;
using Neptuo.Formatters;
using Neptuo.Productivity.SolutionRunner.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class AdditionalApplicationLoader
    {
        public void Add(IApplicationCollection applications)
        {
            string rawValue = Settings.Default.AdditionalApplications;
            if (!String.IsNullOrEmpty(rawValue))
            {
                AdditionalApplicationCollection collection = Converts
                    .To<string, AdditionalApplicationCollection>(rawValue);

                foreach (AdditionalApplicationModel model in collection.Items)
                    applications.Add(model.Name, model.Path, model.Arguments, IconExtractor.Get(model.Path), model.HotKey, false);
            }
        }
    }
}
