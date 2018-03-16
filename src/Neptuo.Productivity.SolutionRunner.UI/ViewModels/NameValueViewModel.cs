using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    /// <summary>
    /// A view model of name - value pairs.
    /// </summary>
    public class NameValueViewModel
    {
        /// <summary>
        /// Gets a name / user friendly.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">A name / user friendly.</param>
        /// <param name="value">A value.</param>
        public NameValueViewModel(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
