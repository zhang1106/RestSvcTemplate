using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace AiDollar.Infrastructure.Logger
{
    public class Log4NetLogger : ILogger
    {
        private readonly string _archivePath;
        private ILog _logger;
        private int _repoCount;

        public Log4NetLogger(string path, string fileName, string archivePath)
        {
            _archivePath = archivePath;
            if (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            GlobalContext.Properties["LogName"] = fileName;
            GlobalContext.Properties["LogPath"] = path;
            var repository = LogManager.CreateRepository($"repo{++_repoCount}");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            _logger = LogManager.GetLogger($"repo{_repoCount}", GetType());
        }

        public void Archive()
        {
            var logFiles = new HashSet<string>();
            foreach (var appender in _logger.Logger.Repository.GetAppenders().OfType<FileAppender>())
            {
                logFiles.Add(appender.File);
            }

            _logger.Logger.Repository.Shutdown();
            try
            {
                foreach (string file in logFiles)
                {
                    string targetDirectory = Path.Combine(_archivePath, $"{DateTime.Today.Date:yyyyMMdd}");
                    if (!Directory.Exists(targetDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }
                        catch
                        {
                        }
                    }

                    File.Move(file,
                        Path.Combine(targetDirectory, Path.GetFileName(file) + $".{DateTime.Now:HHmmssffff}"));
                }
            }
            finally
            {
                var repository = LogManager.CreateRepository($"repo{++_repoCount}");
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                _logger = LogManager.GetLogger($"repo{_repoCount}", GetType());
            }
        }

        public void Log(LogLevel logLevel, string message, Exception exception)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    _logger.Debug(message, exception);
                    break;
                case LogLevel.Information:
                    _logger.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, exception);
                    break;
            }
        }
    }
}
