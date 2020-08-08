﻿using System;
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
using IniParser;
using IniParser.Model;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public FileIniDataParser gIniParser = new FileIniDataParser();

		#region UI
		public static Grid ui_BackupWorkspacke;
		#endregion UI

		public MainWindow()
		{
			InitializeComponent();
			#region InitUI
			ui_BackupWorkspacke = BackupWorkspacke;
			#endregion InitUI
			init();
		}
		private void init()
		{

		}
	}
}
