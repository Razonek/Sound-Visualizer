using Caliburn.Micro;
using System;
using System.Windows.Media;

namespace Sound_Visualizer
{

    public class SpectrumBar : Screen
    {

        public SpectrumBar()
        {
            SoundVisualizerViewModel.SetBar += SetNewBarWidth;
            SoundVisualizerViewModel.SetColor += SetNewBarColor;
            this.Value = 0;
            this.Width = 0;
        }




        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                NotifyOfPropertyChange("Value");
            }
        }


        private int _Width;
        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                NotifyOfPropertyChange("Width");
            }
        }

        private Brush _BarColor;
        public Brush BarColor
        {
            get { return _BarColor; }
            private set
            {
                _BarColor = value;
                NotifyOfPropertyChange("BarColor");
            }
        }



        public void SetNewBarWidth(int Value)
        {
            Width = Value;
        }

        public void SetNewBarColor(string Value)
        {            
            BarColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Value));
        }


    }
}
