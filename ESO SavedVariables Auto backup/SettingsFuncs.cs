using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESO_SavedVariables_Auto_backup
{
	class SettingsFuncs
	{
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
	}
}
