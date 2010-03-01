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
using RawHttp;
using System.Web;
using System.Net;
using System.Windows;

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
		[WebGet(UriTemplate = "file/{id}/{index}")]
		Message GetFile(string id, string index);

		[OperationContract]
		[WebGet(UriTemplate = "image/{id}")]
		Message GetImage(string id);
	}

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

		public Message GetFile(string id, string index)
		{
			return null;
		}

		public Message GetImage(string id)
		{
			var feed = GetFeedDefinition(id);
			var _webEncoder = BindingFactory.CreateEncoder().CreateMessageEncoderFactory().Encoder;

			using (var responseStream = new MemoryStream())
			{
				using (var writer = new StreamWriter(responseStream))
				{
					using (var requestStream = new MemoryStream())
					{
						var responseProperty = new HttpResponseMessageProperty();

						ProcessRequest(feed.Image, responseProperty, writer);

						writer.Flush();
						Message responseMessage = new WebRequestHandler.RawMessage(responseStream.ToArray());
						responseMessage.Properties[HttpResponseMessageProperty.Name] = responseProperty;
						return responseMessage;
					}
				}
			}
		}

		protected void ProcessRequest(string file, HttpResponseMessageProperty responseProperty, TextWriter responseBody)
		{
			var url = OperationContext.Current.RequestContext.RequestMessage.Headers.To;
			var request = new HttpRequest(file, url.ToString(), "");
			request.RequestType = "GET";

			var response = new HttpResponse(responseBody);
			response.ContentType = "image/gif";

			var context = new HttpContext(request, response);
			var handler = new FileRequestHandler();
			handler.ProcessRequest(context);

			responseProperty.Headers.Add("Content-Type", response.ContentType);
			responseProperty.StatusCode = (HttpStatusCode)response.StatusCode;
			responseProperty.StatusDescription = response.StatusDescription;
		}

		Feed GetFeedDefinition(string id)
		{
			var i = int.Parse(id);
			return Document.Feeds.First(f => f.ID == i);
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
