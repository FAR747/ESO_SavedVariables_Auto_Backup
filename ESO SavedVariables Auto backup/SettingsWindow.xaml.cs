using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
			Label_version.Content = String.Format("Version {0} ({1})", MainWindow.VERSION_NAME, MainWindow.VERSION_CODE);
			startupWindows_CB.IsChecked = SettingsFuncs.getstartupwindows();
			autobackup_startup_CB.IsChecked = SettingsVars.autobackup_startup;
			autobackup_exitESO_CB.IsChecked = SettingsVars.autobackup_exitESO;
			autodeletebackups_CB.IsChecked = SettingsVars.autodeletebackups;
			maxdaybackup_TB.Text = SettingsVars.maxdaybackup.ToString();

		}

		private void ClearBTN_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void SaveBTN_Click(object sender, RoutedEventArgs e)
		{
			SettingsFuncs.setstartupwindows(startupWindows_CB.IsChecked.Value);
			SettingsVars.autodeletebackups = autodeletebackups_CB.IsChecked.Value;
			string maxdaysstr = maxdaybackup_TB.Text.Replace(" ", "");
			if (maxdaysstr == "")
			{
				maxdaysstr = "30";
			}
			int maxdaysback = Convert.ToInt32(maxdaysstr);
			if (maxdaysback <= 0)
			{
				maxdaysback = 30;
			}
			SettingsVars.maxdaybackup = maxdaysback;

			SettingsVars.autobackup_startup = autobackup_startup_CB.IsChecked.Value;
			SettingsVars.autobackup_exitESO = autobackup_exitESO_CB.IsChecked.Value;
			SettingsVars.SaveConfig();
			this.Close();
		}

		private void maxdaybackup_TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (!Char.IsDigit(e.Text, 0))
			{
				e.Handled = true;
			}
		}
	}
}
