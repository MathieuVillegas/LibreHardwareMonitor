﻿// Mozilla Public License 2.0
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors
// All Rights Reserved

using System.Collections.Generic;
using OpenHardwareMonitor.Interop;

namespace OpenHardwareMonitor.Hardware.HDD {
  public class SmartAttribute {
    private readonly RawValueConversion rawValueConversion;

    public delegate float RawValueConversion(byte[] rawValue, byte value, IReadOnlyList<IParameter> parameters);

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartAttribute" /> class.
    /// </summary>
    /// <param name="identifier">The SMART identifier of the attribute.</param>
    /// <param name="name">The name of the attribute.</param>
    public SmartAttribute(byte identifier, string name) :
      this(identifier, name, null, null, 0, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartAttribute" /> class.
    /// </summary>
    /// <param name="identifier">The SMART identifier of the attribute.</param>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="rawValueConversion">
    /// A delegate for converting the raw byte
    /// array into a value (or null to use the attribute value).
    /// </param>
    public SmartAttribute
    (
      byte identifier,
      string name,
      RawValueConversion rawValueConversion) :
      this(identifier, name, rawValueConversion, null, 0, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartAttribute" /> class.
    /// </summary>
    /// <param name="identifier">The SMART identifier of the attribute.</param>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="rawValueConversion">
    /// A delegate for converting the raw byte
    /// array into a value (or null to use the attribute value).
    /// </param>
    /// <param name="sensorType">
    /// Type of the sensor or null if no sensor is to
    /// be created.
    /// </param>
    /// <param name="sensorChannel">
    /// If there exists more than one attribute with
    /// the same sensor channel and type, then a sensor is created only for the
    /// first attribute.
    /// </param>
    /// <param name="sensorName">
    /// The name to be used for the sensor, or null if
    /// no sensor is created.
    /// </param>
    /// <param name="defaultHiddenSensor">True to hide the sensor initially.</param>
    /// <param name="parameterDescriptions">
    /// Description for the parameters of the sensor
    /// (or null).
    /// </param>
    public SmartAttribute
    (
      byte identifier,
      string name,
      RawValueConversion rawValueConversion,
      SensorType? sensorType,
      int sensorChannel,
      string sensorName,
      bool defaultHiddenSensor = false,
      ParameterDescription[] parameterDescriptions = null) {
      Identifier = identifier;
      Name = name;
      this.rawValueConversion = rawValueConversion;
      SensorType = sensorType;
      SensorChannel = sensorChannel;
      SensorName = sensorName;
      DefaultHiddenSensor = defaultHiddenSensor;
      ParameterDescriptions = parameterDescriptions;
    }

    public bool DefaultHiddenSensor { get; }

    public bool HasRawValueConversion => rawValueConversion != null;

    /// <summary>
    /// Gets the SMART identifier.
    /// </summary>
    public byte Identifier { get; }

    public string Name { get; }

    public ParameterDescription[] ParameterDescriptions { get; }

    public int SensorChannel { get; }

    public string SensorName { get; }

    public SensorType? SensorType { get; }

    internal float ConvertValue(Kernel32.SMART_ATTRIBUTE value, IReadOnlyList<IParameter> parameters) {
      if (rawValueConversion == null) {
        return value.CurrentValue;
      }

      return rawValueConversion(value.RawValue, value.CurrentValue, parameters);
    }
  }
}