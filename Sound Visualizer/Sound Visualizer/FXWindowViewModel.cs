using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using Un4seen.BassAsio;
using System.Windows.Media;
using System.Drawing;
using Caliburn.Micro;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sound_Visualizer
{
    class FXWindowViewModel : Screen
    {


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


        public int RefreshTime { get; set; }
        Analyzer Sound;
        private Enums.Mode Mode { get; set; }
        private List<byte> SpectrumData;
        private List<byte> CopyOfSpectrumData;
        DispatcherTimer Timer;
        private int TickCounter { get; set; }



        public FXWindowViewModel()
        {
            this.ScreenHeight = Overlay.ScreenHeight();
            this.ScreenWidth = Overlay.ScreenWidth();
            SpectrumBarsLeft = new ObservableCollection<SpectrumBar>();
            SpectrumBarsRight = new ObservableCollection<SpectrumBar>();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            Timer.Tick += TimerTick;
            TickCounter = 0;
            
            SpectrumData = new List<byte>();
            CopyOfSpectrumData = new List<byte>();
            RefreshTime = 10;                                                    //100hz
            Sound = new Analyzer(MakeSpectrumBars(), RefreshTime);
            SoundVisualizerViewModel.ToggleStance += TurnOnOff;
            
            SoundVisualizerViewModel.SetMode += SetSpectrumMode;
            Analyzer.SendValues += UpdateSpectum;
            Mode = Enums.Mode.Immediately;
                       
            
            

        }

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



        private void SetSpectrumMode(Enums.Mode Mode)
        {
            this.Mode = Mode;
        }

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
            }
            
        }
        

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

        public void SetBarsHeight(int Value)
        {
            for(int i = 0;i<SpectrumBarsLeft.Count;i++)
            {
                SpectrumBarsLeft[i].Width = Value;
                SpectrumBarsRight[i].Width = Value;
            }
        }







    }
 
      


}
