using System;
using System.Collections.Generic;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// THS - True Heading and Status
    /// </summary>
    public class THSSentence : NMEASentence
    {
        /// <summary>
        /// Heading in Degrees
        /// </summary>
        public decimal HeadingTrue { get; set; }

        /// <summary>
        /// Mode Indicator
        /// </summary>
        public ModeIndicatorEnum ModeIndicator { get; set; }

        public THSSentence()
        {
            SentenceIdentifier = "THS";
            ModeIndicator = ModeIndicatorEnum.V;
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", HeadingTrue);

            stringBuilder.AppendFormat("{0}", ModeIndicator.ToString());

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

            // a Mode Indicator
            ModeIndicator = (ModeIndicatorEnum)Enum.Parse(typeof(ModeIndicatorEnum), vs[2]);
        }

        public enum ModeIndicatorEnum
        {
            /// <summary>
            /// Autnomous
            /// </summary>
            A,
            /// <summary>
            /// Estimated (Dead-Reckoning)
            /// </summary>
            E,
            /// <summary>
            /// Manual Input
            /// </summary>
            M,
            /// <summary>
            /// Simulator
            /// </summary>
            S,
            /// <summary>
            /// Data not valid (including standby)
            /// </summary>
            V,
        }
    }
}
