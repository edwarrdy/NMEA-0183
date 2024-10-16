using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// MTW - Water Temperature Sentence
    /// </summary>
    public class MTWSentence : NMEASentence
    {
        /// <summary>
        /// Water temperature in Celsius
        /// </summary>
        public Decimal WaterTemperature { get; set; }

        public MTWSentence()
        {
            SentenceIdentifier = "MTW";
        }

        public override String EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // 开始生成NMEA字符串
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            // 添加水温数据
            stringBuilder.AppendFormat("{0},C", WaterTemperature);

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

            // 解析水温 (x.x 表示水温，单位是摄氏度)
            WaterTemperature = Decimal.Parse(vs[1]);
        }
    }
}
