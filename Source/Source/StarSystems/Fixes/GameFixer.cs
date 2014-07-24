using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using StarSystems.Utils;

namespace StarSystems.Fixes
{
    /// <summary>
    /// Withouth VesselFixer your spaceships will crash into the nearest planet when going to the trackingstation
    /// </summary>
    public class GameFixer : MonoBehaviour
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
                    MoveStandardPlanets.MoveToKerbol();

                    var PatchedSaveGames = ConfigNode.Load("GameData/StarSystems/Config/PatchedSaveGames.cfg");
                        
                    if (PatchedSaveGames.GetNode("PatchedSaveGames").GetValue(HighLogic.CurrentGame.Title) == null)
                    {
                        PatchedSaveGames.GetNode("PatchedSaveGames").AddValue(HighLogic.CurrentGame.Title, "Patched");
                        PatchedSaveGames.Save("GameData/StarSystems/Config/PatchedSaveGames.cfg");
                        StarSystem.NeedsPatching = true;
                    }
                }
            }

            if (StarSystem.NeedsPatching == true && FlightGlobals.Vessels.Count != 0)
            {
                foreach (var Vessel in FlightGlobals.Vessels)
                {
                    if (Vessel.orbitDriver.orbit.referenceBody == StarSystem.CBDict["Sun"])
                    {
                        Debug.Log("Patching " + Vessel.name);
                        Vessel.orbitDriver.referenceBody = StarSystem.CBDict["Kerbol"];
                        Vessel.orbitDriver.UpdateOrbit();
                        Debug.Log(Vessel.name + "Patched");
                    }
                }

                StarSystem.NeedsPatching = false;
            }
        }
    }
}
