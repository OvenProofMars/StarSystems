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
            if (HighLogic.LoadedScene == GameScenes.TRACKSTATION && StarSystem.Initialized == false)
            {
                //Update the orbitdrivers
                foreach (OrbitDriver orb in Planetarium.Orbits)
                {
                    orb.UpdateOrbit();
                }

                //Incase a vessel get lost, reload it
                foreach (ProtoVessel Pves in HighLogic.CurrentGame.flightState.protoVessels)
                {
                    if (Pves.vesselRef == null)
                    {
                        Pves.Load(HighLogic.CurrentGame.flightState);
                    }
                }
            }

            //Once a flight has been started vessels wont crash into planets anymore....I hope.....
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && StarSystem.Initialized == false)
            {
                StarSystem.Initialized = true;
            }
        }
    }
}
