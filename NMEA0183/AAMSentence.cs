using System;
using System.Text;

namespace edward.Maritime.NMEA0183
{
    /// <summary>
    /// AAM - Waypoint Arrival Alarm
    /// </summary>
    public class AAMSentence : NMEASentence
    {
        /// <summary>
        /// <para>Arrival Circle Entered Status.</para>
        /// <para>True - Arrival Circle Entered.</para>
        /// <para>False - Arrival Circle not Entered.</para>
        /// </summary>
        public bool IsArrivalCircleEntered { get; set; }

        /// <summary>
        /// <para>Perpendicular Passed Status.</para>
        /// <para>True - Perpendicular Passed at Waypoint.</para>
        /// <para>False - Perpendicular not Passed.</para>
        /// </summary>
        public bool IsPerpendicularPassed { get; set; }

        /// <summary>
        /// Arrival circle radius in Nautical Miles.
        /// </summary>
        public decimal ArrivalCircleRadiusNauticalMiles { get; set; }

        /// <summary>
        /// Waypoint ID
        /// </summary>
        public string WaypointID { get; set; }

        public AAMSentence()
        {
            SentenceIdentifier = "AAM";
            IsArrivalCircleEntered = false;
            IsPerpendicularPassed = false;
        }

        public override string EncodeSentence()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("${0}{1},", TalkerIdentifier.ToString(), SentenceIdentifier);

            if (IsArrivalCircleEntered)
                stringBuilder.Append("A,");
            else
                stringBuilder.Append("V,");

            if (IsPerpendicularPassed)
                stringBuilder.Append("A,");
            else
                stringBuilder.Append("V,");

            stringBuilder.AppendFormat("{0},N,", ArrivalCircleRadiusNauticalMiles.ToString());

            stringBuilder.AppendFormat("{0}", WaypointID);

            byte checksum = CalculateChecksum(stringBuilder.ToString());

            stringBuilder.AppendFormat("*{0}\r\n", checksum.ToString("X2"));

            return stringBuilder.ToString();
        }

        protected override void DecodeInternalSentence(string sentence)
        {
            DecodeTalker(sentence);

            string[] vs = sentence.Split(new char[] { ',', '*' });

            // A Arrival Circle Status
            if (vs[1] == "A")
                IsArrivalCircleEntered = true;
            else
                IsArrivalCircleEntered = false;

            // A Perpendicular Passed Status
            if (vs[2] == "A")
                IsPerpendicularPassed = true;
            else
                IsPerpendicularPassed = false;

            // x.x Arrival circle radius
            ArrivalCircleRadiusNauticalMiles = decimal.Parse(vs[3]);

            // c--c Waypoint ID
            WaypointID = vs[5];
        }
    }
}
