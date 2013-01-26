// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System;
using System.Windows.Forms;
using log4net.Config;

namespace Windwardopolis
{
	static class Program
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
		public static int exitCode = 0;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main()
		{
			XmlConfigurator.Configure();
			log.Info("***** Windwardopolis starting *****");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainWindow());
			return exitCode;
		}
	}
}
