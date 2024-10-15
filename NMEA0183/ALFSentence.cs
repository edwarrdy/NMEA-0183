using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// ALF - Custom message for heading, position, and speed.
    /// </summary>
    public class ALFSentence : NMEASentence
    {
        /// <summary>
        /// First heading angle (could be true, magnetic, or geographic)
        /// </summary>
        public Decimal Heading1 { get; set; }

        /// <summary>
        /// Second heading angle (could be true, magnetic, or geographic)
        /// </summary>
        public Decimal Heading2 { get; set; }

        /// <summary>
        /// Third heading angle (could be true, magnetic, or geographic)
        /// </summary>
        public Decimal Heading3 { get; set; }

        /// <summary>
        /// UTC Time
        /// </summary>
        public String UTCTime { get; set; }

        /// <summary>
        /// Latitude with N/S indicator
        /// </summary>
        public String Latitude { get; set; }

        /// <summary>
        /// Longitude with E/W indicator
        /// </summary>
        public String Longitude { get; set; }

        /// <summary>
        /// Course angle (usually 3 digits)
        /// </summary>
        public Decimal CourseAngle { get; set; }

        /// <summary>
        /// First speed in knots
        /// </summary>
        public Decimal Speed1 { get; set; }

        /// <summary>
        /// Second speed in knots
        /// </summary>
        public Decimal Speed2 { get; set; }

        /// <summary>
        /// Third speed in knots
        /// </summary>
        public Decimal Speed3 { get; set; }

        /// <summary>
        /// Status flag
        /// </summary>
        public char StatusFlag { get; set; }

        /// <summary>
        /// Custom data string
        /// </summary>
        public String CustomData { get; set; }

        public ALFSentence()
        {
            SentenceIdentifier = "ALF";
        }

        public override String EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Start sentence
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            // Add heading angles
            stringBuilder.AppendFormat("{0},{1},{2},", Heading1, Heading2, Heading3);

            // Add UTC time
            stringBuilder.AppendFormat("{0},", UTCTime);

            // Add latitude and longitude
            stringBuilder.AppendFormat("{0},{1},", Latitude, Longitude);

            // Add course angle
            stringBuilder.AppendFormat("{0},", CourseAngle);

            // Add speeds
            stringBuilder.AppendFormat("{0},{1},{2},", Speed1, Speed2, Speed3);

            // Add status flag and custom data
            stringBuilder.AppendFormat("{0},{1}", StatusFlag, CustomData);

            // Calculate and append checksum
            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(String sentence)
        {
            DecodeTalker(sentence);

            // Split sentence by ',' and '*'
            String[] vs = sentence.Split(new char[] { ',', '*' });

            // Parse headings
            Heading1 = Decimal.Parse(vs[1]);
            Heading2 = Decimal.Parse(vs[2]);
            Heading3 = Decimal.Parse(vs[3]);

            // Parse UTC time
            UTCTime = vs[4];

            // Parse latitude and longitude
            Latitude = vs[5];
            Longitude = vs[6];

            // Parse course angle
            CourseAngle = Decimal.Parse(vs[7]);

            // Parse speeds
            Speed1 = Decimal.Parse(vs[8]);
            Speed2 = Decimal.Parse(vs[9]);
            Speed3 = Decimal.Parse(vs[10]);

            // Parse status flag
            StatusFlag = vs[11][0];

            // Parse custom data
            CustomData = vs[12];
        }
    }
}
