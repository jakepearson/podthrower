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
		SyndicationFeedFormatter GetFeed(int id);

		[OperationContract]
		[WebGet(UriTemplate = "file/{id}/{index}")]
		Message GetFile(int id, int index);

		[OperationContract]
		[WebGet(UriTemplate = "image/{id}")]
		Message GetImage(int id);
	}

	public class NewsFeedService : WebRequestHandler, INewsFeed
	{
		#region Properties
		Document Document
		{
			get;
			set;
		}
		#endregion

		public SyndicationFeedFormatter GetFeed(int id)
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

		public Message GetFile(int id, int index)
		{
			return null;
		}

		public Message GetImage(int id)
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
						Message responseMessage = new RawMessage(responseStream.ToArray());
						responseMessage.Properties[HttpResponseMessageProperty.Name] = responseProperty;
						return responseMessage;
					}
				}
			}
		}

		protected void ProcessRequest(string file, HttpResponseMessageProperty responseProperty, TextWriter responseBody)
		{
			var request = new HttpRequest(file, address.ToString(), "");
			request.RequestType = requestProperty.Method;

			var response = new HttpResponse(responseBody);
			response.ContentType = "text/html";

			var context = new HttpContext(request, response);
			var handler = new FileRequestHandler();
			handler.ProcessRequest(context);

			responseProperty.Headers.Add("Content-Type", response.ContentType);
			responseProperty.StatusCode = (HttpStatusCode)response.StatusCode;
			responseProperty.StatusDescription = response.StatusDescription;
		}

		Feed GetFeedDefinition(int id)
		{
			return Document.Feeds.First(f => f.ID == id);
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

		protected override void ProcessRequest(Uri address, HttpRequestMessageProperty requestProperty, TextReader requestBody, HttpResponseMessageProperty responseProperty, TextWriter responseBody)
		{
			throw new NotImplementedException();
		}
	}
}
