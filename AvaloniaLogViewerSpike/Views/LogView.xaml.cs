using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaLogViewerSpike.ViewModels;

namespace AvaloniaLogViewerSpike.Views
{
    public class LogView : ReactiveUserControl<LogViewModel>
    {
        private CompositeDisposable _disposables = new CompositeDisposable();
        private CompositeDisposable _scrollViewerDisposables;

        //private readonly ListBox _listBox;

        public LogView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var control = this.FindControl<ListBox>("LogEntries");
            control.GetObservable(ListBox.ScrollProperty)
                .OfType<ScrollViewer>()
                .Take(1)
                .Subscribe(sv =>
                {
                    _scrollViewerDisposables?.Dispose();
                    _scrollViewerDisposables = new CompositeDisposable();

                    sv
                        .GetObservable(ScrollViewer.VerticalScrollBarMaximumProperty)
                        .Subscribe(max =>
                        {
                            //sv.Offset = sv.Offset.WithY(max);
                        })
                        .DisposeWith(_scrollViewerDisposables);
                })
                .DisposeWith(_disposables);

            //var scrollViewer = this.FindControl<ScrollViewer>("scroller");
            //scrollViewer.GetObservable(ScrollViewer.VerticalScrollBarMaximumProperty)
            //    .Take(1)
            //    .Subscribe(max =>
            //    {
            //        scrollViewer.Offset = scrollViewer.Offset.WithY(max);
            //    });
        }
    }
}
