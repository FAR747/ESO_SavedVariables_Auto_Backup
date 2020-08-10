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
using System.Windows.Shapes;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для UpdateAvailable.xaml
	/// </summary>
	public partial class UpdateAvailable : Window
	{
		public UpdateAvailable()
		{
			InitializeComponent();
		}

		private void OpenUPDBTN_Click(object sender, RoutedEventArgs e)
		{
			
			MessageBox.Show("ESO SavedVariables Auto Backup will be closed!\n\nDownload the archive and unpack it to where the program is installed (The folder will open automatically)", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
			System.Diagnostics.Process.Start("https://hgplay.ru/go.php?a=ESVAB-UpdatePage");
			System.Diagnostics.Process.Start(Directory.GetCurrentDirectory());
			System.Windows.Application.Current.Shutdown();
		}

		private void LaterBTN_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
