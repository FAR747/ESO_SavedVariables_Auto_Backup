using System;
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
		public static void Create(SVProfile Profile, string name)
		{

			string SVpath = Profile.Path;
			string backupdir = SettingsVars.Backupdir + "\\" + Profile.Name;
			if (Directory.Exists(backupdir))
			{
				MainWindow.setBP_Visible(true);
				MainWindow.gCreateback_Button.IsEnabled = false;
				Task t = Task.Run(() => CreateBackup(name,SVpath,backupdir,Profile.Name));
				t.ContinueWith((BakcupTask) =>
				{
					CreateBackup_complete();
				});
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

		}
		static void CreateBackup(string name, string path, string outputpath, string profilename)
		{
			string fullpath = String.Format("{0}/{1}.ESVAB.zip",outputpath,name);
			string fullpathmanifest = String.Format("{0}/{1}.ESVAB.backup", outputpath, name);
			ZipFile.CreateFromDirectory(path, fullpath);
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			FileIniDataParser parser = new FileIniDataParser();
			IniData manifest = new IniData();
			manifest.Sections.AddSection("config");
			manifest["config"].AddKey("name", name);
			manifest["config"].AddKey("date", unixTimestamp.ToString());
			manifest["config"].AddKey("ver", BACKUP_VERSION.ToString());
			parser.WriteFile(fullpathmanifest, manifest);
			
		}
		public static List<string> getfilesinbackup(string path)
		{
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
