using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// HDT - Heading - True
    /// </summary>
    public class HDTSentence : NMEASentence
    {
        /// <summary>
        /// Heading in Degrees
        /// </summary>
        public decimal HeadingTrue { get; set; }

        public HDTSentence()
        {
            SentenceIdentifier = "HDT";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},T", HeadingTrue.ToString());

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
        }
    }
}
