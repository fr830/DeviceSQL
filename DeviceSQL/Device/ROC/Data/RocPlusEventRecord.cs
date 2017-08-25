﻿#region Imported Types

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace DeviceSQL.Device.ROC.Data
{

    #region Enums

    public enum ROCPlusEventType : byte
    {
        NoEvent = 0,
        ParameterChangeEvent = 1,
        SystemEvent = 2,
        FSTEvent = 3,
        UserEvent = 4,
        PowerLostEvent = 5,
        ClockSetEvent = 6,
        CalibrateVerifyEvent = 7
    }

    public enum ROCPlusEventCode
    {
        InitializationSequence = 144,
        AllPowerRemoved = 145,
        Initializefromdefaults = 146,
        ROMCRCError = 147,
        DatabaseInitialization = 148,
        ProgramFlash = 150,
        ROC800Reserved1 = 151,
        ROC800Reserved2 = 152,
        ROC800Reserved3 = 153,
        SmartModuleInserted = 154,
        SmartModuleRemoved = 155,
        ClockSet = 200,
        TextMessage = 248,
        DownloadConfiguration = 249,
        UploadConfiguration = 250,
        CalibrationTimeout = 251,
        CalibrationCancel = 252,
        CalibrationSuccess = 253,
        MVSResettoFactoryDefaults = 254
    }

    #endregion

    public class ROCPlusEventRecord
    {

        #region Fields

        internal byte[] data;
        internal ushort index;

        #endregion

        #region Properties

        public DateTime? DateTimeStamp
        {
            get
            {
                var timeStamp = BitConverter.ToUInt32(data, 1);

                if (timeStamp > 0)
                {
                    return new DateTime(1970, 1, 1).AddSeconds(timeStamp);
                }
                else
                {
                    return null;
                }
            }
        }

        public ushort Index
        {
            get { return index; }
        }

        public ROCPlusEventType EventType
        {
            get
            {
                return (ROCPlusEventType)data[0];
            }
        }

        public ROCPlusEventCode? EventCode
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.SystemEvent:
                    case ROCPlusEventType.UserEvent:
                        return (ROCPlusEventCode)data[5];
                    default:
                        return null;
                }
            }
        }

        public string OperatorId
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.CalibrateVerifyEvent:
                    case ROCPlusEventType.UserEvent:
                    case ROCPlusEventType.ParameterChangeEvent:
                        return System.Text.ASCIIEncoding.Default.GetString(data, 5, 3).Replace("\0", "").Trim();
                    default:
                        return null;
                }
            }
        }

        public Tlp Tlp
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.CalibrateVerifyEvent:
                    case ROCPlusEventType.ParameterChangeEvent:
                        return new Tlp(data[8], data[9], data[10]);
                    default:
                        return null;
                }
            }
        }

        public ParameterType? DataType
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.ParameterChangeEvent:
                        return (ParameterType)data[11];
                    default:
                        return null;
                }
            }
        }

        public byte[] NewValue
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.ParameterChangeEvent:
                        return new byte[] { data[12], data[13], data[14], data[15] };
                    default:
                        return null;
                }
            }
        }

        public byte[] OldValue
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.ParameterChangeEvent:
                        return new byte[] { data[16], data[17], data[18], data[19] };
                    default:
                        return null;
                }
            }
        }

        public UInt16 Spare
        {
            get
            {
                return BitConverter.ToUInt16(new byte[] { data[20], data[21] }, 0);
            }
        }

        public string Description
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.SystemEvent:
                        return System.Text.ASCIIEncoding.Default.GetString(data, 6, 16).Replace("\0", "").Trim();
                    case ROCPlusEventType.FSTEvent:
                        return System.Text.ASCIIEncoding.Default.GetString(data, 10, 10).Replace("\0", "").Trim();
                    case ROCPlusEventType.UserEvent:
                        return System.Text.ASCIIEncoding.Default.GetString(data, 9, 13).Replace("\0", "").Trim();
                    default:
                        return null;
                }
            }
        }

        public byte? FstNumber
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.FSTEvent:
                        return data[5];
                    default:
                        return null;
                }
            }
        }

        public float? FstValue
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.FSTEvent:
                        {
                            return BitConverter.ToSingle(data, 6);
                        }
                    default:
                        return null;
                }
            }
        }

        public DateTime? DateTimeValue
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.PowerLostEvent:
                    case ROCPlusEventType.ClockSetEvent:
                        {
                            var timeStamp = BitConverter.ToUInt32(data, 5);

                            if (timeStamp > 0)
                            {
                                return new DateTime(1970, 1, 1).AddSeconds(timeStamp);
                            }
                            else
                            {
                                return null;
                            }

                        }
                    default:
                        return null;
                }
            }
        }

        public float? CalibrationRawValue
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.CalibrateVerifyEvent:
                        {
                            return BitConverter.ToSingle(data, 11);
                        }
                    default:
                        return null;
                }
            }
        }

        public float? CalibrationCalibratedValue
        {
            get
            {
                switch (EventType)
                {
                    case ROCPlusEventType.CalibrateVerifyEvent:
                        {
                            return BitConverter.ToSingle(data, 15);
                        }
                    default:
                        return null;
                }
            }
        }

        #endregion

        #region Constructor(s)

        internal ROCPlusEventRecord(ushort index, byte[] data)
        {

            if (data == null)
            {
                throw new ArgumentNullException();
            }

            if (data.Length != 22)
            {
                throw new ArgumentOutOfRangeException("data", "Data must be exactly 22 bytes");
            }

            this.data = data;

            this.index = index;

        }

        #endregion

        #region Helper Methods

        public byte[] GetBytes()
        {
            var bytes = new byte[data.Length];
            Buffer.BlockCopy(data, 0, bytes, 0, data.Length);
            return bytes;
        }

        public static float? ConvertToNullableFloat(byte[] value)
        {
            if (value.Length < 4)
            {
                return null;
            }
            else
            {
                var floatValue = BitConverter.ToSingle(value, 0);

                if (Single.IsNaN(floatValue) || Single.IsInfinity(floatValue))
                {
                    return null;
                }
                else
                {
                    return floatValue;
                }
            }
        }

        public static double? ConvertToNullableDouble(byte[] value)
        {
            if (value.Length < 8)
            {
                return null;
            }
            else
            {
                var doubleValue = BitConverter.ToDouble(value, 0);

                if (Double.IsNaN(doubleValue) || Double.IsInfinity(doubleValue))
                {
                    return null;
                }
                else
                {
                    return doubleValue;
                }
            }
        }

        #endregion

    }

}
