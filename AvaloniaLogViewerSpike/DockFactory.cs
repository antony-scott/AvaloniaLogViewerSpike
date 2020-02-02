using Avalonia.Data;
using AvaloniaLogViewerSpike.ViewModels;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;

namespace AvaloniaLogViewerSpike
{
    public class DockFactory : Factory
    {
        private object _context;

        public DockFactory(object context)
        {
            _context = context;
        }

        public override IDock CreateLayout()
        {
            var document1 = new LogViewModel { Name = "Log1", Id = "Log1", Title = "Log1" };
            var document2 = new LogViewModel { Name = "Log2", Id = "Log2", Title = "Log2" };
            var document3 = new LogViewModel { Id = "AllLogs", Title = "All Logs" };

            var xmainLayout = new ProportionalDock
            {
                Id = "MainLayout",
                Title = "MainLayout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ProportionalDock
                    {
                        Id = "LeftPane",
                        Title = "LeftPane",
                        Proportion = double.NaN,
                        Orientation = Orientation.Vertical,
                        ActiveDockable = null,
                        VisibleDockables = CreateList<IDockable>
                        (
                            new ProportionalDock
                            {
                                Id = "TopLeftPane",
                                Title = "TopLeftPane",
                                Proportion = double.NaN,
                                Orientation = Orientation.Vertical,
                                ActiveDockable = null,
                                VisibleDockables = CreateList<IDockable>
                                (
                                    new DocumentDock
                                    {
                                        Id = "DocumentsPaneTopLeft",
                                        Title = "DocumentsPaneTopLeft",
                                        Proportion = double.NaN,
                                        ActiveDockable = document1,
                                        VisibleDockables = CreateList<IDockable>
                                        (
                                            document1
                                        )
                                    }
                                )
                            },
                            new ProportionalDock
                            {
                                Id = "BottomLeftPane",
                                Title = "BottomLeftPane",
                                Proportion = double.NaN,
                                Orientation = Orientation.Vertical,
                                ActiveDockable = null,
                                VisibleDockables = CreateList<IDockable>
                                (
                                    new DocumentDock
                                    {
                                        Id = "DocumentsPaneBottomLeft",
                                        Title = "DocumentsPaneBottomLeft",
                                        Proportion = double.NaN,
                                        ActiveDockable = document2,
                                        VisibleDockables = CreateList<IDockable>
                                        (
                                            document2
                                        )
                                    }
                                )
                            }
                        )
                    },
                    new DocumentDock
                    {
                        Id = "DocumentsPaneRight",
                        Title = "DocumentsPaneRight",
                        Proportion = double.NaN,
                        ActiveDockable = document3,
                        VisibleDockables = CreateList<IDockable>
                        (
                            document3
                        )
                    }
                )
            };

            var mainLayout = new DocumentDock
            {
                Id = "DocumentsPane",
                Title = "DocumentsPane",
                Proportion = double.NaN,
                ActiveDockable = document1,
                VisibleDockables = CreateList<IDockable>
                (
                    document1,
                    document2,
                    document3
                )
            };

            var mainView = new MainViewModel
            {
                Id = "Main",
                Title = "Main",
                ActiveDockable = mainLayout,
                VisibleDockables = CreateList<IDockable>(mainLayout)
            };

            var root = CreateRootDock();

            root.Id = "Root";
            root.Title = "Root";
            root.ActiveDockable = mainView;
            root.DefaultDockable = mainView;
            root.VisibleDockables = CreateList<IDockable>(mainView);

            return root;
        }

        public override void InitLayout(IDockable layout)
        {
            //this.ContextLocator = new Dictionary<string, Func<object>>
            //{
            //    [nameof(IRootDock)] = () => _context,
            //    [nameof(IPinDock)] = () => _context,
            //    [nameof(IProportionalDock)] = () => _context,
            //    [nameof(IDocumentDock)] = () => _context,
            //    [nameof(IToolDock)] = () => _context,
            //    [nameof(ISplitterDock)] = () => _context,
            //    [nameof(IDockWindow)] = () => _context,
            //    [nameof(IDocument)] = () => _context,
            //    [nameof(ITool)] = () => _context,
            //    ["DocumentsPane"] = () => _context,
            //    ["MainLayout"] = () => _context,
            //    ["Main"] = () => _context,
            //};

            this.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () =>
                {
                    var hostWindow = new HostWindow()
                    {
                        [!HostWindow.TitleProperty] = new Binding("ActiveDockable.Title")
                    };
                    return hostWindow;
                }
            };

            this.DockableLocator = new Dictionary<string, Func<IDockable>>
            {
            };

            base.InitLayout(layout);
        }
    }
}
