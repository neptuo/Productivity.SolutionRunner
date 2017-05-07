using Neptuo;
using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Colors;
using Neptuo.Productivity.SolutionRunner.Services.Converters;
using Neptuo.Productivity.SolutionRunner.Services.Positions;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
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
                    Settings.Default.IsFileNameRemovedFromDisplayedPath = true;
                    //Settings.Default.IsDisplayedPathTrimmedToLastFolderName = true;

                    mainViewModel = new MainViewModel(new FileSearchService(true), () => FileSearchMode.StartsWith, () => 20);
                    mainViewModel.IsLoading = false;
                    mainViewModel.SearchPattern = "Neptuo.sln";

                    VsVersionLoader loader = new VsVersionLoader();
                    loader.Add(mainViewModel);

                    mainViewModel.Add("File Explorer", @"C:\Windows\explorer.exe", null, IconExtractor.Get(@"C:\Windows\explorer.exe"), Key.E, false);
                    mainViewModel.Add("Notepad", @"C:\Windows\notepad.exe", null, IconExtractor.Get(@"C:\Windows\notepad.exe"), Key.N, false);
                }

                return mainViewModel;
            }
        }

        private class FileSearchService : IFileSearchService
        {
            private readonly bool hasItems;

            public FileSearchService(bool hasItems)
            {
                this.hasItems = hasItems;
            }

            public Task InitializeAsync()
            {
                return Async.CompletedTask;
            }

            public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
            {
                if (hasItems)
                {
                    files.Add("Neptuo", @"C:\Development\Framework\Neptuo.sln", true);
                    files.Add("Neptuo.Templates", @"C:\Development\Templates\Neptuo.Templates.sln", false);
                    files.Add("Neptuo.Productivity", @"C:\Development\Productivity\Neptuo.Productivity.sln", false);
                    files.Add("Neptuo.Productivity.SolutionRunner", @"C:\Development\Productivity\Neptuo.Productivity.SolutionRunner.sln", false);
                    files.Add("Sample", @"C:\Development\Sample.sln", false);
                }
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
                    configurationViewModel.FileSearchMode = FileSearchMode.Contains;
                    configurationViewModel.FileSearchCount = 10;
                    configurationViewModel.IsFileSearchPatternSaved = true;
                    configurationViewModel.IsAutoSelectApplicationVersion = true;
                    PreferedApplicationCollection preferedApplications = new PreferedApplicationCollection();
                    configurationViewModel.AdditionalApplications = new ObservableCollection<AdditionalApplicationListViewModel>();
                    preferedApplications.AddCollectionChanged(configurationViewModel.AdditionalApplications);
                    configurationViewModel.AdditionalApplications.AddRange(
                        new AdditionalApplicationListViewModel(new AdditionalApplicationModel("Notepad", @"C:\Windows\notepad.exe", "", Key.N)),
                        new AdditionalApplicationListViewModel(new AdditionalApplicationModel("GitExtensions", @"C:\Program Files (x86)\GitExtensions\GitExtensions.exe", "", Key.G))
                    );
                    configurationViewModel.MainApplications = new ObservableCollection<MainApplicationListViewModel>();
                    preferedApplications.AddCollectionChanged(configurationViewModel.MainApplications);
                    configurationViewModel.MainApplications.AddRange(
                        new MainApplicationListViewModel()
                        {
                            Name = "Visual Studio 12.0",
                            Path = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe",
                            Icon = IconExtractor.Get(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"),
                            IsEnabled = false
                        },
                        new MainApplicationListViewModel()
                        {
                            Name = "Visual Studio 14.0",
                            Path = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                            Icon = IconExtractor.Get(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"),
                            IsEnabled = true
                        }
                    );
                    configurationViewModel.PreferedApplications = preferedApplications;
                    configurationViewModel.PreferedApplication = preferedApplications.FirstOrDefault();
                    configurationViewModel.VsVersions = new ObservableCollection<VersionViewModel>()
                    {
                        VersionViewModel.Empty(),
                        new VersionViewModel(new Version(12, 0)),
                        new VersionViewModel(new Version(14, 0))
                    };
                    configurationViewModel.RunKey = new KeyViewModel(Key.V, ModifierKeys.Control);

                    configurationViewModel.PositionMode = PositionMode.UserDefined;
                    configurationViewModel.PositionLeft = 450;
                    configurationViewModel.PositionTop = 20;
                }

                return configurationViewModel;
            }
        }

        private class SaveConfigurationCommandFactory : IFactory<SaveConfigurationCommand, ConfigurationViewModel>
        {
            public SaveConfigurationCommand Create(ConfigurationViewModel viewModel)
            {
                return new SaveConfigurationCommand(
                    viewModel,
                    Settings.Default,
                    new RunHotKeyService(),
                    new ShortcutService(
                        "Neptuo",
                        "Productivity",
                        "SolutionRunner"
                    )
                );
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

            public void OpenStatistics()
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
                if (additionalApplication == null)
                {
                    additionalApplication = new AdditionalApplicationEditViewModel(new Navigator(), null, m => { });
                    additionalApplication.Path = @"C:\Windows\notepad.exe";
                    additionalApplication.Arguments = "browse {FilePath}";
                    additionalApplication.Icon = IconExtractor.Get(@"C:\Windows\notepad.exe");
                }

                return additionalApplication;
            }
        }

        #endregion

        #region Statistics

        private static ContainerCollection<ContainerCollection<StatisticsViewModel>> statistics;

        public static ContainerCollection<ContainerCollection<StatisticsViewModel>> Statistics
        {
            get
            {
                if (statistics == null)
                {
                    statistics = new ContainerCollection<ContainerCollection<StatisticsViewModel>>();
                    statistics.Add(new Container<ContainerCollection<StatisticsViewModel>>()
                    {
                        Title = "2017",
                        Data = new ContainerCollection<StatisticsViewModel>()
                        {
                            new Container<StatisticsViewModel>()
                            {
                                Title = "January",
                                Data = new StatisticsViewModel(new RandomColorGenerator())
                            },
                            new Container<StatisticsViewModel>()
                            {
                                Title = "February",
                                Data = new StatisticsViewModel(new RandomColorGenerator())
                            },
                            new Container<StatisticsViewModel>()
                            {
                                Title = "March",
                                Data = new StatisticsViewModel(new RandomColorGenerator())
                            }
                        }
                    });
                    statistics.Add(new Container<ContainerCollection<StatisticsViewModel>>()
                    {
                        Title = "2016",
                        Data = new ContainerCollection<StatisticsViewModel>()
                    });

                    statistics[0].Data[0].Data.AddApplication(@"C:\Windows\Notepad.exe", 15);
                    statistics[0].Data[0].Data.AddApplication(@"C:\Windows\calc.exe", 22);
                    statistics[0].Data[0].Data.AddApplication(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe", 103);
                    statistics[0].Data[0].Data.AddApplication(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe", 14);

                    statistics[0].Data[0].Data.AddFile(@"C:\Temp\Log.txt", 8);
                    statistics[0].Data[0].Data.AddFile(@"C:\Development\Project1.sln", 34);
                    statistics[0].Data[0].Data.AddFile(@"C:\Development\Project2.sln", 4);
                    statistics[0].Data[0].Data.AddFile(@"C:\Development\Slider.sln", 18);
                }

                return statistics;
            }
        }

        #endregion
    }
}
