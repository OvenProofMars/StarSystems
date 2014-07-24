using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StarSystems
{
    class Planet : MonoBehaviour
    {
        public static void CreatePlanet(string TemplateName, string Name, string ReferenceBody, int FlightGlobalIndex, double SMA)
        {
            var PlanetClone = (PSystemBody)Instantiate(StarSystem.PSBDict[TemplateName]);
            PlanetClone.children.Clear();
            PlanetClone.flightGlobalsIndex = FlightGlobalIndex;
            PlanetClone.celestialBody.bodyName = Name;
            PlanetClone.orbitDriver.orbit = new Orbit(0, 0, SMA, 0, 0, 0, 0, StarSystem.PSBDict[ReferenceBody].celestialBody);
            PlanetClone.celestialBody.CBUpdate();
            StarSystem.PSBDict[ReferenceBody].children.Add(PlanetClone);
            StarSystem.PSBDict[PlanetClone.celestialBody.bodyName] = PlanetClone;

        }
    }
}
