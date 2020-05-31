﻿using GalaSoft.MvvmLight.Threading;
using LRReader.Internal;
using LRReader.UWP.ViewModels;
using LRReader.UWP.Views.Items;
using LRReader.UWP.Views.Tabs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LRReader.UWP.Views.Main
{
	public sealed partial class HostTabPage : Page
	{

		private HostTabPageViewModel Data;

		private CoreApplicationView CoreView;
		private ApplicationView AppView;

		public HostTabPage()
		{
			this.InitializeComponent();

			Data = new HostTabPageViewModel();

			CoreView = CoreApplication.GetCurrentView();
			AppView = ApplicationView.GetForCurrentView();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			CoreView.TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
			CoreView.TitleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
			AppView.VisibleBoundsChanged += AppView_VisibleBoundsChanged;

			TabViewStartHeader.Margin = new Thickness(CoreView.TitleBar.SystemOverlayLeftInset, 0, 0, 0);
			TabViewEndHeader.Margin = new Thickness(0, 0, CoreView.TitleBar.SystemOverlayRightInset, 0);

			Window.Current.SetTitleBar(TitleBar);

			Global.EventManager.ShowNotificationEvent += ShowNotification;
			Global.EventManager.AddTabEvent += AddTab;
			Global.EventManager.CloseAllTabsEvent += CloseAllTabs;
			Global.EventManager.CloseTabWithHeaderEvent += CloseTabWithHeader;
			await DispatcherHelper.RunAsync(() =>
			{
				Global.EventManager.AddTab(new ArchivesTab());
				if (Global.SettingsManager.OpenBookmarksTab)
					Global.EventManager.AddTab(new BookmarksTab(), false);
				Global.EventManager.AddTab(new CategoriesTab(), false);
			});
			var info = await Global.UpdatesManager.CheckUpdates(Util.GetAppVersion());
			if (info != null)
				ShowNotification("New update available! - " + info.name, "Check Settings -> About for more info", 0);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);

			CoreView.TitleBar.LayoutMetricsChanged -= TitleBar_LayoutMetricsChanged;
			CoreView.TitleBar.IsVisibleChanged -= TitleBar_IsVisibleChanged;
			AppView.VisibleBoundsChanged -= AppView_VisibleBoundsChanged;

			Window.Current.SetTitleBar(null);
			Global.EventManager.ShowNotificationEvent -= ShowNotification;
			Global.EventManager.AddTabEvent -= AddTab;
			Global.EventManager.CloseAllTabsEvent -= CloseAllTabs;
			Global.EventManager.CloseTabWithHeaderEvent -= CloseTabWithHeader;
		}

		private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar coreTitleBar, object args)
		{
			//TitleBar.Height = coreTitleBar.Height;
			TabViewStartHeader.Margin = new Thickness(coreTitleBar.SystemOverlayLeftInset, 0, 0, 0);
			TabViewEndHeader.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);
		}

		private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
		{
			if (sender.IsVisible)
			{
				TabViewControl.Margin = new Thickness(0, 0, 0, 0);
				TitleBarBackground.Visibility = Visibility.Visible;
			}
			else
			{
				TabViewControl.Margin = new Thickness(0, -40, 0, 0);
				TitleBarBackground.Visibility = Visibility.Collapsed;
			}
		}

		private void ShowNotification(string title, string content, int duration = 5000) => Notifications.Show(new NotificationItem(title, content), duration);

		private void SettingsButton_Click(object sender, RoutedEventArgs e) => Global.EventManager.AddTab(new SettingsTab());

		private void EnterFullScreen_Click(object sender, RoutedEventArgs e) => AppView.TryEnterFullScreenMode();

		private void Bookmarks_Click(object sender, RoutedEventArgs e) => Global.EventManager.AddTab(new BookmarksTab(), true);

		private void Categories_Click(object sender, RoutedEventArgs e) => Global.EventManager.AddTab(new CategoriesTab(), true);

		private void Search_Click(object sender, RoutedEventArgs e) => Global.EventManager.AddTab(new SearchResultsTab(), true);

		private void AppView_VisibleBoundsChanged(ApplicationView sender, object args) => Data.FullScreen = AppView.IsFullScreenMode;

		private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
		{
			if (args.Tab is CustomTab tab)
				tab.Unload();
			TabViewControl.TabItems.Remove(args.Tab);
		}

		public async void AddTab(CustomTab tab, bool switchToTab)
		{
			var current = GetTabFromId(tab.CustomTabId);
			if (current != null)
			{
				if (switchToTab)
					Data.CurrentTab = current;
			}
			else
			{
				TabViewControl.TabItems.Add(tab);
				if (switchToTab)
					await DispatcherHelper.RunAsync(() => Data.CurrentTab = tab);
			}
		}

		public void CloseAllTabs()
		{
			foreach (var t in TabViewControl.TabItems)
			{
				if (t is CustomTab tab)
					tab.Unload();
			}
			TabViewControl.TabItems.Clear();
		}

		public void CloseTabWithHeader(string id)
		{
			var tab = GetTabFromId(id);
			if (tab != null)
			{
				TabViewControl.TabItems.Remove(tab);
			}
		}

		private CustomTab GetTabFromId(string id) => TabViewControl.TabItems.FirstOrDefault(t => (t as CustomTab).CustomTabId.Equals(id)) as CustomTab;

		private void CloseTab_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			args.Handled = true;
			var t = Data.CurrentTab;
			if (!t.IsClosable)
				return;
			if (t is CustomTab tab)
				tab.Unload();
			TabViewControl.TabItems.Remove(t);
		}

	}
}