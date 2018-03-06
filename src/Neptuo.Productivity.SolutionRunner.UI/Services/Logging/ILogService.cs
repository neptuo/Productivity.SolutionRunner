﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public interface ILogService
    {
        IEnumerable<string> GetFileNames();

        string FindFileContent(string fileName);
    }
}
