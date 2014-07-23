using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarSystems
{
    class PSystemBodies : MonoBehaviour
    {
        public static void GrabPSystemBodies(PSystemBody PSB)
        {
            StarSystem.PSBDict[PSB.celestialBody.bodyName] = PSB;
            Debug.Log(PSB.celestialBody.bodyName);

            foreach (var ChildPSB in PSB.children)
            {
                GrabPSystemBodies(ChildPSB);
            }
        }
    }
}
