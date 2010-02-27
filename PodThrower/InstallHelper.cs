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

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			Process.Start("netsh", @"http add urlacl url=http://+:4242/PodThrower user=BUILTIN\Users");
		}
	}
}
