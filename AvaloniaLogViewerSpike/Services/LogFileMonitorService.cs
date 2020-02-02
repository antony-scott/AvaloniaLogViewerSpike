using AvaloniaLogViewerSpike.Messages;
using AvaloniaLogViewerSpike.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AvaloniaLogViewerSpike.Services
{
    public interface ILogFileMonitorService
    {
        void Clear();
        bool IsFileBeingMonitored(string filename);
        Guid AddMonitor(string name, string filename);
        void GetPreviousLogs(int numberOfLines);
    }

    public class LogFileMonitorService : ILogFileMonitorService
    {
        private readonly List<LogFileMonitor> _monitors = new List<LogFileMonitor>();

        public LogFileMonitorService()
        {
        }

        public void Clear()
        {
            _monitors.Clear();
        }

        public bool IsFileBeingMonitored(string filename)
        {
            return _monitors.Any(x => x.LogFilename == filename);
        }

        public Guid AddMonitor(string name, string filename)
        {
            var identifier = Guid.NewGuid();
            var monitor = LogFileMonitor.Create(name, identifier, filename, WatcherOnChangedOrCreated);
            _monitors.Add(monitor);
            return identifier;
        }

        public void GetPreviousLogs(int numberOfLines)
        {
            _monitors.ForEach(monitor => PublishMessage(monitor, lastNEntries: numberOfLines));
        }

        private void WatcherOnChangedOrCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            var watcher = sender as FileSystemWatcher;
            var monitor = _monitors.FirstOrDefault(x => x.Watcher == watcher);
            if (monitor == null) return;

            PublishMessage(monitor, filename: fileSystemEventArgs.FullPath);
        }

        private void PublishMessage(LogFileMonitor monitor, string filename = null, int? lastNEntries = null)
        {
            var logEntries = monitor.GetNewLogEntries(logfilename: filename, lastNEntries: lastNEntries);

            MessageBus
                .Current
                .SendMessage(new LogEntriesMessage(
                    monitor.Name,
                    monitor.Identifier,
                    logEntries));
        }
    }

    internal class LogFileMonitor : IDisposable
    {
        private DateTime _lastWriteTime;
        private long? _previousSize;

        public string Name { get; private set; }
        public Guid Identifier { get; private set; }
        public string LogFilename { get; private set; }
        public bool IsMonitoring { get; set; }
        public FileSystemWatcher Watcher { get; private set; }

        private LogFileMonitor() { }

        public static LogFileMonitor Create(string name, Guid identifier, string logFilename, Action<object, FileSystemEventArgs> changedOrCreatedAction)
        {
            var folder = Path.GetDirectoryName(logFilename);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var monitor = new LogFileMonitor
            {
                Name = name,
                Identifier = identifier,
                LogFilename = logFilename,
                Watcher = new FileSystemWatcher
                {
                    Path = folder,
                    Filter = "*.*",
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.LastWrite
                },
                IsMonitoring = true
            };

            monitor.Watcher.Created += (sender, args) => changedOrCreatedAction(sender, args);
            monitor.Watcher.Changed += (sender, args) => changedOrCreatedAction(sender, args);

            return monitor;
        }

        public void Dispose()
        {
            Watcher?.Dispose();
        }

        private static FileInfo GetLogFile(string logFilename)
        {
            string folder;
            string searchPattern;

            if (Directory.Exists(logFilename))
            {
                folder = logFilename;
                searchPattern = "*.*";
            }
            else
            {
                folder = Path.GetDirectoryName(logFilename);
                searchPattern = Path.GetFileNameWithoutExtension(logFilename) + "*" + Path.GetExtension(logFilename);
            }

            if (string.IsNullOrWhiteSpace(folder)) return null;

            var logFolder = folder;

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var logfile =
                Directory.GetFiles(logFolder, searchPattern)
                    .Select(filename => new FileInfo(filename))
                    .OrderByDescending(x => x.LastWriteTime)
                    .FirstOrDefault();

            return logfile;
        }

        public bool HasNewLogData(FileInfo logFile)
        {
            if (logFile == null)
            {
                return true;
            }

            if (logFile.FullName != LogFilename)
            {
                return true;
            }

            if (logFile.LastWriteTime != _lastWriteTime)
            {
                return true;
            }

            if (logFile.Length != _previousSize)
            {
                return true;
            }

            return false;
        }

        public IEnumerable<LogEntryViewModel> GetNewLogEntries(string logfilename = null, int? lastNEntries = null)
        {
            var logfile = GetLogFile(logfilename ?? LogFilename);
            if (logfile == null) return null;
            if (!HasNewLogData(logfile)) return null;

            // get the new log file content
            using (var fs = File.Open(logfile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                if (logfile.Name == Path.GetFileName(LogFilename))
                {
                    fs.Seek(_previousSize ?? 0, SeekOrigin.Begin);
                }

                var text = sr.ReadToEnd();

                var entries = Parse(text, lastNEntries);

                _lastWriteTime = logfile.LastWriteTime;
                _previousSize = logfile.Length;
                LogFilename = logfile.FullName;

                return entries;
            }
        }

        private IEnumerable<LogEntryViewModel> Parse(string text, int? lastNEntries = null)
        {
            var lines = Regex
                .Split(text, "\u001e")
                .Where(line => line.Length > 0)
                .ToList();
            if (lastNEntries.HasValue)
            {
                lines.RemoveRange(0, Math.Max(0, lines.Count - lastNEntries.Value));
            }
            var entries = lines
                .Where(line => line.Length > 0)
                .Select(line =>
                {
                    var parts = line.Split('\t');

                    if (parts.Length == 1)
                    {
                        return new LogEntryViewModel
                        {
                            Message = parts[0]
                        };
                    }

                    var strThreadId = Regex.Split(parts[1], @"\D").First();
                    var timeStamp = DateTime.TryParse(parts[0], null, DateTimeStyles.None, out var ts) ? ts : (DateTime?)null;
                    var threadId = int.Parse(strThreadId);
                    var severity = parts[2];

                    return new LogEntryViewModel
                    {
                        Severity = severity,
                        Message = string.Join("\t", parts.Skip(3)),
                        Timestamp = timeStamp
                    };
                });

            return entries;
        }
    }
}
