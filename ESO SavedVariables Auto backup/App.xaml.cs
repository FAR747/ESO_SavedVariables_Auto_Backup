﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs args)
		{
			base.OnStartup(args);
			//System.Diagnostics.Debug.WriteLine("Process NAME: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName);
			if (!InstanceCheck())
			{
				///
				/// TODO: Maximize the window of an already open application
				///
				/*
				//System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessesByName("ESO SavedVariables Auto backup").FirstOrDefault();
				System.Diagnostics.Process[] tt = System.Diagnostics.Process.GetProcessesByName("ESO SavedVariables Auto backup");
				foreach (System.Diagnostics.Process p in tt)
				{
					if (p != null)
					{
						var element = AutomationElement.FromHandle(p.MainWindowHandle);
						if (element != null)
						{
							var pattern = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
							if (pattern != null)
							{
								pattern.SetWindowVisualState(WindowVisualState.Normal);
								pattern.SetWindowVisualState(WindowVisualState.Maximized);
							}
							element.SetFocus();
						}
					}
				}
				*/
				MessageBox.Show("ESO SavedVariables Auto Backup is already running!", "Startup error", MessageBoxButton.OK, MessageBoxImage.Error);
				System.Windows.Application.Current.Shutdown();
			}
		}

		static Mutex InstanceCheckMutex;
		static bool InstanceCheck()
		{
			bool isNew;
			InstanceCheckMutex = new Mutex(true, "ESO SavedVariables Auto Backup", out isNew);
			return isNew;
		}
	}
}
