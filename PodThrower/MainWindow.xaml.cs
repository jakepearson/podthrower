using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace PodThrower
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Member Variables
		WebServiceHost host;
		#endregion

		public MainWindow()
		{
			InitializeComponent();

			Connect();
		}

		void Connect()
		{
			Uri address = new Uri("http://localhost:8000/NewsFeedService/");
			host = new WebServiceHost(typeof(NewsFeedService), address);

			try
			{
				host.Open();
			}
			catch (CommunicationException ce)
			{
				MessageBox.Show(ce.ToString());
				host.Abort();
				host = null;
			}
		}
	}
}
