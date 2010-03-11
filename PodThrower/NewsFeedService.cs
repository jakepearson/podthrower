using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Syndication;
using System.IO;
using PodThrower.Model;
using System.ServiceModel.Channels;
using System.Web;
using System.Net;
using System.Windows;
using System.Drawing.Imaging;
using System.Drawing;
using System.ServiceModel.Activation;

namespace PodThrower
{
	// Exposing the service contract    
	[ServiceContract]
	[ServiceKnownType(typeof(Atom10FeedFormatter))]
	[ServiceKnownType(typeof(Rss20FeedFormatter))]
	public interface INewsFeed
	{
		[OperationContract]
		[WebGet(UriTemplate = "feed/{id}")]
		SyndicationFeedFormatter GetFeed(string id);

		[OperationContract]
		[WebGet(UriTemplate = "file/{id}/{index}/*")]
		Stream GetFile(string id, string index);

		[OperationContract]
		[WebInvoke(Method="GET", UriTemplate = "image/{id}")]
		Stream GetImage(string id);
	}

	[ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
	public class NewsFeedService : INewsFeed
	{
		#region Properties
		Document Document
		{
			get { return (Document)Application.Current.Windows[0].DataContext; }
		}
		#endregion

		public SyndicationFeedFormatter GetFeed(string id)
		{
			var feedDefinition = GetFeedDefinition(id);

			SyndicationFeed feed = new SyndicationFeed(feedDefinition.Title, feedDefinition.Title, new Uri(feedDefinition.URL));
			feed.Authors.Add(new SyndicationPerson("nobody@nobody.com"));
			feed.Categories.Add(new SyndicationCategory("Talk Radio"));
			feed.Description = new TextSyndicationContent(feedDefinition.Title);
			feed.Items = GetItems(feedDefinition);
			feed.ImageUrl = new Uri(Constants.RootURL + "image/" + id);

			return new Rss20FeedFormatter(feed, false);
		}

		public Stream GetFile(string id, string index)
		{
			var count = int.Parse(index);
			var feed = GetFeedDefinition(id);
			WebOperationContext.Current.OutgoingResponse.ContentType = Constants.MP3Mime;
			var file = feed.Files.Skip(count).First();
			return new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
		}

		public Stream GetImage(string id)
		{
			var feed = GetFeedDefinition(id);

			WebOperationContext.Current.OutgoingResponse.ContentType = Constants.ImageMIME;
			var imageAsMemoryStream = GetImageAsMemoryStream(feed.Image);
			return imageAsMemoryStream;
		}

	    MemoryStream GetImageAsMemoryStream(string imagePath)  
		{  
			var imageAsMemoryStream = new MemoryStream();  
			var image = new Bitmap(imagePath);  
			image.Save(imageAsMemoryStream, ImageFormat.Jpeg);  
			imageAsMemoryStream.Position = 0;  
			return imageAsMemoryStream;  
		} 

		Feed GetFeedDefinition(string id)
		{
			var i = int.Parse(id);
			return Document.Feeds.First(f => f.ID == i);
		}

		IEnumerable<SyndicationItem> GetItems(Feed feedDefinition)
		{
			var count = 0;
			foreach (var file in feedDefinition.Files)
			{
				var shortFile = file.FullName.Replace(feedDefinition.Folder, "").Replace('\\', '/');

				var item = new SyndicationItem();
				var title = file.Name.Replace(".mp3", "");
				item.Title = new TextSyndicationContent(title);
				item.PublishDate = file.CreationTime;
				item.Id = title;
				item.Summary = new TextSyndicationContent(title);
				item.Categories.Add(new SyndicationCategory("Talk Radio"));

				var uri = new Uri(Constants.RootURL + "file/" + feedDefinition.ID + "/" + count + "/" + file.Name);
				var length = file.Length;
				var type = Constants.MP3Mime;

				item.Links.Add(SyndicationLink.CreateMediaEnclosureLink(uri, type, length));
				count++;
				yield return item;
			}
		}
	}
}
