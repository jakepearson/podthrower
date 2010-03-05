using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PodThrower.View;
using System.Windows;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Xml.Serialization;
using System.Windows.Shapes;
using System.IO;

namespace PodThrower.Model
{
	public class Document : BaseModel
	{
		#region Member Variables
		int port = 4242;
		bool connected;
		ObservableCollection<Feed> feeds;
		WebServiceHost host;
		Visibility visibility = Visibility.Hidden;
		static XmlSerializer serializer;

		ICommand editCommand;
		ICommand connectCommand;
		ICommand addCommand;
		ICommand closeCommand;
		#endregion

		#region Properties
		[XmlIgnore]
		public int Port
		{
			get { return port; }
			set
			{
				if (port != value)
				{
					port = value;
					LaunchChanged("Port");
				}
			}
		}

		[XmlIgnore]
		public bool Connected
		{
			get { return connected; }
			set
			{
				if (connected != value)
				{
					connected = value;
					LaunchChanged("Connected");
					LaunchChanged("Message");
				}
			}
		}

		public string Message
		{
			get { return "Pod Thrower: " + Environment.NewLine + (Connected ? "Connected" : "Not Connected"); }
		}

		[XmlArray]
		public ObservableCollection<Feed> Feeds
		{
			get { return feeds ?? (feeds = new ObservableCollection<Feed>()); }
			set { feeds = value; }
		}

		[XmlIgnore]
		public Visibility Visibility
		{
			get { return Visibility.Hidden; }
			private set
			{
				if (visibility != value)
				{
					visibility = value;
					LaunchChanged("Visibility");
				}
			}
		}

		Window Window
		{
			get;
			set;
		}

		XmlSerializer Serializer
		{
			get { return serializer ?? (serializer = new XmlSerializer(typeof(ObservableCollection<Feed>))); }
		}

		public string DataFile
		{
			get
			{
				var userPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
				var userDataPath = userPath + System.IO.Path.DirectorySeparatorChar + "PodThrower";
				if (!Directory.Exists(userDataPath))
				{
					Directory.CreateDirectory(userDataPath);
				}
				return userDataPath + System.IO.Path.DirectorySeparatorChar + "FeedDefinitions.xml";
			}
		}
		#endregion

		#region Commands
		public ICommand EditCommand
		{
			get { return editCommand ?? (editCommand = new RelayCommand(p => ShowEditor())); }
		}

		public ICommand ConnectCommand
		{
			get { return connectCommand ?? (connectCommand = new RelayCommand(p => Open())); }
		}

		public ICommand AddCommand
		{
			get { return addCommand ?? (addCommand = new RelayCommand(p => Add())); }
		}

		public ICommand CloseCommand
		{
			get { return closeCommand ?? (closeCommand = new RelayCommand(p => Quit())); }
		}
		#endregion

		public Document(Window window)
		{
			Window = window;
			Load();
			Open();
		}

		void ShowEditor()
		{
			Window.Visibility = Visibility.Visible;
		}

		public void Open()
		{
			Close();

			Uri address = new Uri("http://localhost:" + Port + "/podthrower/");
			host = new WebServiceHost(typeof(NewsFeedService), address);

			try
			{
				host.Open();
				Connected = true;
			}
			catch (CommunicationException ce)
			{
				MessageBox.Show(ce.ToString());
				host.Abort();
				host = null;
			}
		}

		public void Close()
		{
			if (host != null && Connected)
			{
				host.Close();
				host = null;
				Connected = false;
			}
		}

		public void Quit()
		{
			Close();
			Save();
			Application.Current.Shutdown();
		}

		public void Add()
		{
			var feed = new Feed();
			if (Feeds.Count > 0)
			{
				feed.ID = Feeds.Select(f => f.ID).Max() + 1;
			}
			else
			{
				feed.ID = 0;
			}
			feed.Parent = this;
			Feeds.Add(feed);
		}

		void Load()
		{
			if(File.Exists(DataFile))
			{
				using(var reader = new StreamReader(DataFile))
				{
					Feeds = (ObservableCollection<Feed>)Serializer.Deserialize(reader);
				}
				Feeds.ForEach(f => f.Parent = this);
			}
		}

		void Save()
		{
			using (var writer = new StreamWriter(DataFile))
			{
				Serializer.Serialize(writer, Feeds);
			}
		}

		public void Remove(Feed feed)
		{
			Feeds.Remove(feed);
		}
	}
}
