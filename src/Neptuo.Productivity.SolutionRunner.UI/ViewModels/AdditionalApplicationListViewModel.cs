using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class AdditionalApplicationListViewModel : ObservableObject, IPreferedApplicationViewModel
    {
        public AdditionalApplicationModel Model { get; private set; }

        private ImageSource icon;
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return Model.Name; }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AdditionalApplicationListViewModel(AdditionalApplicationModel model)
        {
            Ensure.NotNull(model, "model");

            UpdateModel(model);
        }

        public void UpdateModel(AdditionalApplicationModel model)
        {
            Ensure.NotNull(model, "model");
            Icon = IconExtractor.Get(model.Path);
            Path = model.Path;
            Model = model;
            RaisePropertyChanged(nameof(Name));
        }
    }
}
