using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StarSystems.Utils
{
    class MoveStandardPlanets
    {
        public static void MoveToKerbol()
        {
            Debug.Log("Moving standard planets to Kerbol...");
            //Add all standard planets to Kerbol
            foreach (var OriginalPlanet in StarSystem.StandardPlanets)
            {
                foreach (var PlanetCB in StarSystem.CBDict.Values)
                {
                    if (PlanetCB.name == OriginalPlanet)
                    {
                        PlanetCB.orbitDriver.referenceBody = StarSystem.CBDict["Kerbol"];
                        StarSystem.CBDict["Sun"].orbitingBodies.Remove(PlanetCB);
                        StarSystem.CBDict["Kerbol"].orbitingBodies.Add(PlanetCB);
                        PlanetCB.orbitDriver.UpdateOrbit();

                        break;
                    }
                }
            }
            StarSystem.CBDict["Kerbol"].CBUpdate();
            StarSystem.CBDict["Sun"].CBUpdate();

            StarSystem.Initialized = true;

            Debug.Log("Standard planets moved to Kerbol");
        }

        public static void MoveToSun()
        {
            Debug.Log("Moving standard planets to Sun...");
            //Add all standard planets to Kerbol
            foreach (var OriginalPlanet in StarSystem.StandardPlanets)
            {
                foreach (var PlanetCB in StarSystem.CBDict.Values)
                {
                    if (PlanetCB.name == OriginalPlanet)
                    {
                        PlanetCB.orbitDriver.referenceBody = StarSystem.CBDict["Sun"];
                        StarSystem.CBDict["Kerbol"].orbitingBodies.Remove(PlanetCB);
                        StarSystem.CBDict["Sun"].orbitingBodies.Add(PlanetCB);
                        PlanetCB.orbitDriver.UpdateOrbit();

                        break;
                    }
                }
            }
            StarSystem.CBDict["Kerbol"].CBUpdate();
            StarSystem.CBDict["Sun"].CBUpdate();

            StarSystem.Initialized = false;

            Debug.Log("Standard planets moved to Sun");
        }
    }
}
