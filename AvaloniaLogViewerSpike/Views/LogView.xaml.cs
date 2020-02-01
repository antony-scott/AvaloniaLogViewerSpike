using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using AvaloniaLogViewerSpike.ViewModels;
using ReactiveUI;

namespace AvaloniaLogViewerSpike.Views
{
    public class LogView : ReactiveUserControl<LogViewModel>
    {
        private readonly ListBox _listbox;

        public LogView()
        {
            AvaloniaXamlLoader.Load(this);

            _listbox = this.FindControl<ListBox>("LogEntries");
            this.WhenActivated(disposables =>
            {
                var itemsChanged = _listbox
                    .WhenAnyValue(lb => lb.ItemCount)
                    .Where(x => x > 0)
                    .Throttle(TimeSpan.FromMilliseconds(10))
                    .Select(x => x - 1)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(index =>
                    {
                        var item = ViewModel.LogEntries.ElementAt(index);
                        if (item != null)
                        {
                            _listbox.ScrollIntoView(item);
                        }
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
