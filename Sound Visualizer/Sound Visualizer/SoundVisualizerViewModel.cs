using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;

namespace Sound_Visualizer
{

    ///////////////
    ///Delegates///
    /////////////// 

    public delegate void SetBarWidth(int Width);
    public delegate void SetBarColor(string Color);
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
            ModeSelectorList = new List<Enums.Mode>(new Enums.Mode[] { Enums.Mode.Immediately, Enums.Mode.Average, Enums.Mode.Cutdown });
            Analyzer.SendList += SetDeviceList;
            Window.ShowWindow(new FXWindowViewModel());
            LoadSettings();                        
            ChangeContent(Enums.Grids.General);
            TurnOnOff = false;
            
            


        }

        protected override void OnDeactivate(bool close)
        {
            ToggleStance(Enums.Hide.Close, true);
            SaveSettings();
            base.OnDeactivate(close);
        }




        private void ChangeContent(Enums.Grids GridToShow)
        {
            GeneralGrid = Visibility.Hidden;
            ColorSelectorGrid = Visibility.Hidden;
            SoundDeviceGrid = Visibility.Hidden;
            IsCheckedColor = false;
            IsCheckedGeneral = false;
            IsCheckedSoundDevice = false;

           switch(GridToShow)
            {
                case Enums.Grids.General:
                    IsCheckedGeneral = true;
                    GeneralGrid = Visibility.Visible;
                    break;

                case Enums.Grids.Color:
                    IsCheckedColor = true;
                    ColorSelectorGrid = Visibility.Visible;
                    break;

                case Enums.Grids.SoundDevice:
                    IsCheckedSoundDevice = true;
                    SoundDeviceGrid = Visibility.Visible;
                    break;

                    
            }
        }


        public void GeneralGridToggleButton()
        {
            ChangeContent(Enums.Grids.General);
        }

        public void ColorSelectorGridToggleButton()
        {
            ChangeContent(Enums.Grids.Color);
        }

        public void SoundDeviceGridToggleButton()
        {
            ChangeContent(Enums.Grids.SoundDevice);
        }

        private Visibility _GeneralGrid;
        public Visibility GeneralGrid
        {
            get { return _GeneralGrid; }
            private set
            {
                _GeneralGrid = value;
                NotifyOfPropertyChange("GeneralGrid");
            }
        }

        private Visibility _ColorSelectorGrid;
        public Visibility ColorSelectorGrid
        {
            get { return _ColorSelectorGrid; }
            private set
            {
                _ColorSelectorGrid = value;
                NotifyOfPropertyChange("ColorSelectorGrid");
            }
        }

        private Visibility _SoundDeviceGrid;
        public Visibility SoundDeviceGrid
        {
            get { return _SoundDeviceGrid; }
            private set
            {
                _SoundDeviceGrid = value;
                NotifyOfPropertyChange("SoundDeviceGrid");
            }
        }


        private bool _IsCheckedGeneral;
        public bool IsCheckedGeneral
        {
            get { return _IsCheckedGeneral; }
            private set
            {
                _IsCheckedGeneral = value;
                NotifyOfPropertyChange("IsCheckedGeneral");
            }
        }

        private bool _IsCheckedColor;
        public bool IsCheckedColor
        {
            get { return _IsCheckedColor; }
            private set
            {
                _IsCheckedColor = value;
                NotifyOfPropertyChange("IsCheckedColor");
            }
        }

        private bool _IsCheckedSoundDevice;
        public bool IsCheckedSoundDevice
        {
            get { return _IsCheckedSoundDevice; }
            private set
            {
                _IsCheckedSoundDevice = value;
                NotifyOfPropertyChange("IsCheckedSoundDevice");
            }
        }



        private List<Enums.Mode> _ModeSelectorList;
        public List<Enums.Mode> ModeSelectorList
        {
            get { return _ModeSelectorList; }
            set
            {
                _ModeSelectorList = value;
                NotifyOfPropertyChange("ModeSelectorList");
            }
        }

        private Enums.Mode _SelectedModeSelectorList;
        public Enums.Mode SelectedModeSelectorList
        {
            get { return _SelectedModeSelectorList; }
            set
            {
                _SelectedModeSelectorList = value;
                SetMode(value);
                NotifyOfPropertyChange("SelectedModeSelectorList");
            }
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
            Properties.Settings.Default.CutDown = CutDownSelector;
            Properties.Settings.Default.Color = SelectedColor;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Load Settings
        private void LoadSettings()
        {
            BarWidthSlider = Properties.Settings.Default.Width;
            LeftSide = Properties.Settings.Default.LeftBar;
            RightSide = Properties.Settings.Default.RightBar;
            if (Properties.Settings.Default.Color == "0") SelectedColor = "#FF000000";
            else SelectedColor = Properties.Settings.Default.Color;
            CutDownSelector = Properties.Settings.Default.CutDown;            
        }
        #endregion

        
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

        #region Bar Color
        private string _SelectedColor;
        public string SelectedColor
        {
            get { return _SelectedColor; }
            set
            {
                _SelectedColor = value;
                SetColor(value);
                NotifyOfPropertyChange("SelectedColor");
            }
        }
       
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

        

        #region Enable/Disable 
        private bool _TurnOnOff;
        public bool TurnOnOff
        {
            get { return _TurnOnOff; }
            set
            {
                _TurnOnOff = value;
                ToggleStance(Enums.Hide.Both, value);
                if (value) AppState = "Enable";                
                else AppState = "Disable";                
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
