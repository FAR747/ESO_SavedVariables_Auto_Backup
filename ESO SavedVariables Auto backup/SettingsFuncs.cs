using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ESO_SavedVariables_Auto_backup
{
	class SettingsFuncs
	{
		static string startupfolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
		static string startup_shorcutname = "ESVAB_startup.lnk";
		public static string openDirectoryDialog()
		{
			string ret;
			FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
			DialogResult result = folderBrowser.ShowDialog();
			ret = folderBrowser.SelectedPath;
			return ret;
		}
		public static DateTime UnixTimeStampToDateTime(Int64 unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		public static void setstartupwindows (bool enable)
		{
			string scpath = startupfolder + "\\" + startup_shorcutname;
			if (enable)
			{
				IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
				IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
				Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + startup_shorcutname) as IWshRuntimeLibrary.IWshShortcut;
				shortcut.Arguments = " -StartMinimized";
				shortcut.TargetPath = Application.ExecutablePath;
				//shortcut.Description = "my shortcut description";
				shortcut.WorkingDirectory = Application.StartupPath;
				shortcut.Save();
			}
			else
			{
				if (File.Exists(scpath))
				{
					File.Delete(scpath);
				}
			}
		}
		public static bool getstartupwindows()
		{
			if (File.Exists(startupfolder + "\\" + startup_shorcutname))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
