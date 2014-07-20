using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSystems.Data
{
    public class SunInfo
    {
        public SunInfo(double SolarMasses, SunType SunType)
        {
            this.SolarMasses = SolarMasses;
            this.SunType = SunType;
        }
        public double SolarMasses { get; set; }
        public SunType SunType { get; set; }
    }
}
