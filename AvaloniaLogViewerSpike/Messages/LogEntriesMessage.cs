using AvaloniaLogViewerSpike.Models;
using System;
using System.Collections.Generic;

namespace AvaloniaLogViewerSpike.Messages
{
    public class LogEntriesMessage
    {
        public string Name { get; }
        public Guid Identifier { get; }
        public IEnumerable<LogEntryModel> LogEntries { get; }

        public LogEntriesMessage(string name, Guid identifier, IEnumerable<LogEntryModel> logEntries)
        {
            Name = name;
            Identifier = identifier;
            LogEntries = logEntries;
        }
    }
}
