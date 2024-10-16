using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// VDALF - Set Alarm Information
    /// </summary>
    public class ALFSentence : NMEASentence
    {
        public string AlarmStatus1 { get; set; }
        public string AlarmStatus2 { get; set; }
        public string AlarmStatus3 { get; set; }
        public DateTime AlarmTime { get; set; }
        public string AdditionalInfo1 { get; set; }
        public string AdditionalInfo2 { get; set; }
        public string AdditionalInfo3 { get; set; }
        public string Identifier { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public decimal Value3 { get; set; }
        public string StatusIndicator { get; set; }
        public string OptionalInfo { get; set; }

        public ALFSentence()
        {
            SentenceIdentifier = "ALF";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);
            stringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},",
                AlarmStatus1, AlarmStatus2, AlarmStatus3,
                AlarmTime.ToString("hhmmss.ff"), AdditionalInfo1,
                AdditionalInfo2, AdditionalInfo3, Identifier,
                Value1, Value2, Value3, StatusIndicator,
                OptionalInfo);
            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);
            string[] vs = sentence.Split(new char[] { ',', '*' });

            // 警报状态
            AlarmStatus1 = vs[1];
            AlarmStatus2 = vs[2];
            AlarmStatus3 = vs[3];

            // 时间解析
            AlarmTime = DateTime.ParseExact(vs[4], "hhmmss.ff", null);

            // 其他信息
            AdditionalInfo1 = vs[5];
            AdditionalInfo2 = vs[6];
            AdditionalInfo3 = vs[7];
            Identifier = vs[8];

            // 数值
            Value1 = Decimal.Parse(vs[9]);
            Value2 = Decimal.Parse(vs[10]);
            Value3 = Decimal.Parse(vs[11]);

            // 状态指示符和可选信息
            StatusIndicator = vs[12];
            OptionalInfo = vs[13];
        }
    }
}
