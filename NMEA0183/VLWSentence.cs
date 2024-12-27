using System;
using System.Text;


namespace edward.NMEA0183
{
    /// <summary>
    /// VLW - Dual Water and Ground Trip Data
    /// </summary>
    public class VLWSentence : NMEASentence
    {
        /// <summary>
        /// Total trip (Water) in nautical miles
        /// </summary>
        public decimal TotalTripWater { get; set; }

        /// <summary>
        /// Trip (Water) in nautical miles
        /// </summary>
        public decimal TripWater { get; set; }

        /// <summary>
        /// Total Distance (Ground) in nautical miles
        /// </summary>
        public decimal TotalDistanceGround { get; set; }

        /// <summary>
        /// Trip (Ground) in nautical miles
        /// </summary>
        public decimal TripGround { get; set; }

        public VLWSentence()
        {
            SentenceIdentifier = "VLW";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);
            stringBuilder.AppendFormat("{0},N,", TotalTripWater);
            stringBuilder.AppendFormat("{0},N,", TripWater);
            stringBuilder.AppendFormat("{0},N,", TotalDistanceGround);
            stringBuilder.AppendFormat("{0},N", TripGround);

            byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // Total trip (Water)
            TotalTripWater = decimal.Parse(vs[1]);

            // Trip (Water)
            TripWater = decimal.Parse(vs[3]);

            // Total Distance (Ground)
            TotalDistanceGround = decimal.Parse(vs[5]);

            // Trip (Ground)
            TripGround = decimal.Parse(vs[7]);
        }
    }
}
