﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LRReader.Shared.Internal
{
	public interface ISettingsStorage
	{
		void StoreObjectLocal(string key, object obj);

		void StoreObjectRoamed(string key, object obj);

		T GetObjectLocal<T>(string key);

		T GetObjectLocal<T>(string key, T def);

		T GetObjectRoamed<T>(string key);

		T GetObjectRoamed<T>(string key, T def);
	}

	public class StubSettingsStorage : ISettingsStorage
	{
		public T GetObjectLocal<T>(string key) => GetObjectLocal<T>(key, default);

		public T GetObjectLocal<T>(string key, T def) => def;

		public T GetObjectRoamed<T>(string key) => GetObjectRoamed<T>(key, default);

		public T GetObjectRoamed<T>(string key, T def) => def;

		public void StoreObjectLocal(string key, object obj)
		{
		}

		public void StoreObjectRoamed(string key, object obj)
		{
		}
	}
}
