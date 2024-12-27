using System;
using System.Collections.Generic;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// DBT - Depth Below Transducer
    /// </summary>
    public class DBTSentence : NMEASentence
    {
        /// <summary>
        /// Depth in Feet
        /// </summary>
        public decimal DepthFeet { get; set; }

        /// <summary>
        /// Depth in Metres
        /// </summary>
        public decimal DepthMetres { get; set; }

        /// <summary>
        /// Depth in Fathoms
        /// </summary>
        public decimal DepthFathoms { get; set; }

        public DBTSentence()
        {
            SentenceIdentifier = "DBT";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},f,", DepthFeet.ToString());

            stringBuilder.AppendFormat("{0},M,", DepthMetres.ToString());

            stringBuilder.AppendFormat("{0},F", DepthFathoms.ToString());

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // x.x  Depth in feet
            if (vs[1] != null && vs[1] != "")
                DepthFeet = decimal.Parse(vs[1]);


            // x.x  Depth in metres
            if (vs[3] != null && vs[3] != "")
                DepthMetres = decimal.Parse(vs[3]);

            // x.x  Depth in fathoms
            if (vs[5] != null && vs[5] != "")
                DepthFathoms = decimal.Parse(vs[5]);

        }
    }
}
