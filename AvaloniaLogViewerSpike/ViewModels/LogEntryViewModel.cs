using System;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogEntryViewModel : ViewModelBase
    {
        public DateTime? Timestamp { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
    }
}
