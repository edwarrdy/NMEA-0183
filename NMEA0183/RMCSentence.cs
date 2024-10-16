using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// RMC - Recommended Minimum Specific GPS/Transit Data
    /// </summary>
    public class RMCSentence : NMEASentence
    {
        public DateTime UtcTime { get; set; }
        public char Status { get; set; }
        public decimal Latitude { get; set; }
        public char LatitudeDirection { get; set; }
        public decimal Longitude { get; set; }
        public char LongitudeDirection { get; set; }
        public decimal Speed { get; set; }
        public decimal Course { get; set; }
        public DateTime Date { get; set; }

        public RMCSentence()
        {
            SentenceIdentifier = "RMC";
        }

        public override String EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);
            stringBuilder.AppendFormat("{0},{1},", UtcTime.ToString("hhmmss.fff"), Status);
            stringBuilder.AppendFormat("{0},{1},", Latitude, LatitudeDirection);
            stringBuilder.AppendFormat("{0},{1},", Longitude, LongitudeDirection);
            stringBuilder.AppendFormat("{0},{1},", Speed, Course);
            stringBuilder.AppendFormat("{0},,", Date.ToString("ddMMyy")); // Date in ddMMyy format

            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(String sentence)
        {
            DecodeTalker(sentence);

            String[] vs = sentence.Split(new char[] { ',', '*' });

            // Parse UTC time
            UtcTime = DateTime.ParseExact(vs[1], "HHmmss.ff", null);

            // Parse status
            Status = vs[2][0];

            // Parse latitude
            Latitude = Decimal.Parse(vs[3]);
            LatitudeDirection = vs[4][0];

            // Parse longitude
            Longitude = Decimal.Parse(vs[5]);
            LongitudeDirection = vs[6][0];

            // Parse speed
            Speed = Decimal.Parse(vs[7]);

            // Parse course
            Course = Decimal.Parse(vs[8]);

            // Parse date
            Date = DateTime.ParseExact(vs[9], "ddMMyy", null);
        }
    }
}
