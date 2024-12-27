using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// RMC - Recommended Minimum Specific GPS/Transit Data
    /// </summary>
    public class RMCSentence : NMEASentence
    {
        public DateTime UtcTime { get; set; } // UTC 时间
        public char Status { get; set; } // 状态（A = 有效，V = 警告）
        public decimal Latitude { get; set; } // 纬度
        public char LatitudeDirection { get; set; } // 纬度方向（N = 北，S = 南）
        public decimal Longitude { get; set; } // 经度
        public char LongitudeDirection { get; set; } // 经度方向（E = 东，W = 西）
        public decimal Speed { get; set; } // 水平速度（以节为单位）
        public decimal Course { get; set; } // 航向
        public DateTime Date { get; set; } // 日期
        public decimal MagneticVariation { get; set; } // 磁偏角
        public char MagneticVariationDirection { get; set; } // 磁偏角方向（E = 东，W = 西）
        public string FAAIndicator { get; set; } // FAA 模式指示（NMEA 2.3 及更高版本）
        public string NavStatus { get; set; } // 导航状态（NMEA 4.1 及更高版本）

        public RMCSentence()
        {
            SentenceIdentifier = "RMC";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier, SentenceIdentifier);
            stringBuilder.AppendFormat("{0},{1},", UtcTime.ToString("HHmmss.fff"), Status);
            stringBuilder.AppendFormat("{0},{1},", Latitude, LatitudeDirection);
            stringBuilder.AppendFormat("{0},{1},", Longitude, LongitudeDirection);
            stringBuilder.AppendFormat("{0},{1},", Speed, Course);
            stringBuilder.AppendFormat("{0},", Date.ToString("ddMMyy"));
            stringBuilder.AppendFormat("{0},{1},", MagneticVariation, MagneticVariationDirection);
            stringBuilder.AppendFormat("{0},", FAAIndicator);
            stringBuilder.AppendFormat("{0},", NavStatus);

            byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);
            string[] vs = sentence.Split(new char[] { ',', '*' });

            UtcTime = DateTime.ParseExact(vs[1], "HHmmss.ff", null);
            Status = vs[2][0];
            Latitude = decimal.Parse(vs[3]);
            LatitudeDirection = vs[4][0];
            Longitude = decimal.Parse(vs[5]);
            LongitudeDirection = vs[6][0];
            Speed = decimal.Parse(vs[7]);
            Course = decimal.Parse(vs[8]);
            Date = DateTime.ParseExact(vs[9], "ddMMyy", null);

            // 解析磁偏角
            if (vs.Length > 10)
            {
                MagneticVariation = decimal.Parse(vs[10]);
                MagneticVariationDirection = vs[11][0];
            }

            // 解析 FAA 模式指示
            if (vs.Length > 12)
            {
                FAAIndicator = vs[12];
            }

            // 解析导航状态
            if (vs.Length > 13)
            {
                NavStatus = vs[13];
            }
        }
    }
}
