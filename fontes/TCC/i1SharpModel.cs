using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace i1Sharp
{
    class I1SharpModel
    {
        public static ObservableCollection<I1Pro> Devices { get; private set; }

        public delegate void DeviceChangedHandler (I1Pro device);
        
        public event DeviceChangedHandler DeviceChanged;

        private I1Pro currentDevice;
        
        public I1Pro CurrentDevice 
        {
            get { return currentDevice; }
            set
            {
                if (currentDevice != null)
                {
                    currentDevice.Dispose ();
                }
                currentDevice = null;
                try
                {
                    I1Pro newDevice = value;
                    newDevice.Open ();
                    currentDevice = newDevice;
                }
                catch (Exception)
                {
                }

                if (DeviceChanged != null)
                {
                    DeviceChanged (currentDevice);
                }
            }
        }

        public BackgroundWorker worker;
        
        public I1SharpModel ()
        {
            Devices = new ObservableCollection<I1Pro> ();
            I1Pro.DeviceConnected += I1Pro64_DeviceConnected;
            I1Pro.DeviceDisconnected += I1Pro64_DeviceDisconnected;
        }
        
        private void I1Pro64_DeviceConnected (I1Pro device)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke (new Action (() =>
            {
                Devices.Add (device);
            }
            ));
        }
        
        private void I1Pro64_DeviceDisconnected (I1Pro device)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke (new Action (() =>
            {
                Devices.Remove (device);
            }
            ));
        }

       public void SearchDevices()
        {
            worker = new BackgroundWorker ();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync ();
        }

        private void worker_DoWork (object sender, DoWorkEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke (new Action (() =>
                {
                    foreach (var device in I1Pro.LoadDevices ())
                    {
                        Devices.Add (device);
                    }
                }
            ));
        }
    }
}
