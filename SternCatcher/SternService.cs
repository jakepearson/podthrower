using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Syndication;
using System.IO;

namespace SternCatcher
{
	// Exposing the service contract    
	[ServiceContract]
	[ServiceKnownType(typeof(Atom10FeedFormatter))]
	[ServiceKnownType(typeof(Rss20FeedFormatter))]
	public interface INewsFeed
	{
		[OperationContract]
		[WebGet(UriTemplate = "GetNews?format={format}")]
		SyndicationFeedFormatter GetNews(string format);
	}

	public class NewsFeedService : INewsFeed
	{
		const string Path = @"C:\Users\Jake\Downloads\alt.binaries.howard-stern\";

		public SyndicationFeedFormatter GetNews(string format)
		{
			//Setting up the feed formatter.
			SyndicationFeed feed = new SyndicationFeed("Howard Stern", "The Howard Stern Show", new Uri("http://howardstern.com"));
			feed.Authors.Add(new SyndicationPerson("dude@gmail.com"));
			feed.Categories.Add(new SyndicationCategory("Talk Radio"));
			feed.Description = new TextSyndicationContent("The Howard Stern Show");
			feed.Items = GetItems();
			feed.ImageUrl = new Uri("http://localhost:86/sternfirst.gif");

			// Processing and serving the feed according to the required format
			// i.e. either RSS or Atom.
			SyndicationFeedFormatter result = null;
			if (format == "atom")
			{
				result = new Atom10FeedFormatter(feed);
			}
			else
			{
				result = new Rss20FeedFormatter(feed, false);
			}
			return result;
		}

		IEnumerable<SyndicationItem> GetItems()
		{
			foreach (var file in GetMP3s(Path))
			{
				var shortFile = file.FullName.Replace(Path, "").Replace('\\', '/');

				var item = new SyndicationItem();
				var title = file.Name.Replace(".mp3", "");
				item.Title = new TextSyndicationContent(title);
				item.PublishDate = file.CreationTime;
				item.Id = title;
				item.Summary = new TextSyndicationContent(title);
				item.Categories.Add(new SyndicationCategory("Talk Radio"));

				var uri = new Uri("http://localhost:86/" + shortFile);
				var length = file.Length;
				var type = "audio/mpeg";

				item.Links.Add(SyndicationLink.CreateMediaEnclosureLink(uri, type, length));
				yield return item;
			}
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
