/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2012 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System.Runtime.InteropServices;

namespace OpenHardwareMonitor.Hardware.RAM {
  internal class GenericRAM : Hardware {

    const float GIGABYTE = 1024f * 1024f * 1024f;

    private Sensor usedLoadSensor;
    private Sensor freeLoadSensor;
    private Sensor totalMemory;
    private Sensor availableMemory;
    private Sensor usedMemory;
    private Sensor usedSwapLoad;
    private Sensor freeSwapLoad;
    private Sensor totalSwap;
    private Sensor availableSwap;
    private Sensor usedSwap;

        public GenericRAM(string name, ISettings settings)
      : base(name, new Identifier("ram"), settings)
    {   
      usedLoadSensor = new Sensor("Used RAM %", 0, SensorType.Load, this, settings);
      ActivateSensor(usedLoadSensor);

        freeLoadSensor = new Sensor("Free RAM %", 2, SensorType.Load, this, settings);
    ActivateSensor(freeLoadSensor);

    totalMemory = new Sensor("Total RAM", 0, SensorType.Data, this,
        settings);
      ActivateSensor(totalMemory);

      availableMemory = new Sensor("Available RAM", 1, SensorType.Data, this, 
        settings);
      ActivateSensor(availableMemory);

        usedMemory = new Sensor("Used RAM", 4, SensorType.Data, this,
        settings);
            ActivateSensor(usedMemory);

        usedSwapLoad = new Sensor("Used Swap %", 1, SensorType.Load, this, settings);
        ActivateSensor(usedSwapLoad);

        freeSwapLoad = new Sensor("Free Swap %", 3, SensorType.Load, this, settings);
        ActivateSensor(freeSwapLoad);

            totalSwap = new Sensor("Total Swap", 2, SensorType.Data, this,
            settings);
        ActivateSensor(totalSwap);

            availableSwap = new Sensor("Available Swap", 3, SensorType.Data, this,
            settings);
        ActivateSensor(availableSwap);

        usedSwap = new Sensor("Used Swap", 5, SensorType.Data, this,
            settings);
            ActivateSensor(usedSwap);
        }

    public override HardwareType HardwareType {
      get {
        return HardwareType.RAM;
      }
    }

    public override void Update() {
      NativeMethods.MemoryStatusEx status = new NativeMethods.MemoryStatusEx();
      status.Length = checked((uint)Marshal.SizeOf(
          typeof(NativeMethods.MemoryStatusEx)));

      if (!NativeMethods.GlobalMemoryStatusEx(ref status))
        return;

      freeLoadSensor.Value = (100.0f * status.AvailablePhysicalMemory) / status.TotalPhysicalMemory;
      usedLoadSensor.Value = 100.0f - freeLoadSensor.Value;

      totalMemory.Value = (float)(status.TotalPhysicalMemory) / GIGABYTE;

      availableMemory.Value = (float)status.AvailablePhysicalMemory / GIGABYTE;

      usedMemory.Value = totalMemory.Value - availableMemory.Value;

            ulong swapTotal = 0;
        ulong swapFree = 0;
        if (status.TotalPageFile < status.TotalPhysicalMemory)
        {
            swapTotal = 0;
            swapFree = 0;
        }
        else
        {
            swapTotal = status.TotalPageFile - status.TotalPhysicalMemory;
        }
        if ((status.AvailPageFile < status.AvailablePhysicalMemory) || (swapFree > swapTotal))
        {
            swapFree = swapTotal;
        }
        else
        {
            swapFree = status.AvailPageFile - status.AvailablePhysicalMemory;
        }

        if(swapTotal == 0)
        {
             usedSwapLoad.Value = 0L;
             freeSwapLoad.Value = 0L;
        }
        else
        {
            freeSwapLoad.Value = (100.0f * swapFree) / swapTotal;
            usedSwapLoad.Value = 100.0f - freeSwapLoad.Value;

        }

       

        totalSwap.Value = (float)(swapTotal) / GIGABYTE;

        availableSwap.Value = (float)(swapFree) / GIGABYTE;

        if (availableSwap.Value > totalSwap.Value)
        {
            availableSwap.Value = totalSwap.Value;
            usedSwapLoad.Value = 0L;
        }

        usedSwap.Value = totalSwap.Value - availableSwap.Value;

    }

    private class NativeMethods {
      [StructLayout(LayoutKind.Sequential)]
      public struct MemoryStatusEx {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhysicalMemory;
        public ulong AvailablePhysicalMemory;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;
      }

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GlobalMemoryStatusEx(
        ref NativeMethods.MemoryStatusEx buffer);
    }
  }
}
