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

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для BackupInfo_list_UC.xaml
	/// </summary>
	public partial class BackupInfo_list_UC : UserControl
	{
		public static string gname, gsize, gpath, date;
		public BackupInfo_list_UC(string name, string size, string path, string date)
		{
			InitializeComponent();
			Label_Name.Content = name;
			Label_Size.Content = size;
			Label_Date.Content = date;
			gpath = path;
			gsize = size;
			gpath = path;
		}
	}
}
