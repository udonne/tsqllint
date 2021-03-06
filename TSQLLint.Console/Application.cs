using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;

namespace TSQLLint.Console
{
    public class Application
    {
        private readonly string[] _args;
        private readonly IReporter _reporter;
        private readonly ConsoleTimer _timer = new ConsoleTimer();

        public Application(string[] args, IReporter reporter)
        {
            _timer.Start();
            _args = args;
            _reporter = reporter;
        }

        public void Run()
        {
            // parse options
            var commandLineOptions = new CommandLineOptions(_args);

            // perform non-linting actions
            var configHandler = new ConfigHandler.ConfigHandler(commandLineOptions, _reporter);
            if (!configHandler.HandleConfigs())
            {
                return;
            }

            // perform lint
            var lintingHandler = new LintingHandler(commandLineOptions, _reporter);
            lintingHandler.Lint();

            _reporter.ReportResults(_timer.Stop(), lintingHandler.LintedFileCount);
        }
    }
}
