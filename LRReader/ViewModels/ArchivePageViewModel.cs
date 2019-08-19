﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using LRReader.Internal;
using LRReader.Models.Api;
using LRReader.Models.Main;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Net;

namespace LRReader.ViewModels
{
	public class ArchivePageViewModel : ViewModelBase
	{
		private bool _isLoading = false;
		public bool IsLoading
		{
			get
			{
				return _isLoading;
			}
			set
			{
				_isLoading = value;
				RaisePropertyChanged("IsLoading");
			}
		}
		private bool _loadingImages = false;
		public bool LoadingImages
		{
			get => _loadingImages;
			set
			{
				_loadingImages = value;
				RaisePropertyChanged("LoadingImages");
			}
		}
		private bool _refreshOnErrorButton = false;
		public bool RefreshOnErrorButton
		{
			get => _refreshOnErrorButton;
			set
			{
				_refreshOnErrorButton = value;
				RaisePropertyChanged("RefreshOnErrorButton");
			}
		}
		private ObservableCollection<String> _archiveImages = new ObservableCollection<String>();
		public ObservableCollection<string> ArchiveImages
		{
			get => _archiveImages;
		}
		private Archive _archive = new Archive();
		public Archive Archive
		{
			get => _archive;
			set
			{
				if (!_archive.Equals(value))
				{
					_archive = value;
					RaisePropertyChanged("Archive");
				}
			}
		}
		private ObservableCollection<string> _tags = new ObservableCollection<string>();
		public ObservableCollection<string> Tags
		{
			get => _tags;
		}

		public void LoadTags()
		{
			Tags.Clear();

			foreach (var s in Archive.tags.Split(", "))
			{
				Tags.Add(s);
			}
		}

		public async Task LoadImages()
		{
			LoadingImages = true;
			RefreshOnErrorButton = false;
			ArchiveImages.Clear();

			var client = Global.LRRApi.GetClient();

			var rq = new RestRequest("api/extract");

			rq.AddParameter("id", Archive.arcid);

			var r = await client.ExecuteGetTaskAsync(rq);

			var result = LRRApi.GetResult<ArchiveImages>(r);

			LoadingImages = false;
			if (!r.IsSuccessful)
			{
				RefreshOnErrorButton = true;
				Global.EventManager.ShowError("Network Error", r.ErrorMessage);
				return;
			}
			switch (r.StatusCode)
			{
				case HttpStatusCode.OK:
					await Task.Run(async () =>
					{
						foreach (var s in result.Data.pages)
						{
							await DispatcherHelper.RunAsync(() => ArchiveImages.Add(s));
						}
					});
					break;
				case HttpStatusCode.Unauthorized:
					RefreshOnErrorButton = true;
					Global.EventManager.ShowError("API Error", result.Error.error);
					break;
			}
		}
	}
}
