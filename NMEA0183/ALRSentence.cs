using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// VDALR - Set Alarm State
    /// </summary>
    public class ALRSentence : NMEASentence
    {
        /// <summary>
        /// Time in hhmmss.ss format
        /// </summary>
        public DateTime AlarmTime { get; set; }

        /// <summary>
        /// Alarm type or number
        /// </summary>
        public string AlarmType { get; set; }

        /// <summary>
        /// Alarm state (A = Active, D = Deactivated)
        /// </summary>
        public char AlarmState { get; set; }

        /// <summary>
        /// Alarm message
        /// </summary>
        public string AlarmMessage { get; set; }

        public ALRSentence()
        {
            SentenceIdentifier = "ALR";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);
            stringBuilder.AppendFormat("{0},{1},{2},{3},",
                AlarmTime.ToString("hhmmss.ff"), AlarmType, AlarmState, AlarmMessage);
            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);
            string[] vs = sentence.Split(new char[] { ',', '*' });

            // 时间解析
            AlarmTime = DateTime.ParseExact(vs[1], "hhmmss.ff", null);

            // 警报类型
            AlarmType = vs[2];

            // 警报状态
            AlarmState = vs[3][0];

            // 警报信息
            AlarmMessage = vs[4];
        }
    }
}
