// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

using System.Runtime.InteropServices;
using LibreHardwareMonitor.Interop;

namespace LibreHardwareMonitor.Hardware.Memory
{
    internal sealed class GenericWindowsMemory : Hardware
    {
        const float GIGABYTE = 1024f * 1024f * 1024f;

        private Sensor _usedLoadSensor;
        private Sensor _freeLoadSensor;
        private Sensor _totalMemory;
        private Sensor _availableMemory;
        private Sensor _usedMemory;
        private Sensor _usedSwapLoad;
        private Sensor _freeSwapLoad;
        private Sensor _totalSwap;
        private Sensor _availableSwap;
        private Sensor _usedSwap;

        public GenericWindowsMemory(string name, ISettings settings) : base(name, new Identifier("ram"), settings)
        {
            _usedLoadSensor = new Sensor("Used RAM %", 0, SensorType.Load, this, settings);
            ActivateSensor(_usedLoadSensor);

            _freeLoadSensor = new Sensor("Free RAM %", 2, SensorType.Load, this, settings);
            ActivateSensor(_freeLoadSensor);

            _totalMemory = new Sensor("Total RAM", 0, SensorType.Data, this,
                settings);
            ActivateSensor(_totalMemory);

            _availableMemory = new Sensor("Available RAM", 1, SensorType.Data, this,
              settings);
            ActivateSensor(_availableMemory);

            _usedMemory = new Sensor("Used RAM", 4, SensorType.Data, this,
            settings);
            ActivateSensor(_usedMemory);

            _usedSwapLoad = new Sensor("Used Swap %", 1, SensorType.Load, this, settings);
            ActivateSensor(_usedSwapLoad);

            _freeSwapLoad = new Sensor("Free Swap %", 3, SensorType.Load, this, settings);
            ActivateSensor(_freeSwapLoad);

            _totalSwap = new Sensor("Total Swap", 2, SensorType.Data, this,
            settings);
            ActivateSensor(_totalSwap);

            _availableSwap = new Sensor("Available Swap", 3, SensorType.Data, this,
            settings);
            ActivateSensor(_availableSwap);

            _usedSwap = new Sensor("Used Swap", 5, SensorType.Data, this,
                settings);
            ActivateSensor(_usedSwap);
        }

        public override HardwareType HardwareType
        {
            get { return HardwareType.Memory; }
        }

        public override void Update()
        {
            Kernel32.MEMORYSTATUSEX status = new() { dwLength = (uint)Marshal.SizeOf<Kernel32.MEMORYSTATUSEX>() };

            if (!Kernel32.GlobalMemoryStatusEx(ref status))
                return;

            /*_physicalMemoryUsed.Value = (float)(status.ullTotalPhys - status.ullAvailPhys) / (1024 * 1024 * 1024);
            _physicalMemoryAvailable.Value = (float)status.ullAvailPhys / (1024 * 1024 * 1024);
            _physicalMemoryLoad.Value = 100.0f - ((100.0f * status.ullAvailPhys) / status.ullTotalPhys);

            _virtualMemoryUsed.Value = (float)(status.ullTotalPageFile - status.ullAvailPageFile) / (1024 * 1024 * 1024);
            _virtualMemoryAvailable.Value = (float)status.ullAvailPageFile / (1024 * 1024 * 1024);
            _virtualMemoryLoad.Value = 100.0f - ((100.0f * status.ullAvailPageFile) / status.ullTotalPageFile);*/
            _freeLoadSensor.Value = (100.0f * status.ullAvailPhys) / status.ullTotalPhys;
            _usedLoadSensor.Value = 100.0f - _freeLoadSensor.Value;

            _totalMemory.Value = (float)(status.ullTotalPhys) / GIGABYTE;

            _availableMemory.Value = (float)status.ullAvailPhys / GIGABYTE;

            _usedMemory.Value = _totalMemory.Value - _availableMemory.Value;

            ulong swapTotal = 0;
            ulong swapFree = 0;
            if (status.ullTotalPageFile < status.ullTotalPhys)
            {
                swapTotal = 0;
                swapFree = 0;
            }
            else
            {
                swapTotal = status.ullTotalPageFile - status.ullTotalPhys;
            }
            if ((status.ullAvailPageFile < status.ullAvailPhys) || (swapFree > swapTotal))
            {
                swapFree = swapTotal;
            }
            else
            {
                swapFree = status.ullAvailPageFile - status.ullAvailPhys;
            }

            if (swapTotal == 0)
            {
                _usedSwapLoad.Value = 0L;
                _freeSwapLoad.Value = 0L;
            }
            else
            {
                _freeSwapLoad.Value = (100.0f * swapFree) / swapTotal;
                _usedSwapLoad.Value = 100.0f - _freeSwapLoad.Value;

            }



            _totalSwap.Value = (float)(swapTotal) / GIGABYTE;

            _availableSwap.Value = (float)(swapFree) / GIGABYTE;

            if (_availableSwap.Value > _totalSwap.Value)
            {
                _availableSwap.Value = _totalSwap.Value;
                _usedSwapLoad.Value = 0L;
            }

            _usedSwap.Value = _totalSwap.Value - _availableSwap.Value;

        }
    }
}
