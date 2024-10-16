using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// DPT - Depth of Water
    /// </summary>
    public class DPTSentence : NMEASentence
    {
        /// <summary>
        /// Depth in Metres
        /// </summary>
        public decimal DepthMetres { get; set; }

        /// <summary>
        /// Offset from transducer;
        /// positive means distance from transducer to water line,
        /// negative means distance from transducer to keel
        /// </summary>
        public decimal OffsetFromTransducerMetres { get; set; }

        /// <summary>
        /// Maximum Depth Range of the transducer in Metres
        /// </summary>
        public decimal MaximumDepthRangeMetres { get; set; }

        public DPTSentence()
        {
            SentenceIdentifier = "DPT";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", DepthMetres.ToString());

            stringBuilder.AppendFormat("{0},", OffsetFromTransducerMetres.ToString());

            stringBuilder.AppendFormat("{0}", MaximumDepthRangeMetres.ToString());

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // x.x Water Depth
            DepthMetres = decimal.Parse(vs[1]);

            // x.x Transducer Offset
            OffsetFromTransducerMetres = decimal.Parse(vs[2]);

            // x.x Maximum Range Scale
            if (vs.Length > 4)
                MaximumDepthRangeMetres = decimal.Parse(vs[3]);
        }
    }
}
