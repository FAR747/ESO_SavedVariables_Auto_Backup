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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для StartUpWindow.xaml
	/// </summary>
	public partial class StartUpWindow : Page
	{
		public static int currentSteep = 0;

		public static string ESODir = "none";
		public static string BackupDir = "none";
		public StartUpWindow()
		{
			InitializeComponent();
			Page_0.Visibility = Visibility.Visible;
			Page_1.Visibility = Visibility.Hidden;
			Page_2.Visibility = Visibility.Hidden;
			Page_3.Visibility = Visibility.Hidden;
		}

		private void Next_Button_Click(object sender, RoutedEventArgs e)
		{
			if (currentSteep == 0)
			{
				Page_0.Visibility = Visibility.Hidden;
				Page_1.Visibility = Visibility.Visible;
				Page_1_Load();
				currentSteep++;
			}
			else if (currentSteep == 1){
				currentSteep++;
				Page_1.Visibility = Visibility.Hidden;
				Page_2.Visibility = Visibility.Visible;
				Page_2_Load();
			}
			else if (currentSteep == 2)
			{
				currentSteep++;
				Page_2.Visibility = Visibility.Hidden;
				Page_3.Visibility = Visibility.Visible;
				Next_Button.Content = "Finish";
			}
			else if (currentSteep == 3)
			{
				currentSteep++;
				Finish_settings();
			}
		}
		
		private void Page_1_Load()
		{
			string mydocyments = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			Next_Button.IsEnabled = false;
			Search_ESOFolder(mydocyments);
		}
		private void Page_2_Load()
		{
			string mydocyments = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			BackupDir = ESODir + "\\"+ SettingsVars.DefFolderName;
			DirectoryBS_TB.Text = BackupDir;

			string[] dirs = Directory.GetDirectories(ESODir);
			foreach (string dir in dirs)
			{
				string SV = dir + "/SavedVariables";
				if (Directory.Exists(SV))
				{
					string dirName = new DirectoryInfo(dir).Name;
					if (dirName.IndexOf(' ') <= 0)
					{
						CheckBox cb1 = new CheckBox();
						cb1.Tag = SV;
						cb1.Content = dirName;
						cb1.IsChecked = true;
						cb1.Foreground = new SolidColorBrush(Colors.White);
						cb1.ToolTip = dir;
						if (dirName == "live")
						{
							cb1.IsEnabled = false;
						}
						addprofiles_LV.Items.Add(cb1);
					}
				}
			}
		}

		private void Finish_settings()
		{
			SettingsVars.Backupdir = BackupDir;
			SettingsVars.ESODir = ESODir;
			SettingsVars.firstsettings_finish();
			foreach (CheckBox CB in addprofiles_LV.Items)
			{
				//System.Diagnostics.Debug.WriteLine(CB.Tag);
				if (CB.IsChecked == true)
				{
					SVProfile SVP = new SVProfile();
					SVP.Name = CB.Content.ToString();
					SVP.Path = CB.Tag.ToString();
					SettingsVars.addprofile(SVP);
				}
			}
			MainWindow.init();
		}

		private void Search_ESOFolder(string path)
		{
			string ESOFolder = path + "\\Elder Scrolls Online";
			if (Directory.Exists(ESOFolder))
			{
				DirectoryTB.Text = ESOFolder;
				ESOFolder_init(ESOFolder);
				ESODir = ESOFolder;
			}
			else
			{
				MessageBox.Show("Could not find ESO folder. Specify it manually!", "Warning");
			}
		}

		private void ESOFolder_init(string path)
		{
			string[] dirs = Directory.GetDirectories(path);
			ESOSVTree.Items.Clear();
			bool isfound = false;
			foreach (string dir in dirs)
			{
				string SV = dir + "/SavedVariables";
				if (Directory.Exists(SV))
				{
					string dirName = new DirectoryInfo(dir).Name;
					isfound = true;
					if (dirName.IndexOf(' ') <= 0)
					{
						TreeViewItem SVFolder = new TreeViewItem();
						SVFolder.Header = dirName;
						SVFolder.Foreground = new SolidColorBrush(Colors.White);
						ESOSVTree.Items.Add(SVFolder);
						//string[] files = Directory.GetFiles(SV, "*.lua");
						DirectoryInfo files = new DirectoryInfo(SV);
						foreach (FileInfo file in files.GetFiles("*.lua"))
						{
							TreeViewItem SVFile = new TreeViewItem();
							SVFile.Foreground = new SolidColorBrush(Colors.White);
							SVFile.Header = file.Name;
							SVFolder.Items.Add(SVFile);
						}
					}
				}
			}
			if (isfound)
			{
				Next_Button.IsEnabled = true;
			}
			else
			{
				MessageBox.Show("Could not find SavedVariables. Check if the ESO folder is correct.", "Warning");
				Next_Button.IsEnabled = false;
			}
		}

		private void OpenDirectoryDialog_Click(object sender, RoutedEventArgs e)
		{
			string directory = SettingsFuncs.openDirectoryDialog();
			//System.Diagnostics.Debug.Write(directory);
			DirectoryTB.Text = directory;
			ESODir = directory;
			ESOFolder_init(directory);
		}

		private void OpefDirectoryDialog_BS_Click(object sender, RoutedEventArgs e)
		{
			string directory = SettingsFuncs.openDirectoryDialog();
			DirectoryBS_TB.Text = directory;
			BackupDir = directory;
		}

		private void BS_RB_Checked(object sender, RoutedEventArgs e)
		{
			if (currentSteep != 2)
			{
				return;
			}
			RadioButton pressed = (RadioButton)sender;
			//System.Diagnostics.Debug.Write(pressed.Name); 
			string ESOFolder_RB_Name = "BS_RB_ESOFolder";
			string Custom_RB_Name = "BS_RB_Custom";
			if (pressed.Name == ESOFolder_RB_Name)
			{
				string mydocyments = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				BackupDir = ESODir + "\\" + SettingsVars.DefFolderName;
				DirectoryBS_TB.Text = BackupDir;
			}
			else
			{

			}
		}
	}
}
