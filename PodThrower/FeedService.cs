using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.ServiceModel.Samples.Syndication;
using System.IO;

namespace PodThrower
{
	[ServiceContract]
	public class FeedService
	{
		const string ItemNs = "http://localhost/sterncatcher";
		const string Path = @"C:\Users\Jakers\Downloads\alt.binaries.howard-stern\";

		//Returns the List Podcasts
		[OperationContract]
		ContentFeed<FeedItem> GetItemList()
		{
			//instantiate the feed
			ContentFeed<FeedItem> feed = new ContentFeed<FeedItem>();
			//populate the list of feed items
			feed.FeedItems = GenerateItemList();
			//set some feed-level properties
			feed.FeedAuthor = "pearsonjj@yahoo.com";
			feed.FeedTitle = "Stern Feed";
			feed.FeedDescription = "Stern Feed";
			feed.FeedId = ItemNs;
			feed.FeedWebPageLink = ItemNs;
			//return the feed
			return feed;
		}

		ItemList<FeedItem> GenerateItemList()
		{
			ItemList<FeedItem> list = new ItemList<FeedItem>();

			foreach (var file in GetMP3s(Path))
			{
				FeedItem item = new FeedItem();
				item.ItemTitle = file.Name;
				//treat the content of this item as HTML and do not escape it
				item.ItemContent = new ItemContent("<div xmlns='http://www.w3.org/1999/xhtml'>...</div>", true);
				item.ItemPublishDate = file.LastWriteTime;
				item.ItemLastUpdated = file.LastWriteTime;
				item.ItemCategories.Add(new FeedCategory("http://contoso.com/categories/", "Everyone"));
				item.ItemExtensions.Add(new ObjectExtension("Status", "InProgress", ItemNs));
				item.ItemId = "http://contoso.com/feed/items/7333830f-a644-430f-8476-ce697d27d38e";
				list.Add(item);
			}
			return list;
		}

		IEnumerable<FileInfo> GetMP3s(string path)
		{
			foreach (var file in Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories))
			{
				yield return new FileInfo(file);
			}
		}
	}
}
