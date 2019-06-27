using Barman_PC.Core;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Barman
{
    public partial class Main : Form
    {
        private Manager _manager;
        private BluetoothDeviceInfo _deviceInfo;

        public Main()
        {
            InitializeComponent();
            _manager = new Manager();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            _manager.BluetoothManager.DevicesFounded += BluetoothManager_DevicesFounded;
            _manager.BluetoothManager.Scan();
        }

        private void BluetoothManager_DevicesFounded()
        {
            _manager.BluetoothManager.DevicesFounded -= BluetoothManager_DevicesFounded;

            listBox1.BeginInvoke(new Action(() =>
            {
                listBox1.Items.Clear();
                foreach (var i in _manager.BluetoothManager.Devices)
                {
                    if (!i.Connected)
                        listBox1.Items.Add(i.DeviceName);
                }
            }));
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            _manager.BluetoothManager.Connected += BluetoothManager_Connected;
            _manager.BluetoothManager.Disconnected += BluetoothManager_Disconnected;
            _manager.BluetoothManager.CurrentDevice = _manager.BluetoothManager.Devices.ElementAt(listBox1.SelectedIndex);
            _manager.BluetoothManager.StartConnection();
        }

        private void BluetoothManager_Disconnected()
        {
            Console.WriteLine("Disconnected");
        }

        private void BluetoothManager_Connected()
        {
            Console.WriteLine("Connected");
            _manager.BluetoothManager.DataReceived += BluetoothManager_DataReceived;
        }

        private void BluetoothManager_DataReceived(string value)
        {
            switch (value)
            {
                case "i": // Initialisation terminé
                    button3.BeginInvoke(new Action(() => button3.Enabled = true));
                    button4.BeginInvoke(new Action(() => button4.Enabled = true));
                    break;
                case "a":
                    button2.BeginInvoke(new Action(() => button2.Enabled = true));
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _manager.BluetoothManager.Message = "i";
            button2.BeginInvoke(new Action(() => button2.Enabled = false));
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            button4.BeginInvoke(new Action(() => button4.Enabled = false));
            _manager.BluetoothManager.Message = "l";
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            button4.BeginInvoke(new Action(() => button4.Enabled = true));
            button3.BeginInvoke(new Action(() => button3.Enabled = true));
            _manager.BluetoothManager.Message = "n";
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            button3.BeginInvoke(new Action(() => button3.Enabled = false));
            _manager.BluetoothManager.Message = "r";
        }
    }
}
