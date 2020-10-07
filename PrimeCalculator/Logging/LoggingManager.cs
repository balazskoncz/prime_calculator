using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;

namespace PrimeCalculator.Logging
{
    public class LoggingManager : ILoggingManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggingManager));

        public LoggingManager()
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();

                using (var fs = File.OpenRead("log4net.config"))
                {
                    log4netConfig.Load(fs);

                    var repo = LogManager.CreateRepository(
                            Assembly.GetEntryAssembly(),
                            typeof(log4net.Repository.Hierarchy.Hierarchy));

                    XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                    // The first log to be written 
                    _logger.Info("Log System Initialized");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }

        public void LogError(string error)
        {
            _logger.Error(error);
        }

        public void LogInformation(string message)
        {
            _logger.Info(message);
        }

        public void LogFatal(string message)
        {
            _logger.Fatal(message);
        }
    }
}
