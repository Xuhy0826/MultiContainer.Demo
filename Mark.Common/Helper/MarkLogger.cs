using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Mark.Common.Helper
{
    public class Logger
    {
        private static string _logsDir = string.Empty;
        private static string _serilogOutputTemplate = "{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" + new string('-', 100);
        static Logger()
        {
            _logsDir = System.Environment.CurrentDirectory;
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.File($"{_logsDir}\\logs\\log.log", rollingInterval: RollingInterval.Day, outputTemplate: _serilogOutputTemplate)
                        .CreateLogger();
        }
        /// <summary>
        /// 开发调试过程中重置日志配置
        /// </summary>
        /// <param name="write"></param>
        /// <param name="rollingInterval"></param>
        public static void LogWithDebug(string path = "")
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _logsDir = path;
            }
            Log.CloseAndFlush();
            var logConfig = new LoggerConfiguration().MinimumLevel.Debug();
            logConfig.WriteTo.File($"{_logsDir}\\logs\\log.log", rollingInterval: RollingInterval.Day,
                outputTemplate: _serilogOutputTemplate);
            Log.Logger = logConfig.CreateLogger();
        }

        public static void SetLogsPath(string path = "")
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _logsDir = path;
            }
            Log.CloseAndFlush();
            var logConfig = new LoggerConfiguration().MinimumLevel.Information();
            logConfig.WriteTo.File($"{_logsDir}\\logs\\log.log", rollingInterval: RollingInterval.Day,
                outputTemplate: _serilogOutputTemplate);
            Log.Logger = logConfig.CreateLogger();
        }

        ~Logger()
        {
            Log.CloseAndFlush();
        }

        public static void Info(string msgTpl, params object[] properties)
        {
            Log.Logger.Information(msgTpl, properties);
        }

        public static void Error(Exception ex, string msgTpl, params object[] properties)
        {
            Log.Logger.Error(ex, msgTpl, properties);
        }

        public static void Error(string msgTpl, params object[] properties)
        {
            Log.Logger.Error(msgTpl, properties);
        }

        public static void Fatal(Exception ex, string msgTpl, params object[] properties)
        {
            Log.Logger.Fatal(ex, msgTpl, properties);
        }

        public static void Debug(string msgTpl, params object[] properties)
        {
            Log.Logger.Debug(msgTpl, properties);
        }
    }

    public enum WriteTo
    {
        Console,
        Debug,
        File
    }


    public class EFLoggerProvider : ILoggerProvider
    {
        public void Dispose() { }

        Microsoft.Extensions.Logging.ILogger ILoggerProvider.CreateLogger(string categoryName) => new EFLogger(categoryName);
    }
    public class EFLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string categoryName;

        public EFLogger(string categoryName) => this.categoryName = categoryName;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //ef core执行数据库查询时的categoryName为Microsoft.EntityFrameworkCore.Database.Command,日志级别为Information
            if (logLevel != LogLevel.Debug) return;
            var logContent = formatter(state, exception);
            //TODO: 拿到日志内容想怎么玩就怎么玩吧
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================SQL===================");
            Console.WriteLine(logContent);
            Console.WriteLine("======================================");
            Console.ResetColor();
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
