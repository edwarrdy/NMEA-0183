﻿using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// TTM - Tracked Target Message 
    /// </summary>
    public class TTMSentence : NMEASentence
    {
        private byte _targetNumber;

        /// <summary>
        /// Target number, 00 to 99
        /// </summary>
        public byte TargetNumber { get => _targetNumber; set => _targetNumber = Math.Min(value, (byte)99); }

        /// <summary>
        /// Target distance, from own ship 
        /// </summary>
        public decimal TargetDistance { get; set; }

        /// <summary>
        /// Bearing from own ship, degrees
        /// </summary>
        public decimal BearingFromOwnship { get; set; }

        /// <summary>
        /// Bearing from own ship, degrees True or Relative
        /// </summary>
        public DegreesEnum BearingFromOwnshipTrueRelative { get; set; }

        /// <summary>
        /// Target speed
        /// </summary>
        public decimal TargetSpeed { get; set; }

        /// <summary>
        /// Target course, degrees
        /// </summary>
        public decimal TargetCourse { get; set; }

        /// <summary>
        /// Target course, degrees True or Relative
        /// </summary>
        public DegreesEnum TargetCourseTrueRelative { get; set; }

        /// <summary>
        /// Distance of closest-point-of-approach 
        /// </summary>
        public decimal ClosestPointOfApproach { get; set; }

        /// <summary>
        /// Time to Distance of closest-point-of-approach, minutes
        /// </summary>
        public decimal TimeToClosestPointOfApproachMinutes { get; set; }

        /// <summary>
        /// Speed/distance units
        /// </summary>
        public SpeedDistanceUnitsEnum SpeedDistanceUnits { get; set; }

        /// <summary>
        /// Target name
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Target status
        /// </summary>
        public TargetStatusEnum TargetStatus { get; set; }

        /// <summary>
        /// Reference Target: set to "R" if target is a reference used to determined own-ship position or velocity, null otherwise.
        /// </summary>
        public bool ReferenceTarget { get; set; }

        /// <summary>
        /// UTC of Data
        /// </summary>
        public DateTime UTCOfData { get; set; }

        /// <summary>
        /// Type of acquisition
        /// </summary>
        public TypeOfAcquisitionEnum TypeOfAcquisition { get; set; }

        public TTMSentence()
        {
            SentenceIdentifier = "TTM";
            BearingFromOwnshipTrueRelative = DegreesEnum.T;
            TargetCourseTrueRelative = DegreesEnum.T;
            SpeedDistanceUnits = SpeedDistanceUnitsEnum.N;
            TargetStatus = TargetStatusEnum.L;
            UTCOfData = DateTime.UtcNow;
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", TargetNumber.ToString("D2"));

            stringBuilder.AppendFormat("{0},", TargetDistance);

            stringBuilder.AppendFormat("{0},", BearingFromOwnship);

            stringBuilder.AppendFormat("{0},", BearingFromOwnshipTrueRelative.ToString());

            stringBuilder.AppendFormat("{0},", TargetSpeed);

            stringBuilder.AppendFormat("{0},", TargetCourse);

            stringBuilder.AppendFormat("{0},", TargetCourseTrueRelative.ToString());

            stringBuilder.AppendFormat("{0},", ClosestPointOfApproach);

            stringBuilder.AppendFormat("{0},", TimeToClosestPointOfApproachMinutes);

            stringBuilder.AppendFormat("{0},", TargetName);

            stringBuilder.AppendFormat("{0},", TargetStatus.ToString());

            if (ReferenceTarget)
                stringBuilder.Append("R,");
            else
                stringBuilder.Append(",");

            stringBuilder.AppendFormat("{0},", UTCOfData.ToString("HHmmss.FF,"));

            stringBuilder.AppendFormat("{0}", TypeOfAcquisition.ToString());

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // xx Target Number
            TargetNumber = byte.Parse(vs[1]);

            // x.x Target Distance
            TargetDistance = decimal.Parse(vs[2]);

            // x.x Bearing from Ownship
            BearingFromOwnship = decimal.Parse(vs[3]);

            // a Bearing from Ownship True or Relative
            BearingFromOwnshipTrueRelative = (DegreesEnum)Enum.Parse(typeof(DegreesEnum), vs[4]);

            // x.x Target Speed
            TargetSpeed = decimal.Parse(vs[5]);

            // x.x Target Course
            TargetCourse = decimal.Parse(vs[6]);

            // a Target Course True or Relative
            TargetCourseTrueRelative = (DegreesEnum)Enum.Parse(typeof(DegreesEnum), vs[7]);

            // x.x Closest Point of Approach
            ClosestPointOfApproach = decimal.Parse(vs[8]);

            // x.x Time to Closest Point of Approach
            TimeToClosestPointOfApproachMinutes = decimal.Parse(vs[9]);

            // a Speed / Distance Units
            SpeedDistanceUnits = (SpeedDistanceUnitsEnum)Enum.Parse(typeof(SpeedDistanceUnitsEnum), vs[10]);

            // c--c Target Name
            TargetName = vs[11];

            // a Target Status
            TargetStatus = (TargetStatusEnum)Enum.Parse(typeof(TargetStatusEnum), vs[12]);

            // a Reference Target
            if (vs[13].Length > 0)
            {
                ReferenceTarget = vs[13] == "R";
            }
            else
            {
                ReferenceTarget = false;
            }

            // hhmmss.ss UTC of data
            string time = vs[14];
            time = time.Insert(4, ":");
            time = time.Insert(2, ":");
            TimeSpan timeSpan = TimeSpan.Parse(time);
            DateTime date = DateTime.Today;
            UTCOfData = new DateTime(date.Year, date.Month, date.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds, DateTimeKind.Utc);

            // a Type of acquisition
            TypeOfAcquisition = (TypeOfAcquisitionEnum)Enum.Parse(typeof(TypeOfAcquisitionEnum), vs[15]);
        }

        public enum DegreesEnum
        {
            /// <summary>
            /// Degrees, True
            /// </summary>
            T,
            /// <summary>
            /// Degrees, Relative
            /// </summary>
            R,
        }

        public enum SpeedDistanceUnitsEnum
        {
            /// <summary>
            /// km, km/h - Kilometres, Kilometres per Hour
            /// </summary>
            K,
            /// <summary>
            /// M, Knots - Nautical Mile, Nautical Mile per Hour
            /// </summary>
            N,
            /// <summary>
            /// mi, mi/h - Statute miles, Statute miles per Hour
            /// </summary>
            S,
        }

        public enum TargetStatusEnum
        {
            /// <summary>
            /// Lost, tracked target has been lost 
            /// </summary>
            L,
            /// <summary>
            /// Query, target in the process of acquisition 
            /// </summary>
            Q,
            /// <summary>
            /// Tracking
            /// </summary>
            T,
        }

        public enum TypeOfAcquisitionEnum
        {
            /// <summary>
            /// Auto
            /// </summary>
            A,
            /// <summary>
            /// Manual
            /// </summary>
            M,
            /// <summary>
            /// Reported
            /// </summary>
            R,
        }
    }
}
