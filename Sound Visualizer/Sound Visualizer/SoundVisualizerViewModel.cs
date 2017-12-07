using System;
using Caliburn.Micro;
using System.Collections.Generic;

namespace Sound_Visualizer
{

    ///////////////
    ///Delegates///
    /////////////// 

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
            TurnOnOff = false;


        }

        protected override void OnDeactivate(bool close)
        {
            ToggleStance(Enums.Hide.Close, true);
            SaveSettings();
            base.OnDeactivate(close);
        }


        #region Set Device List
        public void SetDeviceList(List<string> List)
        {
            AudioOutput.Clear();
            foreach(string device in List)
            {
                AudioOutput.Add(device);
            }
        }
        #endregion

        #region Audio Output selected item
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
        #endregion

        #region Audio Output index
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
        #endregion

        #region Audio Output list
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
        #endregion       

        #region Save settings
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
        #endregion

        #region Load Settings
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
        #endregion

        #region Bars
        #region Bar Width
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
        #endregion

        #region Red Color Bar
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
        #endregion

        #region Green Color Bar
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
        #endregion

        #region Blue Color Bar
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
        #endregion
        #endregion

        #region Cut Down toggle button
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
        #endregion

        #region ComboBox Disable
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
        #endregion

        #region Enable/Disable 
        private bool _TurnOnOff;
        public bool TurnOnOff
        {
            get { return _TurnOnOff; }
            set
            {
                _TurnOnOff = value;
                ToggleStance(Enums.Hide.Both, value);
                if (value)
                {
                    ComboDisable = false;
                    AppState = "Disable";
                }
                else AppState = "Enable";                
                NotifyOfPropertyChange("TurnOnOff");
            }
        }
        #endregion

        #region Sides
        #region Left Side
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
        #endregion

        #region Right Side
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
        #endregion
        #endregion

        #region Application State on button 
        private string _AppState;
        public string AppState
        {
            get { return _AppState; }
            private set
            {
                _AppState = value;
                NotifyOfPropertyChange("AppState");
            }
        }
        #endregion

    }
}
