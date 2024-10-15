using System;
using System.Collections.Generic;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// Q - Query Sentence
    /// </summary>
    public class QuerySentence : NMEASentence
    {
        /// <summary>
        /// Listener Identifier
        /// </summary>
        public TalkerIdentifier ListenerIdentifier { get; set; }

        /// <summary>
        /// Sentence Identifier
        /// </summary>
        public string QuerySentenceIdentifier { get; set; }

        public QuerySentence()
        {
            SentenceIdentifier = "Q";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1}{2},", TalkerIdentifier.ToString(), ListenerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0}", QuerySentenceIdentifier);

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            ListenerIdentifier = TalkerIdentifier.FromString(vs[0].Substring(3, 2));

            QuerySentenceIdentifier = vs[1];
        }
    }
}
