using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace Sound_Visualizer
{
    public delegate void Send(List<byte> Data);


    public class Analyzer
    {
        private bool _Enable;               //_Enabled status
        private DispatcherTimer DisplayTimer;         //timer that refreshes the display
        private float[] fft;               //buffer for fft data

        private WASAPIPROC process;        //callback function to obtain data
        private int lastlevel;             //last output level
        private int hanctr;                //last output level counter
        private List<byte> spectrumdata;   //spectrum data buffer        
        private List<string> devicelist;       //device list                           ////////////////// <------------
        private bool initialized;          //initialized flag
        private int devindex { get; set; }               //used device index

        private int SpectrumBarsCount { get; set; }      // number of spectrum lines
        public static Send SendValues;
        public static GetDeviceList SendList;
        
        public Analyzer(int SpectrumBarsCount, int DisplayRefreshRate)
        {
            this.SpectrumBarsCount = SpectrumBarsCount;
            fft = new float[1024];
            lastlevel = 0;
            hanctr = 0;
            DisplayTimer = new DispatcherTimer();
            DisplayTimer.Tick += DisplayTimerTick;
            DisplayTimer.Interval = TimeSpan.FromMilliseconds(DisplayRefreshRate); 
            DisplayTimer.IsEnabled = false;            
            process = new WASAPIPROC(Process);
            spectrumdata = new List<byte>();
            SoundVisualizerViewModel.ToggleStance += TurnOnOff;
            SoundVisualizerViewModel.SetDevice += SetDevice;
            initialized = false;
            devicelist = new List<string>();
            Init();
            

        }

        public void SetDevice(int Device)
        {
            if (Enable)
            {                                
                Enable = false;
                Free();
                devindex = Device;
                Enable = true;
            }
            else
                devindex = Device;
            
            
        }
        
        public void TurnOnOff(Enums.Hide Hide, bool Value)
        {
            if(Hide == Enums.Hide.Both) Enable = Value;
        }

        // flag for display Enable
        public bool DisplayEnable { get; set; }

        //flag for enabling and disabling program functionality
        public bool Enable
        {
            get { return _Enable; }
            set
            {
                _Enable = value;
                if (value)
                {
                    if (!initialized)
                    {
                       
                        bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, process, IntPtr.Zero);
                        if (!result)
                        {
                            var error = Bass.BASS_ErrorGetCode();
                            MessageBox.Show(error.ToString());
                        }
                        else
                        {
                            initialized = true;
                            
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else BassWasapi.BASS_WASAPI_Stop(true);                
                DisplayTimer.IsEnabled = value;
            }
        }

        // initialization
        private void Init()
        {
            bool result = false;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    devicelist.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            SendList(devicelist);
            
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }

        //timer 
        private void DisplayTimerTick(object sender, EventArgs e)
        {
            int ret = BassWasapi.BASS_WASAPI_GetData(fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
            if (ret < -1) return;
            int x, y;
            int b0 = 0;

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x = 0; x < SpectrumBarsCount; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (SpectrumBarsCount - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < fft[1 + b0]) peak = fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (y > 255) y = 255;
                if (y < 0) y = 0;
                spectrumdata.Add((byte)y);
                
            }

            SendValues(spectrumdata);           
            spectrumdata.Clear();


            int level = BassWasapi.BASS_WASAPI_GetLevel();
            
            if (level == lastlevel && level != 0) hanctr++;
            lastlevel = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            if (hanctr > 3)
            {
                hanctr = 0;                
                Free();
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                initialized = false;                
                Enable = true;
            }
        }

        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }
    }
}

