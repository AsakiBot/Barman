using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barman_PC.Core
{
    public class Manager
    {
        public Bluetooth.BluetoothManager BluetoothManager { get; private set; }
        public Manager()
        {
            BluetoothManager = new Bluetooth.BluetoothManager(this);
        }
    }
}
