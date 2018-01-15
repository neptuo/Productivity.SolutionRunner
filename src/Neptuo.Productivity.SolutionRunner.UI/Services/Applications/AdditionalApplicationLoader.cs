using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
        private readonly ISettings settings;

        public AdditionalApplicationLoader(ISettings settings)
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;
        }

        public void Add(IApplicationCollection applications)
        {
            string rawValue = settings.AdditionalApplications;
            if (!String.IsNullOrEmpty(rawValue))
            {
                AdditionalApplicationCollection collection = Converts
                    .To<string, AdditionalApplicationCollection>(rawValue);

                foreach (AdditionalApplicationModel model in collection.Items)
                {
                    IApplicationBuilder builder = applications.Add(
                        model.Name, 
                        model.Path, 
                        model.Arguments, 
                        model.IsAdministratorRequired, 
                        IconExtractor.Get(model.Path), 
                        model.HotKey, 
                        false
                    );

                    foreach (AdditionalApplicationModel commandModel in model.Commands)
                    {
                        builder.AddCommand(
                            commandModel.Name, 
                            commandModel.Path, 
                            commandModel.Arguments, 
                            commandModel.IsAdministratorRequired, 
                            commandModel.HotKey
                        );
                    }
                }
            }
        }
    }
}
