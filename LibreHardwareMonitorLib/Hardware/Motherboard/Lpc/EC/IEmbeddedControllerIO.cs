﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// All Rights Reserved.

using System;

namespace LibreHardwareMonitor.Hardware.Motherboard.Lpc.EC
{
    public interface IEmbeddedControllerIO : IDisposable
    {
        void WriteByte(byte register, byte value);

        void WriteWord(byte register, ushort value);

        byte ReadByte(byte register);

        ushort ReadWordBE(byte register);

        ushort ReadWordLE(byte register);
    }
}
