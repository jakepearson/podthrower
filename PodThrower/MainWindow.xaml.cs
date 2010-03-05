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
using PodThrower.Model;
using System.ComponentModel;

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

			var document = new Document(this);
			//document.Add();
			DataContext = document;


			//Connect();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
			e.Cancel = true;
		}
	}
}