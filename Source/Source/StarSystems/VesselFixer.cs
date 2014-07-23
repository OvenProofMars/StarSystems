using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StarSystems
{
    /// <summary>
    /// Withouth VesselFixer your spaceships will crash into the nearest planet when going to the trackingstation
    /// </summary>
    public class VesselFixer : MonoBehaviour
    {
        /// <summary>
        /// When entering trackingstation
        /// </summary>
        void Update()
        {
            if (HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (StarSystem.Initialized == false)
                {
                    Debug.Log("Moving standard planets...");
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

                    Debug.Log("Standard planets moved");

                    StarSystem.Initialized = true;
                }
            }
        }
    }
}
