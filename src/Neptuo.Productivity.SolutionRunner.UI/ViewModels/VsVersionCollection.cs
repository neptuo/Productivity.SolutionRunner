﻿using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class VsVersionCollection : ObservableCollection<Version>, IApplicationCollection
    {
        public IApplicationCollection Add(string name, string path, string arguments, ImageSource icon, bool isMain)
        {
            return this;
        }

        public IApplicationCollection Add(string name, Version version, string path, string arguments, ImageSource icon, bool isMain)
        {
            if (isMain)
                Add(version);

            return this;
        }
    }
}