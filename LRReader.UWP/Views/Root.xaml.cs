﻿using LRReader.Shared.Services;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LRReader.UWP.Views
{
	public sealed partial class Root : Page
	{
		public Root()
		{
			this.InitializeComponent();
			if (!ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 13))
			{
				Background = (AcrylicBrush)Resources["MicaFallbackBrush"];
			}
		}

		public void ChangeTheme(AppTheme theme)
		{
			switch (theme)
			{
				case AppTheme.System:
					RequestedTheme = ElementTheme.Default;
					break;
				case AppTheme.Dark:
					RequestedTheme = ElementTheme.Dark;
					break;
				case AppTheme.Light:
					RequestedTheme = ElementTheme.Light;
					break;
			}
		}

		public void UpdateThemeColors()
		{
			var AppView = ApplicationView.GetForCurrentView();
			var titleBar = AppView.TitleBar;
			switch (ActualTheme)
			{
				case ElementTheme.Light:
					titleBar.ButtonForegroundColor = Colors.Black;
					break;
				case ElementTheme.Dark:
					titleBar.ButtonForegroundColor = Colors.White;
					break;
			}
		}
	}
}
