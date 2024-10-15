﻿using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// HDT - Heading, Deviation & Variation
    /// </summary>
    public class HDGSentence : NMEASentence
    {
        /// <summary>
        /// Magnetic sensor heading in Degrees
        /// </summary>
        public decimal MagneticSensorHeading { get; set; }

        /// <summary>
        /// Magnetic deviation in Degrees
        /// Left as null if Unknown
        /// </summary>
        public decimal? MagneticDeviation { get; set; }

        /// <summary>
        /// Easterly or Westerly Magnetic Deviation
        /// </summary>
        public DirectionEnum MagneticDeviationDirection { get; set; }

        /// <summary>
        /// Magnetic Variation in Degrees
        /// Left as null if Unknown
        /// </summary>
        public decimal? MagneticVariation { get; set; }

        /// <summary>
        /// Easterly or Westerly Magnetic Variation
        /// </summary>
        public DirectionEnum MagneticVariationDirection { get; set; }

        public HDGSentence()
        {
            SentenceIdentifier = "HDG";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", MagneticSensorHeading.ToString());

            if (MagneticDeviation.HasValue)
            {
                stringBuilder.AppendFormat("{0},", MagneticDeviation.ToString());
                stringBuilder.AppendFormat("{0},", MagneticDeviationDirection.ToString());
            }
            else
            {
                stringBuilder.Append(",,");
            }

            if (MagneticVariation.HasValue)
            {
                stringBuilder.AppendFormat("{0},", MagneticVariation.ToString());
                stringBuilder.AppendFormat("{0}", MagneticVariationDirection.ToString());
            }
            else
            {
                stringBuilder.Append(",");
            }

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // x.x Magnetic sensor heading
            MagneticSensorHeading = decimal.Parse(vs[1]);

            // x.x,a Magnetic deviation, degrees E/W
            if (vs[2].Length > 0)
            {
                MagneticDeviation = decimal.Parse(vs[2]);
                MagneticDeviationDirection = (DirectionEnum)Enum.Parse(typeof(DirectionEnum), vs[3]); ;
            }
            else
            {
                MagneticDeviation = null;
                MagneticDeviationDirection = DirectionEnum.E;
            }

            // x.x,a Magnetic variation, degrees E/W
            if (vs[4].Length > 0)
            {
                MagneticVariation = decimal.Parse(vs[4]);
                MagneticVariationDirection = (DirectionEnum)Enum.Parse(typeof(DirectionEnum), vs[5]); ;
            }
            else
            {
                MagneticVariation = null;
                MagneticVariationDirection = DirectionEnum.E;
            }
        }

        public enum DirectionEnum
        {
            /// <summary>
            /// Easterly
            /// </summary>
            E,
            /// <summary>
            ///  Westerly
            /// </summary>
            W,
        }
    }
}
