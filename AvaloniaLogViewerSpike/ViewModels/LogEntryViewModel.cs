using Avalonia.Media;
using System;

namespace AvaloniaLogViewerSpike.ViewModels
{
    public class LogEntryViewModel : ViewModelBase
    {
        public DateTime? Timestamp { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }

        public SolidColorBrush Colour => Severity.ToLower() switch
            {
                "error" => new SolidColorBrush(Colors.IndianRed),
                "verbose" => new SolidColorBrush(Colors.Gray),
                "debug" => new SolidColorBrush(Colors.DimGray),
                "info" => new SolidColorBrush(Colors.LawnGreen),
                "warning" => new SolidColorBrush(Colors.Orange),
                "fatal" => new SolidColorBrush(Colors.Red),
                _ => new SolidColorBrush(Colors.White)
            };
    }
}
