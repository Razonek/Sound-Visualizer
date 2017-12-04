using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Drawing;
using System.Windows.Media;

namespace Sound_Visualizer
{

    public delegate void SetBarWidth(int Width);
    public delegate void SetBarColor(int R,int G, int B);
    public delegate void SetMode(Enums.Mode Mode);
    public delegate void IsEnabled(Enums.Hide Side, bool Value);
    public delegate void GetDeviceList(List<string> List);

    class SoundVisualizerViewModel : Screen
    {

        WindowManager Window;
        
        public static SetBarColor SetColor;
        public static SetBarWidth SetBar;
        public static SetMode SetMode;
        public static IsEnabled ToggleStance;
        public static SetBarWidth SetDevice;

        public SoundVisualizerViewModel()
        {
            this.DisplayName = "Sound Visualizer";
            Window = new WindowManager();
            AudioOutput = new List<string>();
            Analyzer.SendList += SetDeviceList;
            Window.ShowWindow(new FXWindowViewModel());
            LoadSettings();
            ComboDisable = true;
            DeviceSelectedIndex = 0;


        }

        public void SetDeviceList(List<string> List)
        {
            AudioOutput.Clear();
            foreach(string device in List)
            {
                AudioOutput.Add(device);
            }
        }

        private string _SelectedAudioOutput;
        public string SelectedAudioOutput
        {
            get { return _SelectedAudioOutput; }
            set
            {
                _SelectedAudioOutput = value;
                var array = value.Split(' ');
                SetDevice(Convert.ToInt32(array[0]));
                NotifyOfPropertyChange("SelectedAudioOutput");
            }
        }

        private int _DeviceSelectedIndex;
        public int DeviceSelectedIndex
        {
            get { return _DeviceSelectedIndex; }
            private set
            {
                _DeviceSelectedIndex = value;
                NotifyOfPropertyChange("DeviceSelectedIndex");
            }
        }

        private List<string> _AudioOutput;
        public List<string> AudioOutput
        {
            get { return _AudioOutput; }
            set
            {
                _AudioOutput = value;
                NotifyOfPropertyChange("AudioOutput");
            }
        }



        protected override void OnDeactivate(bool close)
        {
            ToggleStance(Enums.Hide.Close, true);
            SaveSettings();
            base.OnDeactivate(close);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Width = BarWidthSlider;
            Properties.Settings.Default.LeftBar = LeftSide;
            Properties.Settings.Default.RightBar = RightSide;
            Properties.Settings.Default.R = RedBarIndicator;
            Properties.Settings.Default.G = GreenBarIndicator;
            Properties.Settings.Default.B = BlueBarIndicator;
            Properties.Settings.Default.CutDown = CutDownSelector;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            BarWidthSlider = Properties.Settings.Default.Width;
            LeftSide = Properties.Settings.Default.LeftBar;
            RightSide = Properties.Settings.Default.RightBar;
            RedBarIndicator = Properties.Settings.Default.R;
            GreenBarIndicator = Properties.Settings.Default.G;
            BlueBarIndicator = Properties.Settings.Default.B;
            CutDownSelector = Properties.Settings.Default.CutDown;
            
        }

        private int _BarWidthSlider;
        public int BarWidthSlider
        {
            get { return _BarWidthSlider; }
            set
            {
                _BarWidthSlider = value;
                SetBar(value);
                NotifyOfPropertyChange("BarWidthSlider");
            }
        }

        private int _RedBarIndicator;
        public int RedBarIndicator
        {
            get { return _RedBarIndicator; }
            set
            {
                _RedBarIndicator = value;
                SetColor(value, GreenBarIndicator, BlueBarIndicator);
                NotifyOfPropertyChange("RedBarIndicator");
            }
        }

        private int _GreenBarIndicator;
        public int GreenBarIndicator
        {
            get { return _GreenBarIndicator; }
            set
            {
                _GreenBarIndicator = value;
                SetColor(RedBarIndicator, value, BlueBarIndicator);
                NotifyOfPropertyChange("GreenBarIndicator");

            }
        }

        private int _BlueBarIndicator;
        public int BlueBarIndicator
        {
            get { return _BlueBarIndicator; }
            set
            {
                _BlueBarIndicator = value;
                SetColor(RedBarIndicator, GreenBarIndicator, value);
                NotifyOfPropertyChange("BlueBarIndicator");
            }
        }


        private bool _CutDownSelector;
        public bool CutDownSelector
        {
            get { return _CutDownSelector; }
            set
            {
                _CutDownSelector = value;
                if (value) SetMode(Enums.Mode.Cutdown);
                else SetMode(Enums.Mode.Immediately);
                NotifyOfPropertyChange("CutDownSelector");
            }
        }

        private bool _ComboDisable;
        public bool ComboDisable
        {
            get { return _ComboDisable; }
            private set
            {
                _ComboDisable = value;
                NotifyOfPropertyChange("ComboDisable");
            }
        }



        private bool _TurnOnOff;
        public bool TurnOnOff
        {
            get { return _TurnOnOff; }
            set
            {
                _TurnOnOff = value;
                ToggleStance(Enums.Hide.Both, value);
                if (value) ComboDisable = false;
                NotifyOfPropertyChange("TurnOnOff");
            }
        }

        private bool _LeftSide;
        public bool LeftSide
        {
            get { return _LeftSide; }
            set
            {
                _LeftSide = value;
                ToggleStance(Enums.Hide.Left, value);
                NotifyOfPropertyChange("LeftSide");
            }
        }

        private bool _RightSide;
        public bool RightSide
        {
            get { return _RightSide; }
            set
            {
                _RightSide = value;
                ToggleStance(Enums.Hide.Right, value);
                NotifyOfPropertyChange("RightSide");

            }
        }

    }
}
