using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для RestoreBackupPage.xaml
	/// </summary>
	public partial class RestoreBackupPage : Page
	{
		string PageLabel_Def = "Restoring backup copy {0} of profile {1}";
		public static SVProfile gSVP;
		public static string gpath, gname;
		public static ProgressBar gPB1;
		public static Label gMessage_Label;
		public Label gLog_Label;
		public RestoreBackupPage(SVProfile SVP, string path, string name)
		{
			InitializeComponent();
			PB1.Visibility = Visibility.Hidden;
			gPB1 = PB1;
			Message_label.Visibility = Visibility.Hidden;
			gMessage_Label = Message_label;
			Log_label.Visibility = Visibility.Hidden;
			gLog_Label = Log_label;
			gSVP = SVP;
			gpath = path;
			gname = name;
			RestoreBackup_Label.Content = String.Format(PageLabel_Def, name.Replace("_","__"), SVP.Name);
			List<string> files = Backups.getfilesinbackup(path);
			foreach (String file in files)
			{
				ListBoxItem item = new ListBoxItem();
				item.Content = file;
				item.Foreground = new SolidColorBrush(Colors.White);
				BackupFileslist.Items.Add(item);
			}
		}

		private void CancelBTN_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.gRestoreBackupFrame.Visibility = Visibility.Hidden;
		}

		private void RestoreBackupBTN_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to start restore?\n\nCheck all parameters. Please note, this action cannot be undone!", "Start restore", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			bool start = false;
			switch (result)
			{
				case MessageBoxResult.Yes:
					start = true;
					break;
				case MessageBoxResult.No:
					start = false;
					break;
			}

			if (start)
			{
				CancelBTN.IsEnabled = false;
				RestoreBackupBTN.IsEnabled = false;
				gPB1.Visibility = Visibility.Visible;
				gMessage_Label.Visibility = Visibility.Visible;
				bool createbackup = CreateBackup_CB.IsChecked.Value;
				bool clearSVFolder = ClearSVFolder_CB.IsChecked.Value;
				gLog_Label.Visibility = Visibility.Visible;
				Task t = Task.Run(() => RestoreBackup(createbackup,clearSVFolder,gSVP,gpath,gname));
				t.ContinueWith((BakcupTask) =>
				{
					Restore_complete();
				});
			}
		}

		static void RestoreBackup(bool createbackup, bool clearSVFolder,SVProfile SVP, string path, string name)
		{
			if (createbackup)
			{
				Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				string backupname = String.Format("RestoreBackup_{0}", unixTimestamp);
				MainWindow.gRestoreBackupFrame.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestoreBackupPage.gLog_Label.Content = "Create Backup"));
				Backups.Create(SVP, backupname, false);
			}
			if (clearSVFolder)
			{
				MainWindow.gRestoreBackupFrame.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestoreBackupPage.gLog_Label.Content = "Clear SV Folder"));
				DirectoryInfo dir = new DirectoryInfo(SVP.Path);
				foreach (FileInfo file in dir.GetFiles())
				{
					file.Delete();
				}
				foreach (DirectoryInfo cdir in dir.GetDirectories())
				{
					cdir.Delete(true);
				}
			}
			MainWindow.gRestoreBackupFrame.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestoreBackupPage.gLog_Label.Content = "Restore Backup"));
			Backups.RestoreBackup(path, SVP.Path);
		}

		static void Restore_complete()
		{
			MainWindow.gRestoreBackupFrame.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestoreBackupPage.gLog_Label.Content = "Restore Backup Complete!"));
			MessageBoxResult result = MessageBox.Show("Restore completed successfully!", "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
			MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.LoadBackups(MainWindow.LoadedProfile.Name)));
			MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gRestoreBackupFrame.Visibility = Visibility.Hidden));
		}
	}
}
