using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.ServiceModel.Samples.Syndication;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Xml;
using System.ServiceModel.Syndication;
using System.ServiceModel;

namespace PodThrower
{
	public partial class MainForm : Form
	{
		WebServiceHost host;

		public MainForm()
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

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (FormWindowState.Minimized == WindowState)
			{
				Hide();
			}
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (host != null)
			{
				host.Close();
			}
		}
	}
}
