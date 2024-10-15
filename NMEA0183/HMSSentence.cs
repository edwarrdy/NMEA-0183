using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// HMS - Heading Monitor Set
    /// </summary>
    public class HMSSentence : NMEASentence
    {
        /// <summary>
        /// Heading Sensor 1 ID
        /// </summary>
        public string HeadingSensorOneID { get; set; }

        /// <summary>
        /// Heading Sensor 2 ID
        /// </summary>
        public string HeadingSensorTwoID { get; set; }

        /// <summary>
        /// Maximum difference, degrees 
        /// </summary>
        public decimal MaximumDifferenceDegrees { get; set; }

        public HMSSentence()
        {
            SentenceIdentifier = "HMS";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", HeadingSensorOneID);

            stringBuilder.AppendFormat("{0},", HeadingSensorTwoID);

            stringBuilder.AppendFormat("{0}", MaximumDifferenceDegrees);

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // c--c Heading Sensor 1 ID
            HeadingSensorOneID = vs[1];

            // c--c Heading Sensor 2 ID
            HeadingSensorTwoID = vs[2];

            // x.x Maximum difference, degrees
            MaximumDifferenceDegrees = decimal.Parse(vs[3]);
        }
    }
}
