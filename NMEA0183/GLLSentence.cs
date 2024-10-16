using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// GLL - Geographic Position – Latitude/Longitude
    /// </summary>
    public class GLLSentence : NMEASentence
    {
        /// <summary>
        /// Latitude in Decimal Degrees, North - Positive, South - Negative
        /// </summary>
        public decimal LatitudeDecimalDegrees { get; set; }

        /// <summary>
        /// Longitude in Decimal Degrees, East - Positive, West - Negative
        /// </summary>
        public decimal LongitudeDecimalDegrees { get; set; }

        /// <summary>
        /// UTC Time At Position
        /// </summary>
        public DateTime UTCTimeAtPosition { get; set; }

        /// <summary>
        /// Is the data valid?
        /// </summary>
        public bool IsDataValid { get; set; }

        public GLLSentence()
        {
            SentenceIdentifier = "GLL";
            UTCTimeAtPosition = DateTime.UtcNow;
            IsDataValid = false;
        }

        /// <summary>
        /// Conversion from Decimal Degrees to Degrees, Minute
        /// </summary>
        /// <param name="decimalDegrees">Decimal Degress</param>
        /// <param name="degrees">Degrees output</param>
        /// <param name="decimalMinutes">Decimal Minutes output</param>
        protected static void DecimalDegreesToDegreesMinute(decimal decimalDegrees, out int degrees, out decimal decimalMinutes)
        {
            decimal absDecimalDegrees = Math.Abs(decimalDegrees);

            degrees = Convert.ToInt32(Math.Floor(absDecimalDegrees));

            decimalMinutes = absDecimalDegrees - degrees;
            decimalMinutes *= 60.0M;

            if (decimalDegrees < 0)
                degrees *= -1;
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            DecimalDegreesToDegreesMinute(LatitudeDecimalDegrees, out int LatDegrees, out decimal LatMinutes);
            stringBuilder.AppendFormat("{0}{1},", Math.Abs(LatDegrees).ToString("D2"), LatMinutes.ToString("00.#######"));
            if (LatDegrees < 0)
                stringBuilder.Append("S,");
            else
                stringBuilder.Append("N,");

            DecimalDegreesToDegreesMinute(LongitudeDecimalDegrees, out int LonDegrees, out decimal LonMinutes);
            stringBuilder.AppendFormat("{0}{1},", Math.Abs(LonDegrees).ToString("D3"), LonMinutes.ToString("00.#######"));
            if (LonDegrees < 0)
                stringBuilder.Append("W,");
            else
                stringBuilder.Append("E,");

            stringBuilder.AppendFormat("{0},", UTCTimeAtPosition.ToString("HHmmss.FF"));

            if (IsDataValid)
                stringBuilder.Append("A");
            else
                stringBuilder.Append("V");

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // XXyy.yy Latitude
            string degreeDec = vs[1].Substring(0, 2);
            string degreeMin = vs[1].Substring(2);
            decimal degreeMinute = decimal.Parse(degreeMin);
            LatitudeDecimalDegrees = decimal.Parse(degreeDec) + degreeMinute / 60.0M;

            // North or South
            if (vs[2] == "S")
                LatitudeDecimalDegrees *= decimal.MinusOne;

            // XXXyy.yy Latitude
            degreeDec = vs[3].Substring(0, 3);
            degreeMin = vs[3].Substring(3);
            degreeMinute = decimal.Parse(degreeMin);
            LongitudeDecimalDegrees = decimal.Parse(degreeDec) + degreeMinute / 60.0M;

            // North or South
            if (vs[4] == "W")
                LongitudeDecimalDegrees *= decimal.MinusOne;

            // hhmmss.ss UTC Time
            string time = vs[5];
            time = time.Insert(4, ":");
            time = time.Insert(2, ":");
            TimeSpan timeSpan = TimeSpan.Parse(time);
            DateTime today = DateTime.UtcNow;
            UTCTimeAtPosition = new DateTime(today.Year, today.Month, today.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds, DateTimeKind.Utc);

            // A Status
            if (vs[6] == "A")
                IsDataValid = true;
            else
                IsDataValid = false;
        }
    }
}
