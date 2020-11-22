﻿using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public interface INavigator
    {
        void OpenMain();
        void OpenConfiguration();
        void OpenAdditionalApplicationEdit(AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved);
        void OpenAdditionalCommandEdit(AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved);
        void OpenStatistics();
        void Notify(string message);
    }
}
