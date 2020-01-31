using AvaloniaLogViewerSpike.Models;
using System;
using System.Collections.Generic;

namespace AvaloniaLogViewerSpike.Messages
{
    public class LogEntriesMessage
    {
        public Guid Identifier { get; }
        public IEnumerable<LogEntryModel> LogEntries { get; }

        public LogEntriesMessage(Guid identifier, IEnumerable<LogEntryModel> logEntries)
        {
            Identifier = identifier;
            LogEntries = logEntries;
        }
    }
}
