using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarSystems.Data;
using UnityEngine;

namespace StarSystems.Creator
{
    public class CenterRoot
    {
        private static CenterRoot instance;

        private CenterRoot()
        {
        }

        public static CenterRoot Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CenterRoot();
                }
                return instance;
            }
        }
        public void OnPSystemReady(RootDefinition Root, CelestialBody OriginalSun, Transform ScaledSun)
        {
            Debug.Log("Altering sun...");

            //Set Original Sun Parameters
            double SolarMasses;


            SolarMasses = Root.SolarMasses;

            OriginalSun.Mass = SolarMasses * OriginalSun.Mass;
            OriginalSun.Radius = (2 * (6.74E-11) * OriginalSun.Mass) / (Math.Pow(299792458, 2.0));
            OriginalSun.GeeASL = OriginalSun.Mass * (6.674E-11 / 9.81) / Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gMagnitudeAtCenter = OriginalSun.GeeASL * 9.81 * Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gravParameter = OriginalSun.gMagnitudeAtCenter;

            OriginalSun.scienceValues.InSpaceLowDataValue = OriginalSun.scienceValues.InSpaceLowDataValue * 10f;
            OriginalSun.scienceValues.RecoveryValue = OriginalSun.scienceValues.RecoveryValue * 5f;

            OriginalSun.bodyName = "Blacky Karman";

            OriginalSun.bodyDescription =
                "This recently discovered black hole, named after its discoverer Billy-Hadrick Kerman, is the central point where multiple star systems revolve around.";

            OriginalSun.CBUpdate();

            //Make Sun Black
            ScaledSun.renderer.material.SetColor("_EmitColor0", new Color(0.0f, 0.0f, 0.0f, 1));
            ScaledSun.renderer.material.SetColor("_EmitColor1", new Color(0.0f, 0.0f, 0.0f, 1));
            ScaledSun.renderer.material.SetColor("_SunspotColor", new Color(0.0f, 0.0f, 0.0f, 1));
            ScaledSun.renderer.material.SetColor("_RimColor", new Color(0.0f, 0.0f, 0.0f, 1.0f));

            //Update Sun Scale
            var ScaledSunMeshFilter = (MeshFilter)ScaledSun.GetComponent(typeof(MeshFilter));
            var SunRatio = (float)OriginalSun.Radius / 261600000f;

            MeshScaler.ScaleMesh(ScaledSunMeshFilter.mesh, SunRatio);

            //Change Sun Corona
            foreach (var SunCorona in ScaledSun.GetComponentsInChildren<SunCoronas>())
            {
                SunCorona.renderer.material.mainTexture =
                    GameDatabase.Instance.GetTexture("StarSystems/Resources/BlackHoleCorona", false);
                var SunCoronaMeshFilter = (MeshFilter)SunCorona.GetComponent(typeof(MeshFilter));
                MeshScaler.ScaleMesh(SunCoronaMeshFilter.mesh, SunRatio);
            }

            Debug.Log("Sun altered");
        }
    }
}
