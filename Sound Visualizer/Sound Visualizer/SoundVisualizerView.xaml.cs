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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sound_Visualizer
{
    /// <summary>
    /// Interaction logic for SoundVisualizerView.xaml
    /// </summary>
    public partial class SoundVisualizerView : Window
    {
        public SoundVisualizerView()
        {
            InitializeComponent();

            System.Windows.Forms.NotifyIcon NotifyIcon = new System.Windows.Forms.NotifyIcon();
            NotifyIcon.Icon = new System.Drawing.Icon("Icon.ico");
            NotifyIcon.Visible = true;
            NotifyIcon.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            Overlay.SetWindowExTransparent(hwnd);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if(WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }


    }
}
