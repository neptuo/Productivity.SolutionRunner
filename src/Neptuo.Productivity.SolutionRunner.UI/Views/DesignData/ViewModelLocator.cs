using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Converters;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.UI.DesignData
{
    internal class ViewModelLocator
    {
        #region Main
        
        private static MainViewModel mainViewModel;

        public static MainViewModel MainViewModel
        {
            get
            {
                if (mainViewModel == null)
                {
                    mainViewModel = new MainViewModel(new FileSearchService(), () => FileSearchMode.StartsWith, () => 20);
                    mainViewModel.SearchPattern = "Magic.sln";
                    mainViewModel.IsLoading = false;

                    VsVersionLoader loader = new VsVersionLoader();
                    loader.Add(mainViewModel);

                    mainViewModel.Add("Notepad", @"C:\Windows\notepad.exe", null, IconExtractor.Get(@"C:\Windows\notepad.exe"), false);
                    mainViewModel.Add("Second notepad", @"C:\Windows\notepad.exe", null, IconExtractor.Get(@"C:\Windows\notepad.exe"), false);
                }

                return mainViewModel;
            }
        }

        private class FileSearchService : IFileSearchService
        {
            public Task InitializeAsync()
            {
                return Async.CompletedTask;
            }

            public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
            {
                //files.Add("Neptuo", @"C:\Development\Framework\Neptuo.sln", true);
                //files.Add("Neptuo.Templates", @"C:\Development\Templates\Neptuo.Templates.sln", false);
                //files.Add("Neptuo.Productivity", @"C:\Development\Productivity\Neptuo.Productivity.sln", false);
                //files.Add("Neptuo.Productivity.SolutionRunner", @"C:\Development\Productivity\Neptuo.Productivity.SolutionRunner.sln", false);
                //files.Add("Sample", @"C:\Development\Sample.sln", false);
                return Task.FromResult(true);
            }
        }

        #endregion

        #region Configuration

        private static ConfigurationViewModel configurationViewModel;

        public static ConfigurationViewModel ConfigurationViewModel
        {
            get
            {
                if (configurationViewModel == null)
                {
                    Converts.Repository
                        .Add(new KeyViewModelConverter());

                    configurationViewModel = new ConfigurationViewModel(new SaveConfigurationCommandFactory(), new Navigator());
                    configurationViewModel.SourceDirectoryPath = @"D:\Development";
                    configurationViewModel.PreferedApplicationPath = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe";
                    configurationViewModel.FileSearchMode = FileSearchMode.Contains;
                    configurationViewModel.FileSearchCount = 10;
                    configurationViewModel.IsFileSearchPatternSaved = true;
                    configurationViewModel.AdditionalApplications = new ObservableCollection<AdditionalApplicationListViewModel>()
                    {
                        new AdditionalApplicationListViewModel(new AdditionalApplicationModel("Notepad", @"C:\Windows\notepad.exe", "")),
                        new AdditionalApplicationListViewModel(new AdditionalApplicationModel("GitExtensions", @"C:\Program Files (x86)\GitExtensions\GitExtensions.exe", ""))
                    };
                    configurationViewModel.RunKey = new KeyViewModel(Key.V, ModifierKeys.Control);
                }

                return configurationViewModel;
            }
        }

        private class SaveConfigurationCommandFactory : IFactory<SaveConfigurationCommand, ConfigurationViewModel>
        {
            public SaveConfigurationCommand Create(ConfigurationViewModel viewModel)
            {
                return new SaveConfigurationCommand(viewModel, new RunHotKeyService());
            }
        }

        private class RunHotKeyService : IRunHotKeyService
        {
            public IRunHotKeyService Bind(Key key, ModifierKeys modifier)
            {
                return this;
            }

            public IRunHotKeyService UnBind()
            {
                return this;
            }

            public KeyViewModel FindKeyViewModel()
            {
                return new KeyViewModel(Key.K, ModifierKeys.Control);
            }

            public bool IsSet
            {
                get { return false; }
            }
        }

        private class Navigator : INavigator
        {
            public void OpenAdditionalApplicationEdit(AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
            {
                throw new NotImplementedException();
            }

            public void OpenConfiguration()
            {
                throw new NotImplementedException();
            }

            public void OpenMain()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region AdditionalApplicationEdit

        private static AdditionalApplicationEditViewModel additionalApplication;

        public static AdditionalApplicationEditViewModel AdditionalApplication
        {
            get
            {
                if(additionalApplication == null)
                {
                    additionalApplication = new AdditionalApplicationEditViewModel(null, m => { });
                    additionalApplication.Path = @"C:\Windows\notepad.exe";
                    additionalApplication.Arguments = "browse {FilePath}";
                    additionalApplication.Icon = IconExtractor.Get(@"C:\Windows\notepad.exe");
                }

                return additionalApplication;
            }
        }

        #endregion
    }
}
