using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// VDCUR - Water Current
    /// </summary>
    public class CURSentence : NMEASentence
    {
        /// <summary>
        /// Current speed in knots
        /// </summary>
        public decimal CurrentSpeed { get; set; }

        /// <summary>
        /// Bottom track source (P = GPS)
        /// </summary>
        public char BottomTrackSource { get; set; }

        public CURSentence()
        {
            SentenceIdentifier = "CUR";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);
            stringBuilder.AppendFormat("{0},", CurrentSpeed);
            stringBuilder.AppendFormat("{0},,,", BottomTrackSource);
            // 添加其他字段
            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // 当前速度（节）
            CurrentSpeed = decimal.Parse(vs[2]);

            // 底部跟踪源
            BottomTrackSource = vs[4][0]; // 'T'或'B'，根据需要解析
        }
    }
}
