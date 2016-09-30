using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.Chart.Util
{
    static class Extensions
    {
    }

    /// <summary>
    /// from 4.62 DateTime struct 
    /// http://referencesource.microsoft.com/#mscorlib/system/datetime.cs,df6b1eba7461813b
    /// </summary>
    public static class DateTimeEx
    {

        // Number of 100ns ticks per time unit
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;
        private const long TicksPerMinute = TicksPerSecond * 60;
        private const long TicksPerHour = TicksPerMinute * 60;
        private const long TicksPerDay = TicksPerHour * 24;

        // Number of milliseconds per time unit
        private const int MillisPerSecond = 1000;
        private const int MillisPerMinute = MillisPerSecond * 60;
        private const int MillisPerHour = MillisPerMinute * 60;
        private const int MillisPerDay = MillisPerHour * 24;

        // Number of days in a non-leap year
        private const int DaysPerYear = 365;
        // Number of days in 4 years
        private const int DaysPer4Years = DaysPerYear * 4 + 1;       // 1461
        // Number of days in 100 years
        private const int DaysPer100Years = DaysPer4Years * 25 - 1;  // 36524
        // Number of days in 400 years
        private const int DaysPer400Years = DaysPer100Years * 4 + 1; // 146097

        // Number of days from 1/1/0001 to 12/31/1600
        private const int DaysTo1601 = DaysPer400Years * 4;          // 584388
        // Number of days from 1/1/0001 to 12/30/1899
        private const int DaysTo1899 = DaysPer400Years * 4 + DaysPer100Years * 3 - 367;
        // Number of days from 1/1/0001 to 12/31/1969
        internal const int DaysTo1970 = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear; // 719,162
        // Number of days from 1/1/0001 to 12/31/9999
        private const int DaysTo10000 = DaysPer400Years * 25 - 366;  // 3652059

        internal const long MinTicks = 0;
        internal const long MaxTicks = DaysTo10000 * TicksPerDay - 1;
        private const long MaxMillis = (long)DaysTo10000 * MillisPerDay;

        private const long FileTimeOffset = DaysTo1601 * TicksPerDay;
        private const long DoubleDateOffset = DaysTo1899 * TicksPerDay;
        // The minimum OA date is 0100/01/01 (Note it's year 100).
        // The maximum OA date is 9999/12/31
        private const long OADateMinAsTicks = (DaysPer100Years - DaysPerYear) * TicksPerDay;
        // All OA dates must be greater than (not >=) OADateMinAsDouble
        private const double OADateMinAsDouble = -657435.0;
        // All OA dates must be less than (not <=) OADateMaxAsDouble
        private const double OADateMaxAsDouble = 2958466.0;

        //private const int DatePartYear = 0;
        //private const int DatePartDayOfYear = 1;
        //private const int DatePartMonth = 2;
        //private const int DatePartDay = 3;

        //private static readonly int[] DaysToMonth365 = {
        //    0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};
        //private static readonly int[] DaysToMonth366 = {
        //    0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};

        //public static readonly DateTime MinValue = new DateTime(MinTicks, DateTimeKind.Unspecified);
        //public static readonly DateTime MaxValue = new DateTime(MaxTicks, DateTimeKind.Unspecified);

        //private const UInt64 TicksMask = 0x3FFFFFFFFFFFFFFF;
        //private const UInt64 FlagsMask = 0xC000000000000000;
        //private const UInt64 LocalMask = 0x8000000000000000;
        //private const Int64 TicksCeiling = 0x4000000000000000;
        //private const UInt64 KindUnspecified = 0x0000000000000000;
        //private const UInt64 KindUtc = 0x4000000000000000;
        //private const UInt64 KindLocal = 0x8000000000000000;
        //private const UInt64 KindLocalAmbiguousDst = 0xC000000000000000;
        //private const Int32 KindShift = 62;

        //private const String TicksField = "ticks";
        //private const String DateDataField = "dateData";



        // Creates a DateTime from an OLE Automation Date.
        public static DateTime FromOADate(double d)
        {
            return new DateTime(DoubleDateToTicks(d), DateTimeKind.Unspecified);
        }


        // Converts the DateTime instance into an OLE Automation compatible
        // double date.
        public static double ToOADate(this DateTime date)
        {
            return TicksToOADate(date.Ticks);
        }

        // This function is duplicated in COMDateTime.cpp
        private static double TicksToOADate(long value)
        {
            if (value == 0)
                return 0.0;  // Returns OleAut's zero'ed date value.
            if (value < TicksPerDay) // This is a fix for VB. They want the default day to be 1/1/0001 rathar then 12/30/1899.
                value += DoubleDateOffset; // We could have moved this fix down but we would like to keep the bounds check.
            if (value < OADateMinAsTicks)
                throw new OverflowException("Arg_OleAutDateInvalid");
            // Currently, our max date == OA's max date (12/31/9999), so we don't 
            // need an overflow check in that direction.
            long millis = (value - DoubleDateOffset) / TicksPerMillisecond;
            if (millis < 0)
            {
                long frac = millis % MillisPerDay;
                if (frac != 0) millis -= (MillisPerDay + frac) * 2;
            }
            return (double)millis / MillisPerDay;
        }

        // Converts an OLE Date to a tick count.
        // This function is duplicated in COMDateTime.cpp
        internal static long DoubleDateToTicks(double value)
        {
            // The check done this way will take care of NaN
            if (!(value < OADateMaxAsDouble) || !(value > OADateMinAsDouble))
                throw new ArgumentException("Arg_OleAutDateInvalid");

            // Conversion to long will not cause an overflow here, as at this point the "value" is in between OADateMinAsDouble and OADateMaxAsDouble
            long millis = (long)(value * MillisPerDay + (value >= 0 ? 0.5 : -0.5));
            // The interesting thing here is when you have a value like 12.5 it all positive 12 days and 12 hours from 01/01/1899
            // However if you a value of -12.25 it is minus 12 days but still positive 6 hours, almost as though you meant -11.75 all negative
            // This line below fixes up the millis in the negative case
            if (millis < 0)
            {
                millis -= (millis % MillisPerDay) * 2;
            }

            millis += DoubleDateOffset / TicksPerMillisecond;

            if (millis < 0 || millis >= MaxMillis) throw new ArgumentException("Arg_OleAutDateScale");
            return millis * TicksPerMillisecond;
        }
    }
}
