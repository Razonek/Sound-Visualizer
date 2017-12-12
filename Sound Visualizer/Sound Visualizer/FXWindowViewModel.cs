using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sound_Visualizer
{
    class FXWindowViewModel : Screen
    {

        private Enums.Mode Mode { get; set; }
        private List<byte> SpectrumData;
        private List<byte> CopyOfSpectrumData;
        private int TickCounter { get; set; }
        private List<byte> FirstSpectrum { get; set; }
        private List<byte> SecondSpectrum { get; set; }
        private List<byte> ThirdSpectrum { get; set; }        

        public int RefreshTime { get; set; }

        Analyzer Sound;
        DispatcherTimer Timer;


        #region Left Spectrum bars
        private ObservableCollection<SpectrumBar> _SpectrumBarsLeft;
        public ObservableCollection<SpectrumBar> SpectrumBarsLeft
        {
            get { return _SpectrumBarsLeft; }
            set
            {
                _SpectrumBarsLeft = value;
                NotifyOfPropertyChange("SpectrumBarsLeft");
            }
        }
        #endregion

        #region Right Spectrum bars
        private ObservableCollection<SpectrumBar> _SpectrumBarsRight;
        public ObservableCollection<SpectrumBar> SpectrumBarsRight
        {
            get { return _SpectrumBarsRight; }
            set
            {
                _SpectrumBarsRight = value;
                NotifyOfPropertyChange("SpectrumBarsRight");
            }
        }
        #endregion

        #region Screen Height
        private int _ScreenHeight;
        public int ScreenHeight
        {
            get { return _ScreenHeight; }
            private set
            {
                _ScreenHeight = value;
                NotifyOfPropertyChange("ScreenHeight");
            }
        }
        #endregion

        #region Screen Width
        private int _ScreenWidth;
        public int ScreenWidth
        {
            get { return _ScreenWidth; }
            private set
            {
                _ScreenWidth = value;
                NotifyOfPropertyChange("ScreenWidth");
            }
        }
        #endregion

        #region Visibility LeftSide
        private Visibility _LeftSide;
        public Visibility LeftSide
        {
            get { return _LeftSide; }
            private set
            {
                _LeftSide = value;
                NotifyOfPropertyChange("LeftSide");
            }
        }
        #endregion

        #region Visibility RightSide
        private Visibility _RightSide;
        public Visibility RightSide
        {
            get { return _RightSide; }
            private set
            {
                _RightSide = value;
                NotifyOfPropertyChange("RightSide");
            }
        }
        #endregion

        public FXWindowViewModel()
        {
            this.ScreenHeight = Overlay.ScreenHeight();
            this.ScreenWidth = Overlay.ScreenWidth();

            SpectrumBarsLeft = new ObservableCollection<SpectrumBar>();
            SpectrumBarsRight = new ObservableCollection<SpectrumBar>();
            SpectrumData = new List<byte>();
            CopyOfSpectrumData = new List<byte>();
            FirstSpectrum = new List<byte>();
            SecondSpectrum = new List<byte>();
            ThirdSpectrum = new List<byte>();
            

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            Timer.Tick += TimerTick;

            TickCounter = 0;  
            RefreshTime = 10;       
                                                         
            Sound = new Analyzer(MakeSpectrumBars(), RefreshTime);

            SoundVisualizerViewModel.ToggleStance += TurnOnOff;            
            SoundVisualizerViewModel.SetMode += SetSpectrumMode;
            Analyzer.SendValues += UpdateSpectum;
            
           
        }

        #region Turn On and turn off bars
        public void TurnOnOff(Enums.Hide Hide, bool Value)
        {
            switch(Hide)
            {
                case Enums.Hide.Both:
                    if(!Value)
                    {
                        Timer.IsEnabled = false;
                        for (int i = 0; i < SpectrumBarsLeft.Count; i++)
                        {
                            SpectrumBarsLeft[i].Value = SpectrumBarsRight[i].Value = 0;
                        }
                    }
                    
                    break;

                case Enums.Hide.Left:
                    if (Value) LeftSide = Visibility.Visible; else LeftSide = Visibility.Hidden;
                    break;

                case Enums.Hide.Right:
                    if (Value) RightSide = Visibility.Visible; else RightSide = Visibility.Hidden;
                    break;

                case Enums.Hide.Close:
                    Timer.IsEnabled = false;
                    this.TryClose();
                    break;

            }
            
        }
        #endregion      

        #region Change Spectrum mode
        private void SetSpectrumMode(Enums.Mode Mode)
        {
            this.Mode = Mode;
        }
        #endregion

        #region Timer Tick
        private void TimerTick(object sender, EventArgs e)
        {
            if (TickCounter == 0)
            {
                CopyOfSpectrumData.Clear();
                CopyOfSpectrumData.AddRange(SpectrumData);
                for (int i = 0; i < CopyOfSpectrumData.Count; i++)
                {
                    SpectrumBarsLeft[i].Value = CopyOfSpectrumData[i];
                    SpectrumBarsRight[i].Value = CopyOfSpectrumData[i];
                }
            }

            else
            {
                for (int i = 0; i < CopyOfSpectrumData.Count; i++)
                {
                    SpectrumBarsLeft[i].Value -= 3;
                    SpectrumBarsRight[i].Value -= 3;
                }
            }

            TickCounter++;
            if (TickCounter == 3) TickCounter = 0;
                
        }
        #endregion

        #region Update Spectrum bars
        /// <summary>
        /// Updating spectrum bars with new data
        /// </summary>
        /// <param name="Data"> New part of value from analyzer </param>
        public void UpdateSpectum(List<byte> Data)
        {
            switch(Mode)
            {

                case Enums.Mode.Immediately:
                    if (Timer.IsEnabled) Timer.IsEnabled = false;
                    for (int i = 0; i < Data.Count; i++)
                    {
                        SpectrumBarsLeft[i].Value = Data[i];
                        SpectrumBarsRight[i].Value = Data[i];
                    }
                    break;


                case Enums.Mode.Cutdown:
                    if (!Timer.IsEnabled) Timer.IsEnabled = true;
                    SpectrumData.Clear();
                    SpectrumData.AddRange(Data);
                    break;

                case Enums.Mode.Average:
                    if (Timer.IsEnabled) Timer.IsEnabled = false;
                    if (FirstSpectrum.Count == 0) FirstSpectrum.AddRange(Data);
                    else if (SecondSpectrum.Count == 0) SecondSpectrum.AddRange(Data);
                    else ThirdSpectrum.AddRange(Data);
                   

                    if(FirstSpectrum.Count != 0 && SecondSpectrum.Count != 0 && ThirdSpectrum.Count != 0)
                    {
                        for (int i = 0; i < Data.Count; i++)
                        {
                            int value = (FirstSpectrum[i] + SecondSpectrum[i] + ThirdSpectrum[i] ) / 3;
                            SpectrumBarsLeft[i].Value = value;
                            SpectrumBarsRight[i].Value = value;                            
                        }
                        FirstSpectrum.Clear();
                        SecondSpectrum.Clear();
                        ThirdSpectrum.Clear();
                       
                    }
                    

                    break;
            }
            
        }
        #endregion       

        #region Make and return spectrum bars/count
        /// <summary>
        /// Making spectrum bars
        /// </summary>
        /// <returns> Returning count of bars on each side. Analyzer need to know how much data need to collect </returns>
        private int MakeSpectrumBars()
        {
            int barsCount = ScreenHeight / 4;
            for(int i = 0; i < barsCount; i++)
            {
                SpectrumBarsLeft.Add(new SpectrumBar());
                SpectrumBarsRight.Add(new SpectrumBar());
            }
            return barsCount;
        }
        #endregion

        #region Bars Height/Width?
        public void SetBarsHeight(int Value)
        {
            for(int i = 0;i<SpectrumBarsLeft.Count;i++)
            {
                SpectrumBarsLeft[i].Width = Value;
                SpectrumBarsRight[i].Width = Value;
            }
        }
        #endregion






    }




}
