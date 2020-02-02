using AvaloniaLogViewerSpike.ViewModels;
using System;
using System.Collections.Generic;

namespace AvaloniaLogViewerSpike.Messages
{
    public class LogEntriesMessage
    {
        public string Name { get; }
        public Guid Identifier { get; }
        public IEnumerable<LogEntryViewModel> LogEntries { get; }

        public LogEntriesMessage(string name, Guid identifier, IEnumerable<LogEntryViewModel> logEntries)
        {
            Name = name;
            Identifier = identifier;
            LogEntries = logEntries;
        }
    }
}
