using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// ACN - Alert Command
    /// </summary>
    public class ACNSentence : NMEASentence
    {
        public DateTime UtcTime { get; set; }
        public string ManufacturerMnemonic { get; set; }
        public string AlertIdentifier { get; set; }
        public string AlertInstance { get; set; }
        public char AlertCommand { get; set; }
        public char StatusFlag { get; set; }

        public ACNSentence()
        {
            SentenceIdentifier = "ACN";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("${0}{1},{2:D2}{3:D2}{4:D2}.{5:D3},{6},{7},{8},{9}\r\n",
                TalkerIdentifier.ToString(), SentenceIdentifier,
                UtcTime.Hour, UtcTime.Minute, UtcTime.Second, UtcTime.Millisecond,
                ManufacturerMnemonic ?? "",
                AlertIdentifier, AlertInstance, AlertCommand, StatusFlag);

            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);
            string[] vs = sentence.Split(new char[] { ',', '*' });

            // 解析 UTC 时间
            string timeStr = vs[1];
            int hours = int.Parse(timeStr.Substring(0, 2));
            int minutes = int.Parse(timeStr.Substring(2, 2));
            int seconds = int.Parse(timeStr.Substring(4, 2));
            int milliseconds = int.Parse(timeStr.Substring(7, 3));

            UtcTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                                   hours, minutes, seconds, milliseconds);

            ManufacturerMnemonic = vs[2];
            AlertIdentifier = vs[3];
            AlertInstance = vs[4];
            AlertCommand = char.Parse(vs[5]);
            StatusFlag = char.Parse(vs[6]);
        }
    }
}
