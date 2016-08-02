using Neptuo.FileSystems;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class FileSystemWatcherSearchService : DisposableBase, IFileSearchService
    {
        private readonly List<FileSystemWatcher> watchers;
        private readonly IPinStateService pinStateService;
        private readonly PatternMatcherFactory matcherFactory = new PatternMatcherFactory();

        private readonly List<FileModel> storage = new List<FileModel>();

        public FileSystemWatcherSearchService(string directoryPath, IPinStateService pinStateService)
        {
            Ensure.Condition.DirectoryExists(directoryPath, "directoryPath");
            Ensure.NotNull(pinStateService, "pinStateService");
            this.watchers = CreateWatcher(directoryPath);
            this.pinStateService = pinStateService;

            storage.AddRange(Directory.GetFiles(directoryPath, "*.sln", SearchOption.AllDirectories).Select(f => new FileModel(f)));
        }

        private List<FileSystemWatcher> CreateWatcher(string directoryPath)
        {
            List<FileSystemWatcher> result = new List<FileSystemWatcher>();

            // Watcher for changes on directory structure.
            FileSystemWatcher directoryWatcher = new FileSystemWatcher(directoryPath);
            directoryWatcher.EnableRaisingEvents = true;
            directoryWatcher.IncludeSubdirectories = true;
            directoryWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            directoryWatcher.Deleted += OnFileDeleted;
            directoryWatcher.Renamed += OnFileRenamed;
            result.Add(directoryWatcher);

            // Watcher for changes on *.sln files.
            FileSystemWatcher fileWatcher = new FileSystemWatcher(directoryPath, "*.sln");
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = NotifyFilters.FileName;
            fileWatcher.Created += OnFileCreated;
            fileWatcher.Deleted += OnFileDeleted;
            fileWatcher.Renamed += OnFileRenamed;
            result.Add(fileWatcher);

            return result;
        }
        
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            storage.Add(new FileModel(e.FullPath));
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

        public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files)
        {
            files.Clear();

            Func<FileModel, bool> filter = matcherFactory.Create(searchPattern, mode);

            foreach (FileModel model in storage.Where(f => filter(f)).Take(count))
                files.Add(model.Name, model.Path, pinStateService.IsPinned(model.Path));

            return Async.CompletedTask;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            foreach (FileSystemWatcher watcher in watchers)
                watcher.Dispose();
        }
    }
}
