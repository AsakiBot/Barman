using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Barman_PC.Core.Bluetooth
{
    public class BluetoothManager
    {
        public BluetoothDeviceInfo[] Devices { get; private set; }
        public BluetoothDeviceInfo CurrentDevice { get; set; }
        public BluetoothClient Client { get; private set; }
        public string Message { get; set; }

        private Manager _manager;

        public event Action Connected;
        public event Action Disconnected;
        public event Action DevicesFounded;
        public event Action<string> DataReceived;
        
        public BluetoothManager(Manager manager)
        {
            _manager = manager;
            Message = "-1";
        }

        private void startSearchingDevice()
        {
            BluetoothClient client = new BluetoothClient();
            Devices = client.DiscoverDevicesInRange();
            DevicesFounded?.Invoke();
        }

        public void StartConnection()
        {
            if (pairDevice())
            {
                Connected?.Invoke();
                Thread bluetoothClientThread = new Thread(new ThreadStart(ClientConnectThread));
                bluetoothClientThread.Start();

            }
            else
            {
                Disconnected?.Invoke();
            }
        }

        public void Scan()
        {
            Thread bluetoothScanThread = new Thread(new ThreadStart(startSearchingDevice));
            bluetoothScanThread.Start();
        }

        private void ClientConnectThread()
        {
            Client = new BluetoothClient();

            Client.BeginConnect(CurrentDevice.DeviceAddress, mUUID, this.BluetoothClientConnectCallback, Client);

        }

        private void BluetoothClientConnectCallback(IAsyncResult result)
        {
            BluetoothClient client = (BluetoothClient)result.AsyncState;
            client.EndConnect(result);

            var stream = client.GetStream();
            stream.ReadTimeout = 1000;
            stream.WriteTimeout = 1000;

            bool first = true;

            while (true)
            {
                Thread.Sleep(250);
                if (first)
                {
                    first = false;
                    var message = Encoding.ASCII.GetBytes("a");
                    stream.Write(message, 0, message.Length);
                }else if (Message != "-1")
                {
                    var message = Encoding.ASCII.GetBytes(Message);
                    Message = "-1";
                    stream.Write(message, 0, message.Length);
                }
                
              if (stream.DataAvailable) {
                    byte[] bytes = new byte[1];

                    int n = stream.Read(bytes, 0, 1);

                    DataReceived?.Invoke(Encoding.ASCII.GetString(bytes));
                }
            }
        }


        Guid mUUID = new Guid("00001101-0000-1000-8000-00805F9B34FB");
        bool ready = false;
        string myPin = "1234";
        byte[] message;

        private bool pairDevice()
        {
            if (!CurrentDevice.Authenticated)
            {
                if (!BluetoothSecurity.PairRequest(CurrentDevice.DeviceAddress, myPin))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
