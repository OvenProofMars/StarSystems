using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarSystems.Utils;

namespace StarSystems.Data
{
    public enum SunType
    {
        Blackhole = 0,
        Sun = 1
    }

    public enum PlanetColor
    {
        [EnumDescriptionAttribute("Blue")]
        Blue,
        [EnumDescriptionAttribute("Red")]
        Red,
        [EnumDescription("Yellow")]
        Yellow
    }

}
