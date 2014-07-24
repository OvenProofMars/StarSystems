using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSystems.Data
{
    public class KspSystemDefinition
    {
        public KspSystemDefinition(RootDefinition Root, double SemiMajorAxis)
        {
            this.Root = Root;
            this.SemiMajorAxis = SemiMajorAxis;
            Stars = new List<StarSystemDefintion>();
        }
        public KspSystemDefinition(RootDefinition Root, double SemiMajorAxis, List<StarSystemDefintion> Stars)
        {

            this.Root = Root;
            this.SemiMajorAxis = SemiMajorAxis;
            this.Stars = Stars;
        }
        public List<StarSystemDefintion> Stars { get; set; }
        public RootDefinition Root { get; set; }
        public double SemiMajorAxis { get; set; }
    }

    
}
