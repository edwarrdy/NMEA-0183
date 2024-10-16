using System;
using System.Collections.Generic;
using System.Text;

namespace edward.NMEA0183
{
    /// <summary>
    /// VDALC - Cyclic Alert List
    /// </summary>
    public class ALCSentence : NMEASentence
    {
        public List<AlertItem> AlertItems { get; set; } = new List<AlertItem>();

        public ALCSentence()
        {
            SentenceIdentifier = "ALC";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            foreach (var item in AlertItems)
            {
                stringBuilder.AppendFormat("{0},{1},{2},{3},{4},",
                    item.AlertType, item.AlertNumber, item.AlertThreshold,
                    item.Description, item.TriggerValue);
            }

            // Remove last comma
            if (AlertItems.Count > 0)
            {
                stringBuilder.Length--;
            }

            Byte checksum = CalculateChecksum(stringBuilder.ToString());
            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);
            string[] vs = sentence.Split(new char[] { ',', '*' });

            for (int i = 1; i < vs.Length - 1; i += 5)
            {
                if (i + 4 < vs.Length)
                {
                    AlertItems.Add(new AlertItem
                    {
                        AlertType = vs[i],
                        AlertNumber = vs[i + 1],
                        AlertThreshold = Decimal.Parse(vs[i + 2]),
                        Description = vs[i + 3],
                        TriggerValue = Decimal.Parse(vs[i + 4])
                    });
                }
            }
        }
    }

    public class AlertItem
    {
        public string AlertType { get; set; }
        public string AlertNumber { get; set; }
        public decimal AlertThreshold { get; set; }
        public string Description { get; set; }
        public decimal TriggerValue { get; set; }
    }
}
