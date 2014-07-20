using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSystems.Data
{
    public class KspSystemDefinition
    {
        public KspSystemDefinition(RootDefinition Sun, double SemiMajorAxis)
        {
            this.Sun = Sun;
            this.SemiMajorAxis = SemiMajorAxis;
            Stars = new List<StarSystemDefintion>();
        }
        public KspSystemDefinition(RootDefinition Sun, double SemiMajorAxis, List<StarSystemDefintion> Stars)
        {

            this.Sun = Sun;
            this.SemiMajorAxis = SemiMajorAxis;
            this.Stars = Stars;
        }
        public List<StarSystemDefintion> Stars { get; set; }
        public RootDefinition Sun { get; set; }
        public double SemiMajorAxis { get; set; }
    }

    
}
