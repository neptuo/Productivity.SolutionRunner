using Neptuo.Productivity.SolutionRunner.Views;

namespace Neptuo.Productivity.SolutionRunner
{
    internal interface IAppWindowManager
    {
        MainWindow Main { get; }
        ConfigurationWindow Configuration { get; }
        StatisticsWindow Statistics { get; }
    }
}
