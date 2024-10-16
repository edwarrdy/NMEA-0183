using System;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// HBT - Heartbeat message.
    /// </summary>
    public class HBTSentence : NMEASentence
    {
        /// <summary>
        /// Heartbeat value or status code
        /// </summary>
        public Decimal HeartbeatValue { get; set; }

        /// <summary>
        /// Status (A=Active, V=Void)
        /// </summary>
        public char Status { get; set; }

        public HBTSentence()
        {
            SentenceIdentifier = "HBT";
        }

        public override String EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Start sentence
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            // Add heartbeat value
            stringBuilder.AppendFormat("{0},", HeartbeatValue);

            // Add status
            stringBuilder.AppendFormat("{0}", Status);

            // Calculate and append checksum
            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(String sentence)
        {
            DecodeTalker(sentence);

            // Split sentence by ',' and '*'
            String[] vs = sentence.Split(new char[] { ',', '*' });

            // Parse heartbeat value
            HeartbeatValue = Decimal.Parse(vs[1]);

            // Parse status
            Status = vs[2][0];
        }
    }
}
