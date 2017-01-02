﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Services.Positions
{
    public interface IPositionTarget
    {
        double Left { get; set; }
        double Top { get; set; }

        double ActualWidth { get; }
        double ActualHeight { get; }

        WindowStartupLocation WindowStartupLocation { get; set; }
    }
}