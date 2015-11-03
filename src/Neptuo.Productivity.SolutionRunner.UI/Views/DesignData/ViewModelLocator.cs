using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Converters;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                    mainViewModel = new MainViewModel(new FileSearchService(), () => FileSearchMode.StartsWith);
                    mainViewModel.SearchPattern = "Magic.sln";
                    mainViewModel
                        .Add("Visual Studio 2015", "devenv.exe", new BitmapImage())
                        .Add("Visual Studio 2013", "devenv.exe", new BitmapImage())
                        .Add("Visual Studio 2012", "devenv.exe", new BitmapImage())
                        .Add("Visual Studio 2010", "devenv.exe", new BitmapImage());
                }

                return mainViewModel;
            }
        }

        private class FileSearchService : IFileSearchService
        {
            public Task SearchAsync(string searchPattern, FileSearchMode mode, IFileCollection files)
            {
                files.Add("Neptuo", @"C:\Development\Framework\Neptuo.sln", true);
                files.Add("Neptuo.Templates", @"C:\Development\Templates\Neptuo.Templates.sln", false);
                files.Add("Neptuo.Productivity", @"C:\Development\Productivity\Neptuo.Productivity.sln", false);
                files.Add("Neptuo.Productivity.SolutionRunner", @"C:\Development\Productivity\Neptuo.Productivity.SolutionRunner.sln", false);
                files.Add("Sample", @"C:\Development\Sample.sln", false);
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
                        .Add<string, KeyViewModel>(new StringToKeyViewModelConverter())
                        .Add<KeyViewModel, string>(new KeyViewModelToStringConverter());

                    configurationViewModel = new ConfigurationViewModel(new SaveConfigurationCommandFactory());
                    configurationViewModel.SourceDirectoryPath = @"D:\Development";
                    configurationViewModel.PreferedApplicationPath = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe";
                    configurationViewModel.FileSearchMode = FileSearchMode.Contains;
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


        #endregion
    }
}
