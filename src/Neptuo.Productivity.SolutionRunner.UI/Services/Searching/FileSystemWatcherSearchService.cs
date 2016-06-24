﻿using Neptuo.FileSystems;
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
        private readonly FileSystemWatcher watcher;
        private readonly IPinStateService pinStateService;
        private readonly PatternMatcherFactory matcherFactory = new PatternMatcherFactory();

        private readonly List<FileModel> storage = new List<FileModel>();

        public FileSystemWatcherSearchService(string directoryPath, IPinStateService pinStateService)
        {
            Ensure.Condition.DirectoryExists(directoryPath, "directoryPath");
            Ensure.NotNull(pinStateService, "pinStateService");
            this.watcher = CreateWatcher(directoryPath);
            this.pinStateService = pinStateService;

            storage.AddRange(Directory.GetFiles(directoryPath, "*.sln", SearchOption.AllDirectories).Select(f => new FileModel(f)));
        }

        private FileSystemWatcher CreateWatcher(string directoryPath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(directoryPath, "*.sln");
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += OnFileCreated;
            watcher.Deleted += OnFileDeleted;
            watcher.Renamed += OnFileRenamed;
            //watcher.Error += OnWatcherError;
            return watcher;
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            throw e.GetException();
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            storage.Add(new FileModel(e.FullPath));
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            FileModel model = storage.FirstOrDefault(f => f.Path == e.FullPath);
            if (model != null)
                storage.Remove(model);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            FileModel model = storage.FirstOrDefault(f => f.Path == e.OldFullPath);
            if (model != null)
                model.Path = e.FullPath;
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
            watcher.Dispose();
        }
    }
}
