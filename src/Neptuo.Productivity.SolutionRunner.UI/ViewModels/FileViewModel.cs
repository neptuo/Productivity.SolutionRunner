using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class FileViewModel : ObservableObject, IFile
    {
        private Version version;

        public string Name { get; private set; }
        public string Path { get; private set; }

        public Version Version
        {
            get
            {
                if (version == null)
                    version = TryToReadFileVersion();

                return version;
            }
        }

        private bool isPinned;
        public bool IsPinned
        {
            get { return isPinned; }
            set
            {
                if (isPinned != value)
                {
                    isPinned = value;
                    RaisePropertyChanged();
                    EventManager.RaiseFilePinned(this);
                }
            }
        }

        public ICommand PinCommand { get; private set; }
        public ICommand UnPinCommand { get; private set; }

        public FileViewModel(string name, string path, bool isPinned = false)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNullOrEmpty(path, "path");
            Name = name;
            Path = path;
            IsPinned = isPinned;

            PinCommand = new PinCommand(this);
            UnPinCommand = new UnPinCommand(this);
        }

        public override bool Equals(object obj)
        {
            FileViewModel other = obj as FileViewModel;
            if (other == null)
                return false;

            return Path.Equals(other.Path);
        }

        public override int GetHashCode()
        {
            return 151 ^ Path.GetHashCode();
        }


        private const string compatibleVersionLinePrefix = "Format Version ";
        public const string newVersionLinePrefix = "VisualStudioVersion = ";

        private Version TryToReadFileVersion()
        {
            if (File.Exists(Path))
            {
                using (var fileReader = new StreamReader(Path))
                {
                    fileReader.ReadLine();
                    string compatibleRawVersion = fileReader.ReadLine();
                    if (compatibleRawVersion == null)
                        return null;

                    fileReader.ReadLine();
                    string newRawVersion = fileReader.ReadLine();
                    if (newRawVersion == null)
                        return null;

                    int indexOfVersion = newRawVersion.IndexOf(newVersionLinePrefix);
                    if (indexOfVersion >= 0)
                    {
                        // We have newer sln file.
                        indexOfVersion += newVersionLinePrefix.Length;
                        string rawVersion = newRawVersion.Substring(indexOfVersion);
                        return new Version(rawVersion);
                    }

                    // We have some quite old sln file.
                    indexOfVersion = compatibleRawVersion.IndexOf(compatibleVersionLinePrefix);
                    if (indexOfVersion >= 0)
                    {
                        indexOfVersion += compatibleVersionLinePrefix.Length;
                        string rawVersion = compatibleRawVersion.Substring(indexOfVersion);
                        Version formatVersion = new Version(rawVersion);
                        if (formatVersion.Major == 12)
                            return new Version(14, 0);
                        else if (formatVersion.Major == 11)
                            return new Version(12, 0);
                        else if (formatVersion.Major == 10)
                            return new Version(10, 0);
                    }
                }
            }

            return null;
        }
    }
}
