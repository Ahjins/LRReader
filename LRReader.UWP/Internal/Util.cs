﻿using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace LRReader.UWP.Internal
{
	public static class Util
	{
		public static Version GetAppVersion()
		{
			Package package = Package.Current;
			PackageId packageId = package.Id;
			PackageVersion version = packageId.Version;

			return new Version(version.Major, version.Minor, version.Build, version.Revision);
		}

		public static string GetPackageFamilyName()
		{
			return AppInfo.Current.PackageFamilyName;
		}

		public static async Task<bool> OpenInBrowser(Uri uri)
		{
			return await Launcher.LaunchUriAsync(uri);
		}

	}
}
