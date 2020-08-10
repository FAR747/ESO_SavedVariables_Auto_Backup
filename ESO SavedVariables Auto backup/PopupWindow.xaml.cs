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
using System.Threading;
using System.Threading.Tasks;

namespace ESO_SavedVariables_Auto_backup
{
	/// <summary>
	/// Логика взаимодействия для PopupWindow.xaml
	/// </summary>
	public partial class PopupWindow : Window
	{
		static int WPos = -30;
		static int HWin = 15;
		static int waittime = 100;
		public Timer timer;
		public PopupWindow(string message)
		{
			InitializeComponent();
			WPos = -30;
			HWin = 15;
			waittime = 100;
			Text_Label.Content = message;
			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
			this.Left = desktopWorkingArea.Right - this.Width - -30;
			this.Top = desktopWorkingArea.Bottom - this.Height - 15;
			TimerCallback tm = new TimerCallback((object obj) => TimerTick_IN(obj, this));
			timer = new Timer(tm, 0, 0, 1);
		}
		static void TimerTick_IN(object obj, PopupWindow window)
		{
			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
			if (WPos < 15)
			{
				WPos = WPos + 5;
				window.Dispatcher.BeginInvoke((Action)(() => window.Left = desktopWorkingArea.Right - window.Width - WPos));
				
			}else if (waittime != 0)
			{
				waittime--;
			}
			else if (HWin > -100)
			{
				HWin = HWin - 15;
				window.Dispatcher.BeginInvoke((Action)(() => window.Top = desktopWorkingArea.Bottom - window.Height - HWin));
			}
			else
			{
				window.Dispatcher.BeginInvoke((Action)(() => window.timer.Dispose()));
				window.Dispatcher.BeginInvoke((Action)(() => window.Close()));
			}
			
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			timer.Dispose();
			this.Close();
		}
	}
}
