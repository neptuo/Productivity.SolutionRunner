using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class AdditionalApplicationModel : IEquatable<AdditionalApplicationModel>
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Arguments { get; private set; }
        public bool IsAdministratorRequired { get; private set; }
        public bool IsApplicationWindowShown { get; private set; } = true;
        public Key HotKey { get; private set; }
        public IReadOnlyList<AdditionalApplicationModel> Commands { get; private set; }

        public AdditionalApplicationModel(string name, string path, string arguments, bool isAdministratorRequired, bool isApplicationWindowShown, Key hotKey)
            : this(name, path, arguments, isAdministratorRequired, isApplicationWindowShown, hotKey, new List<AdditionalApplicationModel>())
        { }

        public AdditionalApplicationModel(string name, string path, string arguments, bool isAdministratorRequired, bool isApplicationWindowShown, Key hotKey, IReadOnlyList<AdditionalApplicationModel> commands)
        {
            Name = name;
            Path = path;
            Arguments = arguments;
            IsAdministratorRequired = isAdministratorRequired;
            IsApplicationWindowShown = isApplicationWindowShown;
            HotKey = hotKey;
            Commands = commands;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash += 13 * Name.GetHashCode();
                hash += 13 * Path.GetHashCode();
                hash += 13 * Arguments.GetHashCode();
                hash += 13 * IsAdministratorRequired.GetHashCode();
                hash += 13 * IsApplicationWindowShown.GetHashCode();
                hash += 13 * HotKey.GetHashCode();

                if (Commands != null)
                    hash += 13 * Commands.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            AdditionalApplicationModel other = obj as AdditionalApplicationModel;
            return Equals(other);
        }

        public bool Equals(AdditionalApplicationModel other)
        {
            if (other == null)
                return false;

            if (other.Name != Name)
                return false;

            if (other.Path != Path)
                return false;

            if (other.Arguments != Arguments)
                return false;

            if (other.IsAdministratorRequired != IsAdministratorRequired)
                return false;

            if (other.IsApplicationWindowShown != IsApplicationWindowShown)
                return false;

            if (other.HotKey != HotKey)
                return false;

            if (other.Commands != null)
            {
                if (Commands != null)
                {
                    if (other.Commands.Count == Commands.Count)
                    {
                        for (int i = 0; i < other.Commands.Count; i++)
                        {
                            AdditionalApplicationModel otherCommand = other.Commands[i];
                            AdditionalApplicationModel command = Commands[i];
                            if (!command.Equals(otherCommand))
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Commands != null)
                    return false;
            }

            return true;
        }
    }
}
