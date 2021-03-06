﻿using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace NewsFactory.UI.Pages
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MainPage : NewsFactory.UI.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();

            DataContext = new MainPageModel();
        }

        private MainPageModel Model
        {
            get { return (MainPageModel)DataContext; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Model.RaiseCommand(Model.InitializeCommand);
        }

        private void OnFeedClick(object sender, ItemClickEventArgs e)
        {
            Model.RaiseCommand(Model.GoToFeedCommand, (NewsFeed)e.ClickedItem);
        }

        private void OnAppBarTapped(object sender, TappedRoutedEventArgs e)
        {
            var appBar = sender as AppBar;
            if (appBar != null)
            {
                appBar.IsOpen = false;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var popupMenu = new PopupMenu();

                popupMenu.Commands.Add(new UICommand("Add one feed by entering RSS feed's url", h =>
                {
                    Model.RaiseCommand(Model.AddFeedCommand);
                    abBottom.IsOpen = false;
                }));
                popupMenu.Commands.Add(new UICommand("Pick one of the popular feeds by category", h =>
                {
                    Model.RaiseCommand(Model.PickFeedsCommand);
                    abBottom.IsOpen = false;
                }));

                var button = (Button)sender;
                var transform = button.TransformToVisual(this);
                var point = transform.TransformPoint(new Point(45, -10));

                await popupMenu.ShowAsync(point);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        private void OnCloseAppBar(object sender, RoutedEventArgs e)
        {
            abBottom.IsOpen = false;
        }

        private async void OnMore(object sender, RoutedEventArgs e)
        {
            try
            {
                var popupMenu = new PopupMenu();

                popupMenu.Commands.Add(new UICommand("Export feeds to OPML file", h =>
                {
                    Model.RaiseCommand(Model.ExportFeedsCommand);
                    abBottom.IsOpen = false;
                }));
                popupMenu.Commands.Add(new UICommandSeparator());
                popupMenu.Commands.Add(new UICommand("Delete feeds", h =>
                {
                    Model.RaiseCommand(Model.SwitchToDeleteFeedsModeCommand);
                    abBottom.IsOpen = false;
                }));
                popupMenu.Commands.Add(new UICommand("Delete all feeds", h =>
                {
                    Model.RaiseCommand(Model.DeleteAllFeedsCommand);
                    abBottom.IsOpen = false;
                }));

                var button = (Button)sender;
                var transform = button.TransformToVisual(this);
                var point = transform.TransformPoint(new Point(45, -10));

                await popupMenu.ShowAsync(point);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        private void itemGridView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems.OfType<NewsFeed>())
                {
                    Model.SelectedFeeds.Add(item);
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems.OfType<NewsFeed>())
                {
                    Model.SelectedFeeds.Remove(item);
                }
            }
        }

        private void ClearSelection()
        {
            if (itemGridView.SelectedItems != null)
            {
                foreach (var item in itemGridView.SelectedItems.ToList())
                {
                    var c = itemGridView.ItemContainerGenerator.ContainerFromItem(item) as GridViewItem;
                    if (c != null)
                    {
                        c.IsSelected = false;
                    }
                }
            }
        }

        private async void OnDeleteFeeds(object sender, RoutedEventArgs e)
        {
            await Model.RaiseCommandAsync(Model.DeleteFeedsCommand);
            ClearSelection();
        }

        private void OnCancelDeleteFeeds(object sender, RoutedEventArgs e)
        {
            Model.RaiseCommand(Model.CancelDeleteFeedsModeCommand);
            ClearSelection();
        }

        private void OnSnappedItemClick(object sender, ItemClickEventArgs e)
        {
            Model.RaiseCommand(Model.GoToFeedCommand, (NewsFeed)e.ClickedItem);
        }
    }
}
