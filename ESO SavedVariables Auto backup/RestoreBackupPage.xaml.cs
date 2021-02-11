using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
			SelectionFiles_Menu_TB.Visibility = Visibility.Hidden;
			gMessage_Label = Message_label;
			Log_label.Visibility = Visibility.Hidden;
			gLog_Label = Log_label;
			gSVP = SVP;
			gpath = path;
			gname = name;
			RestoreBackup_Label.Content = String.Format(PageLabel_Def, name.Replace("_", "__"), SVP.Name);
			if (AutoBackups.ESORunned)
			{
				ESORunning_message.Visibility = Visibility.Visible;
			}
			else
			{
				ESORunning_message.Visibility = Visibility.Hidden;
			}
			List<string> files = Backups.getfilesinbackup(path);
			foreach (String file in files)
			{
				/*ListBoxItem item = new ListBoxItem();
				item.Content = file;
				item.Foreground = new SolidColorBrush(Colors.White);
				*/
				CheckBox cbitem = new CheckBox();
				cbitem.IsChecked = true;
				cbitem.Content = file;
				cbitem.IsEnabled = false;
				cbitem.Foreground = new SolidColorBrush(Colors.White);

				BackupFileslist.Items.Add(cbitem);
			}
		}

		private void CancelBTN_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.gRestoreBackupFrame.Visibility = Visibility.Hidden;
		}

		private void RestoreBackupBTN_Click(object sender, RoutedEventArgs e)
		{
			bool restore = true;
			if (AutoBackups.ESORunned)
			{
				restore = MainWindow.esorunningmessagebox(1);
			}
			if (restore)
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
					List<string> Restore_TrueFiles = new List<string>();
					if (EnableCustomRestore_CB.IsChecked.Value)
					{
						System.Diagnostics.Trace.WriteLine("Start restore via custom. Files:");
						foreach (CheckBox CB in BackupFileslist.Items)
						{
							if (CB.IsChecked.Value)
							{
								Restore_TrueFiles.Add(CB.Content.ToString());
								System.Diagnostics.Trace.WriteLine(CB.Content.ToString());
							}
						}
					}
					else
					{
						Restore_TrueFiles = null;
					}
					Task t = Task.Run(() => RestoreBackup(createbackup, clearSVFolder, gSVP, gpath, gname, Restore_TrueFiles));
					t.ContinueWith((BakcupTask) =>
					{
						Restore_complete();
					});
				}
			}
		}

		void RestoreBackup(bool createbackup, bool clearSVFolder, SVProfile SVP, string path, string name, List<string> Restore_TrueFiles)
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
			Backups.RestoreBackup(path, SVP.Path, Restore_TrueFiles);
		}

		private void EnableCustomRestore_CB_Click(object sender, RoutedEventArgs e)
		{
			if (EnableCustomRestore_CB != null)
			{
				changeCBstatesinFileList(EnableCustomRestore_CB.IsChecked.Value);
				if (EnableCustomRestore_CB.IsChecked.Value)
				{
					SelectionFiles_Menu_TB.Visibility = Visibility.Visible;
				}
				else
				{
					SelectionFiles_Menu_TB.Visibility = Visibility.Hidden;
				}
			}
		}

		private void changeCBstatesinFileList(bool isenable)
		{
			foreach (CheckBox cb in BackupFileslist.Items)
			{
				cb.IsEnabled = isenable;
				if (!isenable)
				{
					cb.IsChecked = true;
				}
			}
		}
		private void allcheckinFileList(bool ischecked)
		{
			foreach (CheckBox cb in BackupFileslist.Items)
			{
				cb.IsChecked = ischecked;
			}
		}

		private void SelectionFiles_SAll_HL_Click(object sender, RoutedEventArgs e)
		{
			allcheckinFileList(true);
		}

		private void SelectionFiles_USAll_HL_Click(object sender, RoutedEventArgs e)
		{
			allcheckinFileList(false);
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
