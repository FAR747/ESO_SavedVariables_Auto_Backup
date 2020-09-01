﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESO_SavedVariables_Auto_backup
{
	class AutoBackups
	{
		static Timer timer;
		public static bool ESORunned = false;
		public static void init()
		{
			if (SettingsVars.autobackup_startup)
			{
				Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				string name = String.Format("AutoBackup_{0}", unixTimestamp);
				//Backups.Create(MainWindow.LoadedProfile, name, true);
				foreach (SVProfile SVP in SettingsVars.Profiles)
				{
					Backups.Create(SVP, name, true);
				}
			}
			ESORunned = CheckESO();
			System.Diagnostics.Debug.WriteLine("ESO Status: " + ESORunned);
			TimerCallback tm = new TimerCallback(Timer_tick);
			timer = new Timer(tm, 0, 0, 10000); //2mins 120000
		}

		static void Timer_tick(object obj)
		{
			
				bool currentESORunned = CheckESO();
				if (ESORunned && !currentESORunned)
				{
					ESORunned = currentESORunned;
					System.Diagnostics.Debug.WriteLine("ESO Status: " + "EXIT");
					if (SettingsVars.autobackup_exitESO)
					{
						Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
						string name = String.Format("AutoBackup_{0}", unixTimestamp);
						//Backups.Create(MainWindow.LoadedProfile, name, true);
						foreach (SVProfile SVP in SettingsVars.Profiles)
						{
							Backups.Create(SVP, name, true);
						}
					}
				}
				else if (currentESORunned && !ESORunned)
				{
					ESORunned = currentESORunned;
					System.Diagnostics.Debug.WriteLine("ESO Status: " + "START");
				}
		}
		static bool CheckESO()
		{
			bool check = System.Diagnostics.Process.GetProcessesByName("eso64").Any();

			if (check)
			{
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Content = "ESO Running"));
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Visibility = System.Windows.Visibility.Visible));
			}
			else
			{
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Visibility = System.Windows.Visibility.Hidden));
			}

			return check;
		}
	}
}
