using Caliburn.Micro;
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

        private SolidColorBrush _Color;
        public SolidColorBrush Color
        {
            get { return _Color; }
            private set
            {
                _Color = value;
                NotifyOfPropertyChange("Color");
            }
        }



        public void SetNewBarWidth(int Value)
        {
            Width = Value;
        }

        public void SetNewBarColor(int R,int G,int B)
        {            
            Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)R, (byte)G, (byte)B));
        }


    }
}
