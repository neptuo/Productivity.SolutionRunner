﻿using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
            foreach (AdditionalApplicationModel model in settings.AdditionalApplications)
            {
                ImageSource icon = null;
                if (model.IconData == null)
                    icon = IconExtractor.Get(model.Path);
                else
                    icon = Base64ImageCoder.GetImageFromString(model.IconData);

                IApplicationBuilder builder = applications.Add(
                    model.Name,
                    model.Path,
                    null,
                    model.Arguments,
                    model.IsAdministratorRequired,
                    model.IsApplicationWindowShown,
                    icon,
                    model.HotKey,
                    false
                );

                foreach (AdditionalApplicationModel commandModel in model.Commands)
                {
                    builder.AddCommand(
                        commandModel.Name,
                        commandModel.Path,
                        null,
                        commandModel.Arguments,
                        commandModel.IsAdministratorRequired,
                        commandModel.IsApplicationWindowShown,
                        commandModel.HotKey
                    );
                }
            }
        }
    }
}
