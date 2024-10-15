using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// ZDA - Time & Date
    /// </summary>
    public class ZDASentence : NMEASentence
    {
        /// <summary>
        /// UTC Date and Time At Position
        /// </summary>
        public DateTime UTCDateTimeAtPosition { get; set; }

        /// <summary>
        /// Local Time Zone
        /// </summary>
        public TimeSpan LocalTimeZone { get; set; }

        protected TimeSpan DifferenceToUtc { get { return TimeSpan.Zero - LocalTimeZone; } set { LocalTimeZone = TimeSpan.Zero - value; } }

        public ZDASentence()
        {
            SentenceIdentifier = "ZDA";
            UTCDateTimeAtPosition = DateTime.UtcNow;
            LocalTimeZone = TimeZoneInfo.Local.BaseUtcOffset;
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", UTCDateTimeAtPosition.ToString("HHmmss.FF"));

            stringBuilder.AppendFormat("{0},", UTCDateTimeAtPosition.ToString("dd"));

            stringBuilder.AppendFormat("{0},", UTCDateTimeAtPosition.ToString("MM"));

            stringBuilder.AppendFormat("{0},", UTCDateTimeAtPosition.ToString("yyyy"));

            if (DifferenceToUtc < TimeSpan.Zero)
                stringBuilder.Append("-");
            stringBuilder.AppendFormat("{0},{1}", DifferenceToUtc.ToString("hh"), DifferenceToUtc.ToString("mm"));

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // hhmmss.ss UTC Time
            string time = vs[1];
            time = time.Insert(4, ":");
            time = time.Insert(2, ":");
            TimeSpan timeSpan = TimeSpan.Parse(time);

            // xx,xx,xxxx UTC Date
            int day = int.Parse(vs[2]);
            int month = int.Parse(vs[3]);
            int year = int.Parse(vs[4]);

            UTCDateTimeAtPosition = new DateTime(year, month, day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds, DateTimeKind.Utc);

            // xx,xx Local zone
            StringBuilder UTCOffset = new StringBuilder();
            UTCOffset.AppendFormat("{0}:{1}", vs[5], vs[6]);
            DifferenceToUtc = TimeSpan.Parse(UTCOffset.ToString());
        }
    }
}
