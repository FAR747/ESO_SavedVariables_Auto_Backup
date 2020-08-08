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

		#region UI
		public static Grid ui_BackupWorkspacke;
		public static Frame Frame_Startup;
		public static MenuItem gProfileMenuItem;
		public static ListBox gBackuplist;
		#endregion UI

		public MainWindow()
		{
			InitializeComponent();
			#region InitUI
			ui_BackupWorkspacke = BackupWorkspacke;
			Frame_Startup = StartUpFrame;
			gProfileMenuItem = ProfileMenuItem;
			gBackuplist = Backuplist;
			#endregion InitUI
			init();
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
			}
			else
			{
				Frame_Startup.Visibility = Visibility.Visible;
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
					break;
				}
			}
			if (found)
			{
				LoadBackups(profile.Name);
			}
		}
		static void LoadBackups(string name)
		{
			string path = SettingsVars.Backupdir + "\\" + name;
			string manifest = path + "\\backupdirectory.cfg";
			DirectoryInfo files = new DirectoryInfo(path);
			foreach (FileInfo file in files.GetFiles("*.ESVAB.zip"))
			{
				//SVFile.Foreground = new SolidColorBrush(Colors.White);

			}
		}
	}
}
