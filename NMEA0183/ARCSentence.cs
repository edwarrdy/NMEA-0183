using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// VDARC - Alert Command Refused
    /// </summary>
    public class ARCSentence : NMEASentence
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public decimal Seconds { get; set; }
        public string AlertDescription { get; set; }
        public decimal RelatedValue1 { get; set; }
        public decimal RelatedValue2 { get; set; }
        public char StatusCode { get; set; }

        public ARCSentence()
        {
            SentenceIdentifier = "ARC";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("${0}{1},{2:D2},{3:D2},{4:0.00},{5},{6},{7},{8}\r\n",
                TalkerIdentifier.ToString(), SentenceIdentifier,
                Hours, Minutes, Seconds,
                AlertDescription, RelatedValue1, RelatedValue2, StatusCode);

            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);
            string[] vs = sentence.Split(new char[] { ',', '*' });

            // 解析时间
            Hours = int.Parse(vs[1]);
            Minutes = int.Parse(vs[2]);
            Seconds = decimal.Parse(vs[3]);
            AlertDescription = vs[4];
            RelatedValue1 = decimal.Parse(vs[5]);
            RelatedValue2 = decimal.Parse(vs[6]);
            StatusCode = char.Parse(vs[7]);
        }
    }
}
