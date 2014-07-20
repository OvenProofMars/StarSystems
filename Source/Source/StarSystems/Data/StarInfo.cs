using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSystems.Data
{
    public class StarInfo
    {
        public StarInfo(string Name, string BodyDescription, PlanetColor StarColor, double Inclination, double Eccentricity, double SemiMajorAxis, double LAN, double ArgumentOfPeriapsis, double MeanAnomalyAtEpoch, double Epoch, double Mass, double Radius, int FlightGlobalsIndex, float ScienceMultiplier)
        {
            this.Name = Name;
            this.BodyDescription = BodyDescription;
            this.StarColor = StarColor;
            this.Inclination = Inclination;
            this.Eccentricity = Eccentricity;
            this.SemiMajorAxis = SemiMajorAxis;
            this.LAN = LAN;
            this.ArgumentOfPeriapsis = ArgumentOfPeriapsis;
            this.MeanAnomalyAtEpoch = MeanAnomalyAtEpoch;
            this.Epoch = Epoch;
            this.Radius = Radius;
            this.FlightGlobalsIndex = flightGlobalsIndex;
            this.ScienceMultiplier = ScienceMultiplier;
        }

        public StarInfo()
        {
               
        }
        public string Name { get; set; }
        public string BodyDescription { get; set; }
        public PlanetColor StarColor { get; set; }
        public double Inclination { get; set; }
        public double Eccentricity { get; set; }
        public double SemiMajorAxis { get; set; }
        public double LAN { get; set; }
        public double ArgumentOfPeriapsis { get; set; }
        public double MeanAnomalyAtEpoch { get; set; }
        public double Epoch { get; set; }
        public double Mass { get; set; }
        public double Radius { get; set; }
        public int FlightGlobalsIndex { get; set; }
        public float ScienceMultiplier { get; set; }
    }
}
