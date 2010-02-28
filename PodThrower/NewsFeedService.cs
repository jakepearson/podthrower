using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Syndication;
using System.IO;
using PodThrower.Model;

namespace PodThrower
{
	// Exposing the service contract    
	[ServiceContract]
	[ServiceKnownType(typeof(Atom10FeedFormatter))]
	[ServiceKnownType(typeof(Rss20FeedFormatter))]
	public interface INewsFeed
	{
		[OperationContract]
		[WebGet(UriTemplate = "GetNews?format={format}&id={id}")]
		SyndicationFeedFormatter GetNews(string format, string id);
	}

	public class NewsFeedService : INewsFeed
	{
		#region Properties
		Document Document
		{
			get;
			set;
		}
		#endregion

		public SyndicationFeedFormatter GetNews(string format, string id)
		{
			var feedDefinition = Document.Feeds.FirstOrDefault(f => f.Title == id);
			if (feedDefinition == null)
			{
				throw new Exception("Unknown feed");
			}

			//Setting up the feed formatter.
			SyndicationFeed feed = new SyndicationFeed(feedDefinition.Title, feedDefinition.Title, new Uri(feedDefinition.URL));
			feed.Authors.Add(new SyndicationPerson("nobody@nobody.com"));
			feed.Categories.Add(new SyndicationCategory("Talk Radio"));
			feed.Description = new TextSyndicationContent(feedDefinition.Title);
			feed.Items = GetItems(feedDefinition);
			feed.ImageUrl = new Uri("http://localhost:86/" + feedDefinition.Image);

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

		IEnumerable<SyndicationItem> GetItems(Feed feedDefinition)
		{
			foreach (var file in GetMP3s(feedDefinition.Folder))
			{
				var shortFile = file.FullName.Replace(feedDefinition.Folder, "").Replace('\\', '/');

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
