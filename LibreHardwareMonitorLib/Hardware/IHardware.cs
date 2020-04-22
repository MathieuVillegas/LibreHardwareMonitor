﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

namespace LibreHardwareMonitor.Hardware
{
    public delegate void SensorEventHandler(ISensor sensor);

    public enum HardwareType
    {
        Motherboard,
        SuperIO,
        AquaComputer,
        Cpu,
        Memory,
        GpuNvidia,
        GpuAmd,
        TBalancer,
        Heatmaster,
        Storage,
        Network
    }

    public interface IHardware : IElement
    {
        HardwareType HardwareType { get; }

        Identifier Identifier { get; }

        string Name { get; set; }

        IHardware Parent { get; }

        ISensor[] Sensors { get; }

        IHardware[] SubHardware { get; }

        string GetReport();

        void Update();

        event SensorEventHandler SensorAdded;

        event SensorEventHandler SensorRemoved;
    }
}
