using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSystems.Data
{
    public class StarSystemInfo
    {
        public StarSystemInfo(SunInfo Sun, double SemiMajorAxis)
        {
            this.Sun = Sun;
            this.SemiMajorAxis = SemiMajorAxis;
            Stars = new List<StarInfo>();
        }
        public StarSystemInfo(SunInfo Sun, double SemiMajorAxis, List<StarInfo> Stars)
        {

            this.Sun = Sun;
            this.SemiMajorAxis = SemiMajorAxis;
            this.Stars = Stars;
        }
        public List<StarInfo> Stars { get; set; }
        public SunInfo Sun { get; set; }
        public double SemiMajorAxis { get; set; }
    }

    
}
