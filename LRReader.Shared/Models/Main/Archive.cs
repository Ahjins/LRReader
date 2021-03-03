﻿using LRReader.Shared.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace LRReader.Shared.Models.Main
{
	public class Archive
	{
		public string arcid { get; set; }
		public string isnew { get; set; }
		public string tags { get; set; }
		public string title { get; set; }
		public int pagecount { get; set; }
		public int progress { get; set; }
		[JsonIgnore]
		public string TagsClean { get; set; }
		[JsonIgnore]
		public List<string> TagsList { get; set; } = new List<string>();
		[JsonIgnore]
		public ObservableCollection<ArchiveTagsGroup> TagsGroups { get; set; } = new ObservableCollection<ArchiveTagsGroup>();

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			UpdateTags();
		}

		public bool IsNewArchive()
		{
			if (string.IsNullOrEmpty(isnew))
				return false;
			if (isnew.Equals("none"))
				return false;
			if (isnew.Equals("block"))
				return true;
			return bool.Parse(isnew);
		}

		public void UpdateTags()
		{
			TagsClean = "";
			var separatedTags = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var s in separatedTags)
				TagsClean += s.Substring(Math.Max(s.IndexOf(':') + 1, 0)).Trim() + ", ";
			TagsClean = TagsClean.Trim().TrimEnd(',');
			TagsList.Clear();
			foreach (var s in separatedTags)
				TagsList.Add(s.Trim());

			TagsGroups.Clear();
			var tmp = new List<ArchiveTagsGroup>();
			foreach (var s in separatedTags)
			{
				var parts = s.Trim().Split(new char[] { ':' }, 2);
				ArchiveTagsGroup group = null;
				var @namespace = parts.Length == 2 ? parts[0] : "other";
				group = tmp.FirstOrDefault(tg => tg.Namespace.Equals(@namespace));
				if (group == null)
					group = AddTagsGroup(tmp, @namespace);
				var tag = parts[parts.Length - 1];
				if (parts[0].Equals("date_added"))
					if (long.TryParse(tag, out long unixTime))
						tag = Util.UnixTimeToDateTime(unixTime).ToString();
				group.Tags.Add(new ArchiveTagsGroupTag { FullTag = s.Trim(), Tag = tag });
			}
			tmp.Sort((a, b) => string.Compare(a.Namespace, b.Namespace));
			var c = tmp.Find(g => g.Namespace.Equals("other"));
			if (c != null)
			{
				tmp.Remove(c);
				tmp.Add(c);
			}
			tmp.ForEach(g =>
			{
				g.Namespace = g.Namespace.UpperFirstLetter().Replace('_', ' ');
				TagsGroups.Add(g);
			});
		}

		private ArchiveTagsGroup AddTagsGroup(List<ArchiveTagsGroup> list, string @namespace)
		{
			var group = new ArchiveTagsGroup() { Namespace = @namespace };
			list.Add(group);
			return group;
		}
	}

	public class ArchiveImages
	{
		public List<string> pages { get; set; }
	}

	public class ArchiveImageSet
	{
		public string LeftImage { get; set; }
		public string RightImage { get; set; }
	}

	public class ImagePageSet
	{

		public string Image { get; set; }
		public int Page { get; set; }

		public ImagePageSet(string image, int page)
		{
			this.Image = image;
			this.Page = page;
		}

		public override bool Equals(object obj)
		{
			return obj is ImagePageSet set &&
				   Image.Equals(set.Image);
		}

		public override int GetHashCode()
		{
			return Image.GetHashCode();
		}
	}

	public class ArchiveSearch
	{
		public List<Archive> data { get; set; }
		public int draw { get; set; }
		public int recordsFiltered { get; set; }
		public int recordsTotal { get; set; }
	}

	public class ArchiveTagsGroup
	{
		public string Namespace { get; set; }
		public List<ArchiveTagsGroupTag> Tags { get; set; }

		public ArchiveTagsGroup()
		{
			Tags = new List<ArchiveTagsGroupTag>();
		}
	}

	public class ArchiveTagsGroupTag
	{
		public string FullTag;
		public string Tag;
	}

	public class ArchiveCategories : GenericApiResult
	{
		List<Category> categories { get; set; }
	}
}
