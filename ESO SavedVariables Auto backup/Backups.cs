﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;
using IniParser;
using IniParser.Model;

namespace ESO_SavedVariables_Auto_backup
{
	class Backups
	{
		public static int BACKUP_VERSION = 1;
		public delegate void InvokeDelegate_gPB1();
		public static void Create(SVProfile Profile, string name, bool usetack)
		{

			string SVpath = Profile.Path;
			string backupdir = SettingsVars.Backupdir + "\\" + Profile.Name;
			if (Directory.Exists(backupdir))
			{
				MainWindow.gPB1.Dispatcher.BeginInvoke((Action)(() => MainWindow.gPB1.Visibility = System.Windows.Visibility.Visible));
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gCreateback_Button.IsEnabled = false));
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestore_button.IsEnabled = false));
				if (usetack)
				{
					Task t = Task.Run(() => CreateBackup(name, SVpath, backupdir, Profile.Name));
					t.ContinueWith((BakcupTask) =>
					{
						CreateBackup_complete();
					});
				}
				else
				{
					CreateBackup(name, SVpath, backupdir, Profile.Name);
					//MainWindow.gPB1.Dispatcher.BeginInvoke((Action)(() => MainWindow.gPB1.Visibility = System.Windows.Visibility.Hidden));
					//MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gCreateback_Button.IsEnabled = true));
					CreateBackup_complete();
				}
			}
			else
			{
				MessageBox.Show("Could not find profile folder!", "Error");
			}
			//Task t = Task.Run(() => CreateBackup("Task",""));
		}
		public static void CreateBackup_complete()
		{
			//MainWindow.setBP_Visible(false);
			MainWindow.gPB1.Dispatcher.BeginInvoke((Action)(() => MainWindow.gPB1.Visibility = System.Windows.Visibility.Hidden));
			MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gCreateback_Button.IsEnabled = true));
			MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.LoadBackups(MainWindow.LoadedProfile.Name)));
			MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestore_button.IsEnabled = true));
			MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.Sendpopup("Backup Complete!")));
		}
		static void CreateBackup(string name, string path, string outputpath, string profilename)
		{
			string fullpath = String.Format("{0}/{1}.ESVAB.zip",outputpath,name);
			string fullpathmanifest = String.Format("{0}/{1}.ESVAB.backup", outputpath, name);
			ZipFile.CreateFromDirectory(path, fullpath);
			Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			FileIniDataParser parser = new FileIniDataParser();
			IniData manifest = new IniData();
			manifest.Sections.AddSection("config");
			manifest["config"].AddKey("name", name);
			manifest["config"].AddKey("date", unixTimestamp.ToString());
			manifest["config"].AddKey("ver", BACKUP_VERSION.ToString());
			parser.WriteFile(fullpathmanifest, manifest);
			
		}

		public static void RestoreBackup(string path, string outputpath, List<string> truefiles)
		{
			if (!File.Exists(path))
			{
				return;
			}
			ZipArchive archive = ZipFile.OpenRead(path);
			foreach (ZipArchiveEntry file in archive.Entries)
			{
				string completeFileName = Path.Combine(outputpath, file.FullName);
				if (file.Name == "")
				{// Assuming Empty for Directory
					Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
					continue;
				}
				bool fileex = true;

				if (truefiles != null)
				{
					System.Diagnostics.Trace.WriteLine(String.Format("Check File: {0}", file.Name));
					if (truefiles.Exists(e => e == file.Name))
					{
						System.Diagnostics.Trace.WriteLine(String.Format("Check File: {0} - OK", file.Name));
						fileex = true;
					}
					else
					{
						System.Diagnostics.Trace.WriteLine(String.Format("Check File: {0} - Not Found", file.Name));
						fileex = false;
					}
				}
				if (fileex)
				{
					System.Diagnostics.Trace.WriteLine(String.Format("Restore File: {0}", file.Name));
					file.ExtractToFile(completeFileName, true);
				}
			}
		}
		public static List<string> getfilesinbackup(string path)
		{
			if (File.Exists("./getpaths"))
			{
				File.WriteAllText("./Last_Arhive_getfiles_path", path);
			}
			List<string> files = new List<string>();
			if (File.Exists(path))
			{
				var backup = ZipFile.OpenRead(path);
				foreach (ZipArchiveEntry entry in backup.Entries)
				{
					files.Add(entry.Name);
				}
			}
			else
			{
				files.Add("Error: Backup not found!");
			}
			return files;

		}

	}
}
