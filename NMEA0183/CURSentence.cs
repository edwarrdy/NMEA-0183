using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// CUR - Water Current Sentence
    /// </summary>
    public class CURSentence : NMEASentence
    {
        /// <summary>
        /// Current Direction in degrees
        /// </summary>
        public Decimal CurrentDirection { get; set; }

        /// <summary>
        /// Current Speed in knots
        /// </summary>
        public Decimal CurrentSpeed { get; set; }

        /// <summary>
        /// Bottom Track Current Speed in knots
        /// </summary>
        public Decimal BottomTrackSpeed { get; set; }

        /// <summary>
        /// GPS Track Current Speed in knots
        /// </summary>
        public Decimal GPSTrackSpeed { get; set; }

        /// <summary>
        /// Tracking mode ('B' for Bottom Track, 'P' for GPS)
        /// </summary>
        public char TrackingMode { get; set; }

        public CURSentence()
        {
            SentenceIdentifier = "CUR";
        }

        public override String EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // 开始生成NMEA字符串
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            // 添加当前流向
            stringBuilder.AppendFormat("{0},", CurrentDirection);

            // 添加当前流速
            stringBuilder.AppendFormat("{0},", CurrentSpeed);

            // 添加底部跟踪流速 (可选，可能为空)
            stringBuilder.AppendFormat(",T{0},", BottomTrackSpeed);

            // 添加 GPS 跟踪流速 (可选，可能为空)
            stringBuilder.AppendFormat("{0},", GPSTrackSpeed);

            // 添加追踪模式
            stringBuilder.AppendFormat(",T,{0}", TrackingMode);

            // 计算校验和
            Byte checksum = CalculateChecksum(stringBuilder.ToString());

            // 添加校验和
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(String sentence)
        {
            DecodeTalker(sentence);

            // 根据',', '*'拆分NMEA字符串
            String[] vs = sentence.Split(new char[] { ',', '*' });

            // 解析当前流向
            CurrentDirection = Decimal.Parse(vs[1]);

            // 解析当前流速
            CurrentSpeed = Decimal.Parse(vs[2]);

            // 解析底部跟踪流速（如果存在）
            if (!string.IsNullOrEmpty(vs[4]))
                BottomTrackSpeed = Decimal.Parse(vs[4]);

            // 解析 GPS 跟踪流速（如果存在）
            if (!string.IsNullOrEmpty(vs[5]))
                GPSTrackSpeed = Decimal.Parse(vs[5]);

            // 解析追踪模式 ('B' for Bottom Track, 'P' for GPS)
            TrackingMode = vs[7][0];
        }
    }
}
