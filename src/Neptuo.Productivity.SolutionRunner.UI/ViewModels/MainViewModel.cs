using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.UserConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    /// <summary>
    /// The view model of main window.
    /// </summary>
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

            IsLoading = true;
        }

        #region Initialization

        public async Task InitializeAsync()
        {
            LockFileList();
            Message = "Initializing...";

            await fileSearch.InitializeAsync();
            EventManager.FilePinned += OnFilePinned;
            IsLoading = false;
            Message = "No favourite solution files, start by typing...";
            UnLockFileList();

            if (isFileSearchReqired)
                TriggerFileSearch();
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Searching

        private void OnFilePinned(FileViewModel viewModel)
        {
            if (!viewModel.IsPinned)
                TriggerFileSearch();
        }

        private CancellationTokenSource lastFileSearchToken;

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
                    TriggerFileSearch();
                }
            }
        }

        private bool isFileSearchReqired = false;

        /// <summary>
        /// Triggers searching operation.
        /// </summary>
        private void TriggerFileSearch()
        {
            if (IsLoading)
            {
                isFileSearchReqired = true;
                return;
            }

            ((IFileCollection)this).Clear();

            if (lastFileSearchToken != null)
                lastFileSearchToken.Cancel();

            lastFileSearchToken = new CancellationTokenSource();

            Message = "Searching...";
            fileSearch.SearchAsync(SearchPattern, fileSearchModeGetter(), fileSearchCountGetter(), this, lastFileSearchToken.Token)
                .ContinueWith(UpdateMessageAfterSearching);
        }

        private bool isFileListLocked;

        /// <summary>
        /// Locks availability of file list.
        /// Even if list should be available, it is not.
        /// </summary>
        private void LockFileList()
        {
            isFileListLocked = true;
            RaisePropertyChanged(nameof(IsFileListAvailable));
        }

        /// <summary>
        /// Unlocks availability of file list.
        /// If state of availability changed when locked, <see cref="PropertyChanged"/> is raised.
        /// </summary>
        private void UnLockFileList()
        {
            isFileListLocked = false;
            RaisePropertyChanged(nameof(IsFileListAvailable));
        }

        private bool isFileListAvailable;
        public bool IsFileListAvailable
        {
            get { return !isFileListLocked && isFileListAvailable; }
            set
            {
                if (isFileListAvailable != value)
                {
                    isFileListAvailable = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Updates message text after completing search.
        /// If <paramref name="task"/> is <see cref="Task.IsCanceled"/>, nothing is done.
        /// </summary>
        /// <param name="task">The task of completed search.</param>
        private void UpdateMessageAfterSearching(Task task)
        {
            if (task.IsCanceled || IsLoading)
                return;
            
            lastFileSearchToken = null;
            if (String.IsNullOrEmpty(SearchPattern))
                Message = "No favourite solution files, start by typing...";
            else
                Message = "No matching solution file found";

            RaisePropertyChanged(nameof(IsFileListAvailable));
        }

        #endregion

        #region Files

        private ObservableCollection<FileViewModel> files;
        public IEnumerable<FileViewModel> Files
        {
            get { return files; }
        }

        IFileCollection IFileCollection.Clear()
        {
            IsFileListAvailable = false;
            files.Clear();
            return this;
        }

        IFileCollection IFileCollection.Add(string fileName, string filePath, bool isPinned)
        {
            files.Add(new FileViewModel(fileName, filePath, isPinned));
            IsFileListAvailable = true;
            return this;
        }

        #endregion

        #region Applications

        private readonly ObservableCollection<ApplicationViewModel> applications;
        public IEnumerable<ApplicationViewModel> Applications
        {
            get { return applications; }
        }

        public IApplicationCollection Add(string name, string path, string arguments, ImageSource icon, bool isMain)
        {
            applications.Add(new ApplicationViewModel(name, null, path, arguments, icon, isMain));
            return this;
        }

        public IApplicationCollection Add(string name, Version version, string path, string arguments, ImageSource icon, bool isMain)
        {
            applications.Add(new ApplicationViewModel(name, version, path, arguments, icon, isMain));
            return this;
        }

        #endregion

        #region Disposing

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            EventManager.FilePinned -= OnFilePinned;

            IDisposable disposable = fileSearch as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        #endregion
    }
}
