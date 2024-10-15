using System;
using System.Text;
using static edward.Maritime.NMEA0183.APBSentence;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// HMR -  Heading Monitor Receive
    /// </summary>
    public class HMRSentence : NMEASentence
    {
        /// <summary>
        /// Heading Sensor 1 ID
        /// </summary>
        public string HeadingSensorOneID { get; set; }

        /// <summary>
        /// Heading Sensor 2 ID
        /// </summary>
        public string HeadingSensorTwoID { get; set; }

        /// <summary>
        /// Difference limit setting, degrees 
        /// </summary>
        public decimal DifferenceLimitSettingDegrees { get; set; }

        /// <summary>
        /// Actual heading sensor difference, degrees
        /// </summary>
        public decimal ActualHeadingSensorDifferenceDegrees { get; set; }

        /// <summary>
        /// <para>Warning flag.</para>
        /// <para>True - difference within set limit.</para>
        /// <para>False - difference exceeds set limit.</para>
        /// </summary>
        public bool WarningFlag { get; set; }

        /// <summary>
        /// Heading reading, Sensor 1, degrees
        /// </summary>
        public decimal HeadingReadingSensorOneDegrees { get; set; }

        /// <summary>
        /// Sensor 1 type, T/M
        /// </summary>
        public SensorTypeEnum SensorOneType { get; set; }

        /// <summary>
        /// Deviation, Sensor 1, degrees E/W,
        /// East - Positive,
        /// West - Negative
        /// </summary>
        public decimal? DeviationSensorOneDegrees { get; set; }

        /// <summary>
        /// Heading reading, Sensor 2, degrees
        /// </summary>
        public decimal HeadingReadingSensorTwoDegrees { get; set; }

        /// <summary>
        /// Sensor 2 type, T/M
        /// </summary>
        public SensorTypeEnum SensorTwoType { get; set; }

        /// <summary>
        /// Deviation, Sensor 2, degrees E/W,
        /// East - Positive,
        /// West - Negative
        /// </summary>
        public decimal? DeviationSensorTwoDegrees { get; set; }

        /// <summary>
        /// Variation, degrees E/W,
        /// East - Positive,
        /// West - Negative
        /// </summary>
        public decimal? VariationDegrees { get; set; }

        public HMRSentence()
        {
            SentenceIdentifier = "HMR";
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            stringBuilder.AppendFormat("{0},", HeadingSensorOneID);

            stringBuilder.AppendFormat("{0},", HeadingSensorTwoID);

            stringBuilder.AppendFormat("{0},", DifferenceLimitSettingDegrees);

            stringBuilder.AppendFormat("{0},", ActualHeadingSensorDifferenceDegrees);

            stringBuilder.AppendFormat("{0},", WarningFlag ? "A" : "V");

            stringBuilder.AppendFormat("{0},A,", HeadingReadingSensorOneDegrees);

            stringBuilder.AppendFormat("{0},", SensorOneType.ToString());

            if (DeviationSensorOneDegrees.HasValue)
            {
                if (DeviationSensorOneDegrees.Value < 0)
                {
                    stringBuilder.AppendFormat("{0},W,", Math.Abs(DeviationSensorOneDegrees.Value));
                }
                else
                {
                    stringBuilder.AppendFormat("{0},E,", Math.Abs(DeviationSensorOneDegrees.Value));
                }
            }
            else
            {
                stringBuilder.Append(",,");
            }

            stringBuilder.AppendFormat("{0},A,", HeadingReadingSensorTwoDegrees);

            stringBuilder.AppendFormat("{0},", SensorTwoType.ToString());

            if (DeviationSensorTwoDegrees.HasValue)
            {
                if (DeviationSensorTwoDegrees.Value < 0)
                {
                    stringBuilder.AppendFormat("{0},W,", Math.Abs(DeviationSensorTwoDegrees.Value));
                }
                else
                {
                    stringBuilder.AppendFormat("{0},E,", Math.Abs(DeviationSensorTwoDegrees.Value));
                }
            }
            else
            {
                stringBuilder.Append(",,");
            }

            if (VariationDegrees.HasValue)
            {
                if (VariationDegrees.Value < 0)
                {
                    stringBuilder.AppendFormat("{0},W", Math.Abs(VariationDegrees.Value));
                }
                else
                {
                    stringBuilder.AppendFormat("{0},E", Math.Abs(VariationDegrees.Value));
                }
            }
            else
            {
                stringBuilder.Append(",");
            }

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // c--c Heading Sensor 1 ID
            HeadingSensorOneID = vs[1];

            // c--c Heading Sensor 2 ID
            HeadingSensorTwoID = vs[2];

            // x.x Difference limit setting, degrees
            DifferenceLimitSettingDegrees = decimal.Parse(vs[3]);

            // x.x Actual heading sensor difference, degrees 
            ActualHeadingSensorDifferenceDegrees = decimal.Parse(vs[4]);

            // A Warning flag
            WarningFlag = vs[5] == "A";

            // x.x Heading reading, Sensor 1, degrees
            HeadingReadingSensorOneDegrees = decimal.Parse(vs[6]);

            // a Sensor 1 type, T/M
            SensorOneType = (SensorTypeEnum)Enum.Parse(typeof(SensorTypeEnum), vs[8]);

            // x.x Deviation, Sensor 1, degrees
            if (string.IsNullOrWhiteSpace(vs[9]))
            {
                DeviationSensorOneDegrees = null;
            }
            else
            {
                DeviationSensorOneDegrees = decimal.Parse(vs[9]);

                if (vs[10] == "W")
                    DeviationSensorOneDegrees *= decimal.MinusOne;
            }

            // x.x Heading reading, Sensor 2, degrees
            HeadingReadingSensorTwoDegrees = decimal.Parse(vs[11]);

            // a Sensor 2 type, T/M
            SensorTwoType = (SensorTypeEnum)Enum.Parse(typeof(SensorTypeEnum), vs[13]);

            // x.x Deviation, Sensor 2, degrees
            if (string.IsNullOrWhiteSpace(vs[14]))
            {
                DeviationSensorTwoDegrees = null;
            }
            else
            {
                DeviationSensorTwoDegrees = decimal.Parse(vs[14]);

                if (vs[15] == "W")
                    DeviationSensorTwoDegrees *= decimal.MinusOne;
            }

            // x.x Variation, degrees E/W
            if (string.IsNullOrWhiteSpace(vs[16]))
            {
                VariationDegrees = null;
            }
            else
            {
                VariationDegrees = decimal.Parse(vs[16]);

                if (vs[17] == "W")
                    VariationDegrees *= decimal.MinusOne;
            }
        }

        public enum SensorTypeEnum
        {
            /// <summary>
            /// T - True
            /// </summary>
            T,
            /// <summary>
            /// M - Magnetic
            /// </summary>
            M,
        }
    }
}
