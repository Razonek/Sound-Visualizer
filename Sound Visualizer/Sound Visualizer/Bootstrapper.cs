using Caliburn.Micro;
using System.Windows;

namespace Sound_Visualizer
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<SoundVisualizerViewModel>();
        }



    }
}
