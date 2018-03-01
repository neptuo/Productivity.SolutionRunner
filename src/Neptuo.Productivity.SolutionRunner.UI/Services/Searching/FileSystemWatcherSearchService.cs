using Neptuo.FileSystems;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class FileSystemWatcherSearchService : DisposableBase, IFileSearchService
    {
        private readonly string directoryPath;
        private readonly List<FileSystemWatcher> watchers;
        private readonly IPinStateService pinStateService;
        private readonly PatternMatcherFactory matcherFactory = new PatternMatcherFactory();
        private readonly FileCache fileCache;

        private readonly FileStorage storage = new FileStorage();

        public FileSystemWatcherSearchService(string directoryPath, IPinStateService pinStateService)
        {
            Ensure.Condition.DirectoryExists(directoryPath, "directoryPath");
            Ensure.NotNull(pinStateService, "pinStateService");
            this.directoryPath = directoryPath;
            this.pinStateService = pinStateService;
            this.watchers = new List<FileSystemWatcher>();
            this.fileCache = new FileCache();
        }

        private IEnumerable<FileModel> EnumerateDirectory(string directoryPath)
        {
            return Directory
                .GetFiles(directoryPath, "*.sln", SearchOption.AllDirectories)
                .Select(f => new FileModel(f));
        }

        public Task InitializeAsync()
        {
            if (watchers.Count == 0)
            {
                Action initializeFromDirectory = () =>
                {
                    storage.AddRange(EnumerateDirectory(directoryPath));
                    storage.IsCacheUsed = false;
                };

                if (!storage.IsCacheEmpty)
                {
                    Task.Factory.StartNew(initializeFromDirectory);
                }

                return Task.Factory.StartNew(() =>
                {
                    if (watchers.Count == 0)
                    {
                        InitializeWatchers(directoryPath);
                        if (storage.IsCacheEmpty)
                            initializeFromDirectory();
                    }
                });
            }

            return Async.CompletedTask;
        }

        private void InitializeWatchers(string directoryPath)
        {
            // Watcher for changes on directory structure.
            FileSystemWatcher directoryWatcher = new FileSystemWatcher(directoryPath);
            directoryWatcher.EnableRaisingEvents = true;
            directoryWatcher.IncludeSubdirectories = true;
            directoryWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            directoryWatcher.Created += (sender, e) => SafeChangeWatcher(sender, e, OnFileCreated);
            directoryWatcher.Deleted += (sender, e) => SafeChangeWatcher(sender, e, OnFileDeleted);
            directoryWatcher.Renamed += (sender, e) => SafeChangeWatcher(sender, e, OnFileRenamed);
            watchers.Add(directoryWatcher);

            // Watcher for changes on *.sln files.
            FileSystemWatcher fileWatcher = new FileSystemWatcher(directoryPath, "*.sln");
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = NotifyFilters.FileName;
            fileWatcher.Created += (sender, e) => SafeChangeWatcher(sender, e, OnFileCreated);
            fileWatcher.Deleted += (sender, e) => SafeChangeWatcher(sender, e, OnFileDeleted);
            fileWatcher.Renamed += (sender, e) => SafeChangeWatcher(sender, e, OnFileRenamed);
            watchers.Add(fileWatcher);
        }

        private void SafeChangeWatcher<T>(object sender, T e, Action<object, T> inner)
            where T : EventArgs
        {
            for (int i = 1; i < 6; i++)
            {
                try
                {
                    inner(sender, e);
                    return;
                }
                catch (Exception)
                {
                    // Some times there is a problem with IO for newly created folders.
                    Thread.Sleep(500 * i);
                }
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            string extension = Path.GetExtension(e.FullPath);
            if (String.IsNullOrEmpty(extension))
            {
                foreach (FileModel file in EnumerateDirectory(e.FullPath))
                {
                    if (!storage.Any(f => f.Path == file.Path))
                        storage.Add(file);
                }
            }
            else if (extension == ".sln" && File.Exists(e.FullPath))
            {
                storage.Add(new FileModel(e.FullPath));
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (String.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                // Deleting directory.
                List<FileModel> toRemove = new List<FileModel>();
                foreach (FileModel model in storage)
                {
                    if (model.Path.StartsWith(e.FullPath))
                        toRemove.Add(model);
                }

                foreach (FileModel model in toRemove)
                    storage.Remove(model);
            }
            else
            {
                // Deleting file.
                FileModel model = storage.FirstOrDefault(f => f.Path == e.FullPath);
                if (model != null)
                    storage.Remove(model);
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (String.IsNullOrEmpty(Path.GetExtension(e.OldFullPath)))
            {
                // Renaming directory.
                foreach (FileModel model in storage)
                {
                    if (model.Path.StartsWith(e.OldFullPath))
                        model.Path = model.Path.Replace(e.OldFullPath, e.FullPath);
                }
            }
            else
            {
                // Renaming solution file.
                FileModel model = storage.FirstOrDefault(f => f.Path == e.OldFullPath);
                if (model != null)
                    model.Path = e.FullPath;
            }
        }

        public async Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            HashSet<FileModel> result = new HashSet<FileModel>();

            await Task.Factory.StartNew(() =>
            {
                Func<FileModel, bool> filter = matcherFactory.Create(searchPattern, mode);

                IEnumerable<FileModel> models = Enumerable.Concat(
                    pinStateService.Enumerate().Select(f => new FileModel(f)).OrderBy(f => f.Name),
                    storage.OrderBy(f => f.Name)
                );

                foreach (FileModel model in models.Where(f => filter(f)))
                {
                    if (result.Add(model) && result.Count == count)
                        return;
                }
            });

            if (cancellationToken.IsCancellationRequested)
                return;

            files.Clear();
            foreach (FileModel model in result)
                files.Add(model.Name, model.Path, pinStateService.IsPinned(model.Path));
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            foreach (FileSystemWatcher watcher in watchers)
                watcher.Dispose();
        }
    }
}
