using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// OSD - Own Ship Data
    /// </summary>
    public class OSDSentence : NMEASentence
    {
        /// <summary>
        /// Heading in Degrees
        /// </summary>
        public decimal HeadingTrue { get; set; }

        /// <summary>
        /// Is the Heading valid?
        /// </summary>
        public bool IsHeadingValid { get; set; }

        /// <summary>
        /// Vessel Course in Degrees
        /// </summary>
        public decimal VesselCourseTrue { get; set; }

        /// <summary>
        /// Course Reference
        /// </summary>
        public ReferenceSystemEnum VesselCourseReference { get; set; }

        /// <summary>
        /// Vessel Speed
        /// </summary>
        public decimal VesselSpeed { get; set; }

        /// <summary>
        /// Speed Reference
        /// </summary>
        public ReferenceSystemEnum VesselSpeedReference { get; set; }

        /// <summary>
        /// Vessel Set in Degrees
        /// </summary>
        public decimal VesselSetTrue { get; set; }

        /// <summary>
        /// Vessel Drift
        /// </summary>
        public decimal VesselDrift { get; set; }

        /// <summary>
        /// Speed units
        /// </summary>
        public SpeedUnitsEnum SpeedUnits { get; set; }

        public OSDSentence()
        {
            SentenceIdentifier = "OSD";
            IsHeadingValid = false;
            VesselCourseReference = ReferenceSystemEnum.M;
            VesselSpeedReference = ReferenceSystemEnum.M;
            SpeedUnits = SpeedUnitsEnum.N;
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", HeadingTrue.ToString());

            if (IsHeadingValid)
                stringBuilder.Append("A,");
            else
                stringBuilder.Append("V,");

            stringBuilder.AppendFormat("{0},", VesselCourseTrue.ToString());

            stringBuilder.AppendFormat("{0},", VesselCourseReference.ToString());

            stringBuilder.AppendFormat("{0},", VesselSpeed.ToString());

            stringBuilder.AppendFormat("{0},", VesselSpeedReference.ToString());

            stringBuilder.AppendFormat("{0},", VesselSetTrue.ToString());

            stringBuilder.AppendFormat("{0},", VesselDrift.ToString());

            stringBuilder.AppendFormat("{0}", SpeedUnits.ToString());

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // x.x Heading True
            HeadingTrue = decimal.Parse(vs[1]);

            // Heading Status
            if (vs[2] == "A")
                IsHeadingValid = true;
            else
                IsHeadingValid = false;

            // x.x Vessel Course True
            VesselCourseTrue = decimal.Parse(vs[3]);

            // a Course Reference
            VesselCourseReference = (ReferenceSystemEnum)Enum.Parse(typeof(ReferenceSystemEnum), vs[4]);

            // x.x Vessel Speed
            VesselSpeed = decimal.Parse(vs[5]);

            // a Course Reference
            VesselSpeedReference = (ReferenceSystemEnum)Enum.Parse(typeof(ReferenceSystemEnum), vs[6]);

            // x.x Vessel Set True
            VesselSetTrue = decimal.Parse(vs[7]);

            // x.x Vessel Drift
            VesselDrift = decimal.Parse(vs[8]);

            // a Speed Units
            SpeedUnits = (SpeedUnitsEnum)Enum.Parse(typeof(SpeedUnitsEnum), vs[9]);
        }

        public enum ReferenceSystemEnum
        {
            /// <summary>
            /// Bottom Tracking Log
            /// </summary>
            B,
            /// <summary>
            /// Manually Entered
            /// </summary>
            M,
            /// <summary>
            /// Water Referenced
            /// </summary>
            W,
            /// <summary>
            /// Radar Tracking (of Fixed Target)
            /// </summary>
            R,
            /// <summary>
            /// Positioning System Ground Reference
            /// </summary>
            P,
        }

        public enum SpeedUnitsEnum
        {
            /// <summary>
            /// km/h - Kilometres per Hour
            /// </summary>
            K,
            /// <summary>
            /// Knots - Nautical Mile per Hour
            /// </summary>
            N,
            /// <summary>
            /// mi/h - Statute Mile per Hour
            /// </summary>
            S,
        }
    }
}
