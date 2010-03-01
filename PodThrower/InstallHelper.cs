using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;

namespace PodThrower
{
	[RunInstaller(true)]
	public partial class InstallHelper : Installer
	{
		public InstallHelper()
		{
			InitializeComponent();
		}

		protected override void OnCommitted(IDictionary savedState)
		{
			base.OnCommitted(savedState);

			var process = Process.Start("netsh", @"http add urlacl url=http://+:4242/podthrower user=BUILTIN\Users");
			process.WaitForExit();
		}
	}
}
