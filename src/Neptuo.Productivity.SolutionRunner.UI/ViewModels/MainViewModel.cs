using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
using Neptuo.Linq.Expressions;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.UserConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class MainViewModel : ObservableObject, IApplicationCollection, IFileCollection, IDisposable
    {
        private readonly IFileSearchService fileSearch;
        private readonly FileSearchModeGetter fileSearchModeGetter;
        private readonly FileSearchCountGetter fileSearchCountGetter;

        public MainViewModel(IFileSearchService fileSearch, FileSearchModeGetter fileSearchModeGetter, FileSearchCountGetter fileSearchCountGetter)
        {
            Ensure.NotNull(fileSearch, "fileSearch");
            Ensure.NotNull(fileSearchModeGetter, "fileSearchModeGetter");
            Ensure.NotNull(fileSearchCountGetter, "fileSearchCountGetter");
            this.fileSearch = fileSearch;
            this.fileSearchModeGetter = fileSearchModeGetter;
            this.fileSearchCountGetter = fileSearchCountGetter;

            applications = new ObservableCollection<ApplicationViewModel>();
            files = new ObservableCollection<FileViewModel>();
        }

        #region Searching

        private string searchPattern;
        public string SearchPattern
        {
            get { return searchPattern; }
            set
            {
                if (searchPattern != value)
                {
                    searchPattern = value;
                    RaisePropertyChanged();
                    fileSearch.SearchAsync(searchPattern, fileSearchModeGetter(), fileSearchCountGetter(), this);
                }
            }
        }

        private ObservableCollection<FileViewModel> files;
        public IEnumerable<FileViewModel> Files
        {
            get { return files; }
        }

        IFileCollection IFileCollection.Clear()
        {
            files.Clear();
            return this;
        }

        IFileCollection IFileCollection.Add(string fileName, string filePath, bool isPinned)
        {
            files.Add(new FileViewModel(fileName, filePath, isPinned));
            return this;
        }

        #endregion

        #region Applications

        private readonly ObservableCollection<ApplicationViewModel> applications;
        public IEnumerable<ApplicationViewModel> Applications
        {
            get { return applications; }
        }

        public IApplicationCollection Add(string name, string path, ImageSource icon)
        {
            applications.Add(new ApplicationViewModel(name, path, icon));
            return this;
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            IDisposable disposable = fileSearch as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        #endregion
    }
}
