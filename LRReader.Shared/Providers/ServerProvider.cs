﻿using LRReader.Shared.Internal;
using LRReader.Shared.Models.Api;
using LRReader.Shared.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LRReader.Shared.Providers
{
	public class ServerProvider
	{
		public async Task<bool> RestartWorker()
		{
			var client = SharedGlobal.LRRApi.GetClient();

			var rq = new RestRequest("api/restart_shinobu");

			var r = await client.ExecuteGetTaskAsync(rq);

			var result = LRRApi.GetResult<GenericApiResult>(r);

			if (!string.IsNullOrEmpty(r.ErrorMessage))
			{
				SharedGlobal.EventManager.ShowError("Network Error", r.ErrorMessage);
				return false;
			}
			switch (r.StatusCode)
			{
				case HttpStatusCode.OK:
					return true;
				case HttpStatusCode.Unauthorized:
					SharedGlobal.EventManager.ShowError("API Error", result.Error.error);
					return false;
			}
			return false;
		}
		public async Task<bool> StopWorker()
		{
			var client = SharedGlobal.LRRApi.GetClient();

			var rq = new RestRequest("api/stop_shinobu");

			var r = await client.ExecuteGetTaskAsync(rq);

			var result = LRRApi.GetResult<GenericApiResult>(r);

			if (!string.IsNullOrEmpty(r.ErrorMessage))
			{
				SharedGlobal.EventManager.ShowError("Network Error", r.ErrorMessage);
				return false;
			}
			switch (r.StatusCode)
			{
				case HttpStatusCode.OK:
					return true;
				case HttpStatusCode.Unauthorized:
					SharedGlobal.EventManager.ShowError("API Error", result.Error.error);
					return false;
			}
			return false;
		}
		public async Task<DownloadPayload> DownloadDB()
		{
			var client = SharedGlobal.LRRApi.GetClient();

			var rq = new RestRequest("api/backup");

			var r = await client.ExecuteGetTaskAsync(rq);

			if (!string.IsNullOrEmpty(r.ErrorMessage))
			{
				SharedGlobal.EventManager.ShowError("Network Error", r.ErrorMessage);
				return null;
			}
			switch (r.StatusCode)
			{
				case HttpStatusCode.OK:
					var download = new DownloadPayload();

					download.Data = r.RawBytes;
					download.Name = "database_backup.json";
					download.Type = ".json";
					return download;
				case HttpStatusCode.Unauthorized:
					SharedGlobal.EventManager.ShowError("API Error", LRRApi.GetError(r).error);
					return null;
			}
			return null;
		}
		public async Task<bool> ClearAllNew()
		{
			var client = SharedGlobal.LRRApi.GetClient();

			var rq = new RestRequest("api/clear_new_all");

			var r = await client.ExecuteGetTaskAsync(rq);

			var result = LRRApi.GetResult<GenericApiResult>(r);

			if (!string.IsNullOrEmpty(r.ErrorMessage))
			{
				SharedGlobal.EventManager.ShowError("Network Error", r.ErrorMessage);
				return false;
			}
			switch (r.StatusCode)
			{
				case HttpStatusCode.OK:
					return true;
				case HttpStatusCode.Unauthorized:
					SharedGlobal.EventManager.ShowError("API Error", result.Error.error);
					return false;
			}
			return false;
		}
		public async Task<ShinobuStatus> GetShinobuStatus()
		{
			var client = SharedGlobal.LRRApi.GetClient();

			var rq = new RestRequest("api/shinobu_status");

			var r = await client.ExecuteGetTaskAsync(rq);

			var result = LRRApi.GetResult<ShinobuStatus>(r);

			if (!string.IsNullOrEmpty(r.ErrorMessage))
			{
				//SharedGlobal.EventManager.ShowError("Network Error", r.ErrorMessage);
				return null;
			}
			switch (r.StatusCode)
			{
				case HttpStatusCode.OK:
					return result.Data;
				case HttpStatusCode.Unauthorized:
					//SharedGlobal.EventManager.ShowError("API Error", result.Error.error);
					return null;
			}
			return null;
		}
	}
}