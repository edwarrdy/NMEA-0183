using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// VHW - Water Speed and Heading
    /// </summary>
    public class VHWSentence : NMEASentence
    {
        /// <summary>
        /// Heading in Degrees True
        /// </summary>
        public decimal HeadingTrue { get; set; }

        /// <summary>
        /// Heading in Degrees Magnetic
        /// </summary>
        public decimal HeadingMagnetic { get; set; }

        /// <summary>
        /// Speed in Knots
        /// </summary>
        public decimal SpeedKnots { get; set; }

        /// <summary>
        /// Speed in Kilometres per Hour
        /// </summary>
        public decimal SpeedKmh { get; set; }

        public VHWSentence()
        {
            SentenceIdentifier = "VHW";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},T,", HeadingTrue);

            stringBuilder.AppendFormat("{0},M,", HeadingMagnetic);

            stringBuilder.AppendFormat("{0},N,", SpeedKnots);

            stringBuilder.AppendFormat("{0},K", SpeedKmh);

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // x.x Heading, degrees True 
            HeadingTrue = decimal.Parse(vs[1]);

            // x.x Heading, degrees Magnetic 
            HeadingMagnetic = decimal.Parse(vs[3]);

            // x.x Speed, knots
            SpeedKnots = decimal.Parse(vs[5]);

            // x.x Speed, Km/h
            SpeedKmh = decimal.Parse(vs[7]);
        }
    }
}
