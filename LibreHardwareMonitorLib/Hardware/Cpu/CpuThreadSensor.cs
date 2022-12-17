// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Cpu
{
    internal class CpuThreadSensor : Sensor
    {
        private Sensor _parentCoreSensor = null;
        private static ThreadLocal<Sensor> localParentCoreSensor = new ThreadLocal<Sensor>();
        public CpuThreadSensor(
            string name,
            int index, 
            SensorType sensorType,
            Sensor parentCoreSensor,
            Hardware hardware, 
            ISettings settings): 
            base(name, index, sensorType, SetTemporaryParentCoreSensor(parentCoreSensor, hardware), settings)
        {
            if (_parentCoreSensor == null)
            {
                _parentCoreSensor = localParentCoreSensor.Value;
                localParentCoreSensor.Value = null;
            }

        }

        private static Hardware SetTemporaryParentCoreSensor(Sensor parentCoreSensor, Hardware hardware)
        {
            localParentCoreSensor.Value = parentCoreSensor;
            return hardware;
        }

        public override Identifier Identifier
        {
            get {
                if (_parentCoreSensor == null)
            {
                _parentCoreSensor = localParentCoreSensor.Value;
                localParentCoreSensor.Value = null;
            }
            return new Identifier(_parentCoreSensor.Identifier, Index.ToString(CultureInfo.InvariantCulture));
            }

        }
    }
}
