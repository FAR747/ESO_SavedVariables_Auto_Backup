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
	/// Логика взаимодействия для BackupInfo_list_UC.xaml
	/// </summary>
	public partial class BackupInfo_list_UC : UserControl
	{
		public string gname, gsize, gpath, gdate;

		private void Delete_MI_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(String.Format("Are you sure you want to delete backup {0}?\n\nThis action cannot be undone!", gname), "Delete Backup", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			switch (result)
			{
				case MessageBoxResult.Yes:
					File.Delete(gpath.Replace(".zip", ".backup"));
					File.Delete(gpath);
					MainWindow.LoadBackups(MainWindow.LoadedProfile.Name);
					break;
				case MessageBoxResult.No:
					
					break;
			}
		}

		public BackupInfo_list_UC(string name, string size, string path, string date)
		{
			InitializeComponent();
			Label_Name.Content = name.Replace("_","__");
			Label_Size.Content = size;
			Label_Date.Content = date;
			gpath = path;
			gsize = size;
			gname = name;
			gdate = date;
		}
	}
}
