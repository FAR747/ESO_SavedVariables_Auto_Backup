using System;
using System.Collections.Generic;
using System.Globalization;
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
using IniParser;
using IniParser.Model;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static int VERSION_CODE = 1;
		public static string VERSION_NAME = "1.0";

		public FileIniDataParser gIniParser = new FileIniDataParser();
		public static SVProfile LoadedProfile;

		#region UI
		public static System.Windows.Forms.NotifyIcon ni;
		public static bool run_startMinimized = false;
		public static Grid ui_BackupWorkspacke;
		public static Frame Frame_Startup;
		public static MenuItem gProfileMenuItem;
		public static ListBox gBackuplist;
		public static ListBox gBackuplistFiles;
		public static ProgressBar gPB1;
		public delegate void InvokeDelegate_gPB1(Visibility visibility);
		public static Button gCreateback_Button;
		public delegate void InvokeDelegate_gCB1(bool enabled);
		public static MenuItem gOpenESOFolder_MItem;
		public static MenuItem gOpenESOSVFolder_MItem;
		public static RestoreBackupPage gRestoreBackupPage;
		public static Frame gRestoreBackupFrame;
		#endregion UI

		public MainWindow()
		{
			InitializeComponent();
			#region InitUI
			ui_BackupWorkspacke = BackupWorkspacke;
			Frame_Startup = StartUpFrame;
			gProfileMenuItem = ProfileMenuItem;
			gBackuplist = Backuplist;
			//gBackuplistFiles = BackupFileslist;
			gPB1 = PB1;
			gCreateback_Button = Createback_Button;
			gOpenESOFolder_MItem = OpenESOFolder_MItem;
			gOpenESOSVFolder_MItem = OpenESOSVFolder_MItem;
			gRestoreBackupFrame = RestoreBackupFrame;
			#endregion InitUI
			gPB1.Visibility = Visibility.Hidden;
			backup_info_grid.Visibility = Visibility.Hidden;
			CheckFiles_button.Visibility = Visibility.Hidden;
			RestoreBackupFrame.Visibility = Visibility.Hidden;

			#region tray
			ni = new System.Windows.Forms.NotifyIcon();
			Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/esvab_icon.ico")).Stream;
			ni.Icon = new System.Drawing.Icon(iconStream);
			ni.Visible = true;
			ni.DoubleClick +=
				delegate (object sender, EventArgs args)
				{
					this.Show();
					this.WindowState = WindowState.Normal;
				};
			ni.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
			ni.ContextMenuStrip.Items.Add("Show").Click += (s, e) => { this.Show(); this.WindowState = WindowState.Normal; };
			ni.ContextMenuStrip.Items.Add("Create Backup").Click += (s, e) => Createback_Button_Click(null,null);
			ni.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => Exit_MI_Click(null,null);
			#endregion tray

			init();
			if (run_startMinimized)
			{
				this.Hide();
				this.WindowState = WindowState.Minimized;
			}
			AutoBackups.init();
		}
		protected override void OnStateChanged(EventArgs e)
		{
			if (WindowState == System.Windows.WindowState.Minimized)
				this.Hide();
			base.OnStateChanged(e);
		}
		public static void init()
		{
			bool isconfigfound = SettingsVars.checkExistConf();
			if (isconfigfound)
			{
				SettingsVars.LoadConfig();
				SettingsVars.LoadProfiles();
				Frame_Startup.Visibility = Visibility.Hidden;
				profile_init();
				run_startMinimized = false;
				StartupEventArgs args = App.gARGS;
				for (int i = 0; i != args.Args.Length; ++i)
				{
					if (args.Args[i] == "-StartMinimized")
					{
						run_startMinimized = true;
					}
				}
			}
			else
			{
				Frame_Startup.Visibility = Visibility.Visible;
			}
		}
		public static void setBP_Visible(bool isvisible)
		{
			if (isvisible)
			{
				gPB1.Visibility = Visibility.Visible;
			}
			else
			{
				gPB1.Visibility = Visibility.Hidden;
			}
		}
		static void profile_init()
		{
			bool isfirst = true;
			foreach (SVProfile SVP in SettingsVars.Profiles)
			{
				RadioButton RB = new RadioButton();
				RB.Name = "RBProfile_" + SVP.Name;
				RB.Content = SVP.Name;
				RB.Tag = SVP.Path;
				RB.GroupName = "SVProfiles";
				RB.Checked += new RoutedEventHandler(Profiles_RB_Checked);
				if (isfirst)
				{
					RB.IsChecked = true;
					isfirst = false;
				}
				gProfileMenuItem.Items.Add(RB);
			}
		}

		static void Profiles_RB_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton pressed = (RadioButton)sender;
			//System.Diagnostics.Debug.WriteLine(pressed.Name);
			string profilename = pressed.Name.Replace("RBProfile_", "");
			string profilepath = pressed.Tag.ToString();
			LoadProfile(profilename, profilepath);
			gProfileMenuItem.Header = String.Format("Profile ({0})", profilename);
			
		}

		static void LoadProfile(string Name, string Path)
		{
			bool found = false;
			SVProfile profile = new SVProfile();
			foreach (SVProfile SVP in SettingsVars.Profiles)
			{
				
				if (SVP.Name == Name && SVP.Path == Path)
				{
					profile = SVP;
					found = true;
					LoadedProfile = SVP;
					break;
				}
			}
			if (found)
			{
				LoadBackups(profile.Name);
				gOpenESOFolder_MItem.Header = String.Format("Open {0} folder", profile.Name);
				gOpenESOSVFolder_MItem.Header = String.Format("Open {0} SavedVars folder", profile.Name);
			}
		}
		public static void LoadBackups(string name)
		{
			string path = SettingsVars.Backupdir + "\\" + name;
			gBackuplist.Items.Clear();
			string manifest = path + "\\backupdirectory.cfg";
			FileIniDataParser parser = new FileIniDataParser();
			DirectoryInfo files = new DirectoryInfo(path);
			FileInfo[] dFiles = files.GetFiles("*.ESVAB.zip").OrderByDescending(p => p.CreationTime).ToArray();
			foreach (FileInfo file in dFiles)
			{
				//SVFile.Foreground = new SolidColorBrush(Colors.White);
				//System.Diagnostics.Debug.WriteLine(file.FullName);
				string cfgname = file.FullName.Replace(".zip", ".backup");
				string bpath = file.FullName;
				if (File.Exists(cfgname))
				{
					IniData data = parser.ReadFile(cfgname);
					string bname = data["config"]["name"];
					string bverstr = data["config"]["ver"];
					string bdatestr = data["config"]["date"];
					DateTime bDT = SettingsFuncs.UnixTimeStampToDateTime(Convert.ToInt64(bdatestr));
					string bDateTime = bDT.ToString("G", DateTimeFormatInfo.InvariantInfo);
					string UCName = String.Format("{0} Backup {1}",name,bDateTime);
					long size = file.Length;
					size = size / 1024;
					BackupInfo_list_UC BILUC = new BackupInfo_list_UC(bname, String.Format("{0}kb",size), bpath,bDateTime);
					gBackuplist.Items.Add(BILUC);
				}
			}
		}

		private void Createback_Button_Click(object sender, RoutedEventArgs e)
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			string name = String.Format("Backup_{0}", unixTimestamp);
			Backups.Create(LoadedProfile, name, true);
		}

		private void Backuplist_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			BackupInfo_list_UC curitem = (BackupInfo_list_UC)Backuplist.SelectedItem;
			if (curitem == null)
			{
				backup_info_grid.Visibility = Visibility.Hidden;
				return;
			}
			backup_info_grid.Visibility = Visibility.Visible;
			BackupNameTB.Text = curitem.gname;
			BackupSizeTB.Text = curitem.gsize;
			BackupDateTB.Text = curitem.gdate;
			OpenBackupFile.Tag = curitem.gpath;
			BackupFileslist.Items.Clear();
			List<string> files = Backups.getfilesinbackup(curitem.gpath);
			foreach (String file in files)
			{
				ListBoxItem item = new ListBoxItem();
				item.Content = file;
				item.Foreground = new SolidColorBrush(Colors.White);
				BackupFileslist.Items.Add(item);
			}
			
			//System.Diagnostics.Debug.WriteLine(curitem.gname);
		}

		private void OpenBackupFile_Click(object sender, RoutedEventArgs e)
		{
			if (OpenBackupFile.Tag != null)
			{
				if (File.Exists(OpenBackupFile.Tag.ToString()))
				{
					System.Diagnostics.Process.Start(OpenBackupFile.Tag.ToString());
				}
			}
		}

		private void OpenESOFolder_MItem_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(LoadedProfile.Path.Replace("SavedVariables", ""));
		}

		private void OpenESOSVFolder_MItem_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(LoadedProfile.Path);
		}

		private void Exit_MI_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void RestoreBackupBTN_Click(object sender, RoutedEventArgs e)
		{
			gRestoreBackupPage = null;
			//gRestoreBackupPage = new RestoreBackupPage();
			BackupInfo_list_UC curitem = (BackupInfo_list_UC)Backuplist.SelectedItem;
			string name = curitem.gname;
			string path = curitem.gpath;
			gRestoreBackupPage = new RestoreBackupPage(LoadedProfile,path,name);
			gRestoreBackupFrame.Navigate(gRestoreBackupPage);
			gRestoreBackupFrame.Visibility = Visibility.Visible;
		}

		private void Settings_MT_Click(object sender, RoutedEventArgs e)
		{
			new SettingsWindow().ShowDialog();
		}

		private void MainWindow1_Closed(object sender, EventArgs e)
		{
			ni.Visible = false;
			ni.Dispose();
		}
	}
}
