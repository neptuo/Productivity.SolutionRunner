using Neptuo.Observables;
using Neptuo.Observables.Collections;
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

        public ObservableCollection<AdditionalApplicationListViewModel> Commands { get; private set; }

        public ICommand RemoveAdditionalApplicationCommand { get; private set; }
        public ICommand EditAdditionalApplicationCommand { get; private set; }
        public ICommand CreateAdditionalApplicationCommand { get; private set; }

        public AdditionalApplicationListViewModel(AdditionalApplicationModel model)
        {
            UpdateModel(model);
        }

        public void UpdateModel(AdditionalApplicationModel model)
        {
            Ensure.NotNull(model, "model");
            Icon = IconExtractor.Get(model.Path);
            Path = model.Path;
            Model = model;
            Commands = new ObservableCollection<AdditionalApplicationListViewModel>(
                model.Commands.Select(m => new AdditionalApplicationListViewModel(m))
            );

            EditAdditionalApplicationCommand = new EditAdditionalApplicationCommand(this, navigator);
            RemoveAdditionalApplicationCommand = new RemoveAdditionalApplicationCommand(this);
            CreateAdditionalApplicationCommand = new CreateAdditionalApplicationCommand(this, navigator);
        }
    }
}
