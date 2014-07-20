using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarSystems.Data;
using StarSystems.Utils;
using UnityEngine;

namespace StarSystems
{
    public class StarSystem : MonoBehaviour
    {
        public static Dictionary<string, CelestialBody> CBDict = new Dictionary<string, CelestialBody>();
        public static Dictionary<string, Transform> TFDict = new Dictionary<string, Transform>();
        private static Dictionary<string, StarSystemDefintion> StarDict = new Dictionary<string, StarSystemDefintion>();

        private static List<string> StandardPlanets = new List<string>
        {
            "Moho",
            "Eve",
            "Kerbin",
            "Duna",
            "Dres",
            "Jool",
            "Eeloo"
        };

        private ConfigNode StarNames;
        private KspSystemDefinition kspSystemDefinition;
        public static bool Initialized = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            if (ConfigSolarNodes.Instance.IsValid("system"))
            {
                kspSystemDefinition = ConfigSolarNodes.Instance.GetConfigData();
                //Load Kerbol
                var Kerbol = new StarSystemDefintion();
                Kerbol.name = "Kerbol";
                Kerbol.inclination = 0;
                Kerbol.eccentricity = 0;
                Kerbol.semiMajorAxis = kspSystemDefinition.SemiMajorAxis;
                Kerbol.LAN = 0;
                Kerbol.argumentOfPeriapsis = 0;
                Kerbol.meanAnomalyAtEpoch = 0;
                Kerbol.epoch = 0;
                Kerbol.Mass = 1.7565670E28;
                Kerbol.Radius = 261600000d;
                Kerbol.flightGlobalsIndex = 0;
                Kerbol.StarColor = PlanetColor.Yellow;
                Kerbol.ScienceMultiplier = 1f;
                Kerbol.BodyDescription =
                    "The Sun is the most well known object in the daytime sky. Scientists have noted a particular burning sensation and potential loss of vision if it is stared at for long periods of time. This is especially important to keep in mind considering the effect shiny objects have on the average Kerbal.";
                kspSystemDefinition.Stars.Add(Kerbol);

                Debug.Log("Starinfo loaded");
            }
            else
            {

                //kill the mod for bad config
                Debug.Log("Config for ksp Mod stoped working");
                kspSystemDefinition = null;
            }

        }

        public void OnLevelWasLoaded(int level)
        {
            if (kspSystemDefinition != null)
            {
                Debug.Log("Level: " + level);

                if (level == 10)
                {
                    //Set max zoom
                    PlanetariumCamera.fetch.maxDistance = 5000000000;

                    Debug.Log("Creating basis for new stars...");

                    //Create base for new stars
                    foreach (StarSystemDefintion Star in StarDict.Values)
                    {
                        //Grab Sun Internal PSystemBody 
                        var InternalSunPSB = PSystemManager.Instance.systemPrefab.rootBody;
                        var InternalSunCB = InternalSunPSB.celestialBody;

                        //Instantiate Sun Internal PSystemBody
                        var InternalStarPSB = (PSystemBody) Instantiate(InternalSunPSB);
                        var InternalStarCB = InternalStarPSB.celestialBody;

                        //Set Star CB and PSB Parameters
                        InternalStarPSB.name = Star.name;
                        InternalStarPSB.flightGlobalsIndex = Star.flightGlobalsIndex;
                        InternalStarPSB.children.Clear();

                        InternalStarPSB.enabled = false;

                        InternalStarCB.bodyName = Star.name;

                        InternalStarCB.Radius = Star.Radius;

                        InternalStarCB.CBUpdate();

                        //Add Star to Sun children
                        InternalSunPSB.children.Add(InternalStarPSB);
                    }

                    PSystemManager.Instance.systemPrefab.rootBody.flightGlobalsIndex = -1;

                    Debug.Log("Basis for new stars created");

                    //PsystemReady trigger
                    PSystemManager.Instance.OnPSystemReady.Add(OnPSystemReady);
                }

                if (level == 2)
                {
                    Initialized = false;
                }

                if (level == 5)
                {
                    //Set sun to Kerbol when loading space center
                    Sun.Instance.sun = CBDict["Kerbol"];
                    Planetarium.fetch.Sun = CBDict["Kerbol"];
                }
            }
        }

        public void OnPSystemReady()
        {
            //Add all CelestialBodies to dictionary
            foreach (var PlanetCB in FlightGlobals.fetch.bodies)
            {
                CBDict[PlanetCB.name] = PlanetCB;
            }
            //Add all Scaled transforms to dictionary
            foreach (var ScaledPlanet in ScaledSpace.Instance.scaledSpaceTransforms)
            {
                TFDict[ScaledPlanet.name] = ScaledPlanet;
            }

            Debug.Log("Moving standard planets...");

            //Add all standard planets to Kerbol
            foreach (var OriginalPlanet in StandardPlanets)
            {
                foreach (var PlanetCB in CBDict.Values)
                {
                    if (PlanetCB.name == OriginalPlanet)
                    {
                        PlanetCB.orbitDriver.referenceBody = CBDict["Kerbol"];

                        CBDict["Kerbol"].orbitingBodies.Add(PlanetCB);
                        PlanetCB.orbitDriver.UpdateOrbit();

                        break;
                    }
                }
            }

            CBDict["Kerbol"].CBUpdate();

            Debug.Log("Standard planets moved");

            Debug.Log("Altering sun...");

            //Set Original Sun Parameters
            var OriginalSun = CBDict["Sun"];
            double SolarMasses;

            try
            {
                SolarMasses =
                    double.Parse(
                        ConfigNode.Load("GameData/StarSystems/Configdata/StarNames.cfg")
                            .GetNode("BlackHole")
                            .GetValue("SolarMasses"));
            }
            catch
            {
                SolarMasses = 7700;
            }

            OriginalSun.Mass = SolarMasses*OriginalSun.Mass;
            OriginalSun.Radius = (2*(6.74E-11)*OriginalSun.Mass)/(Math.Pow(299792458, 2.0));
            OriginalSun.GeeASL = OriginalSun.Mass*(6.674E-11/9.81)/Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gMagnitudeAtCenter = OriginalSun.GeeASL*9.81*Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gravParameter = OriginalSun.gMagnitudeAtCenter;

            OriginalSun.scienceValues.InSpaceLowDataValue = OriginalSun.scienceValues.InSpaceLowDataValue*10f;
            OriginalSun.scienceValues.RecoveryValue = OriginalSun.scienceValues.RecoveryValue*5f;

            OriginalSun.bodyName = "Billy-Hadrick";

            OriginalSun.bodyDescription =
                "This recently discovered black hole, named after its discoverer Billy-Hadrick Kerman, is the central point where multiple star systems revolve around.";

            OriginalSun.CBUpdate();

            //Make Sun Black
            var ScaledSun = TFDict["Sun"];
            ScaledSun.renderer.material.SetColor("_EmitColor0", new Color(0.0f, 0.0f, 0.0f, 1));
            ScaledSun.renderer.material.SetColor("_EmitColor1", new Color(0.0f, 0.0f, 0.0f, 1));
            ScaledSun.renderer.material.SetColor("_SunspotColor", new Color(0.0f, 0.0f, 0.0f, 1));
            ScaledSun.renderer.material.SetColor("_RimColor", new Color(0.0f, 0.0f, 0.0f, 1.0f));

            //Update Sun Scale
            var ScaledSunMeshFilter = (MeshFilter) ScaledSun.GetComponent(typeof (MeshFilter));
            var SunRatio = (float) OriginalSun.Radius/261600000f;

            MeshScaler.ScaleMesh(ScaledSunMeshFilter.mesh, SunRatio);

            //Change Sun Corona
            foreach (var SunCorona in ScaledSun.GetComponentsInChildren<SunCoronas>())
            {
                SunCorona.renderer.material.mainTexture =
                    GameDatabase.Instance.GetTexture("StarSystems/Resources/BlackHoleCorona", false);
                var SunCoronaMeshFilter = (MeshFilter) SunCorona.GetComponent(typeof (MeshFilter));
                MeshScaler.ScaleMesh(SunCoronaMeshFilter.mesh, SunRatio);
            }

            Debug.Log("Sun altered");

            //Build out stars
            foreach (var Star in StarDict.Values)
            {

                Debug.Log("Creating " + Star.name + "...");

                var LocalSunCB = CBDict["Sun"];
                var LocalStarCB = CBDict[Star.name];
                var StarRatio = (float) Star.Radius/261600000f;
                var ScaledStar = TFDict[Star.name];
                var ScaledStarMeshFilter = (MeshFilter) ScaledStar.GetComponent(typeof (MeshFilter));

                //Set Star Variables
                LocalStarCB.Mass = Star.Mass;
                LocalStarCB.Radius = Star.Radius;
                LocalStarCB.GeeASL = LocalStarCB.Mass*(6.674E-11/9.81)/Math.Pow(LocalStarCB.Radius, 2.0);
                LocalStarCB.gMagnitudeAtCenter = LocalStarCB.GeeASL*9.81*Math.Pow(LocalStarCB.Radius, 2.0);
                LocalStarCB.gravParameter = LocalStarCB.gMagnitudeAtCenter;
                LocalStarCB.bodyDescription = Star.BodyDescription;

                //Set Science parameters
                LocalStarCB.scienceValues.InSpaceLowDataValue = LocalStarCB.scienceValues.InSpaceLowDataValue*
                                                                Star.ScienceMultiplier;
                LocalStarCB.scienceValues.RecoveryValue = LocalStarCB.scienceValues.RecoveryValue*Star.ScienceMultiplier;

                //Create new Orbitdriver
                OrbitDriver NewOrbitDriver = LocalStarCB.gameObject.AddComponent<OrbitDriver>();

                //Add new OrbitDriver to Kerbol
                LocalStarCB.orbitDriver = NewOrbitDriver;

                //Set OrbitDriver parameters
                LocalStarCB.orbitDriver.name = LocalStarCB.name;
                LocalStarCB.orbitDriver.celestialBody = LocalStarCB;
                LocalStarCB.orbitDriver.referenceBody = LocalSunCB;
                LocalStarCB.orbitDriver.updateMode = OrbitDriver.UpdateMode.UPDATE;
                LocalStarCB.orbitDriver.QueuedUpdate = true;

                //Create new orbit
                LocalStarCB.orbitDriver.orbit = new Orbit(Star.inclination, Star.eccentricity, Star.semiMajorAxis,
                    Star.LAN, Star.argumentOfPeriapsis, Star.meanAnomalyAtEpoch, Star.epoch, LocalSunCB);

                //Calculate SOI
                LocalStarCB.sphereOfInfluence = (LocalStarCB.orbit.semiMajorAxis*
                                                 Math.Pow(LocalStarCB.Mass/LocalStarCB.orbit.referenceBody.Mass, (2.0/5)));
                LocalStarCB.hillSphere = LocalStarCB.orbit.semiMajorAxis*(1.0 - LocalStarCB.orbit.eccentricity)*
                                         Math.Pow((LocalStarCB.Mass/(3.0*LocalStarCB.orbit.referenceBody.Mass)), 1.0/3.0);

                //Update CelestialBody
                LocalStarCB.CBUpdate();

                //Update OrbitDriver
                NewOrbitDriver.UpdateOrbit();

                //Update Star Scale
                MeshScaler.ScaleMesh(ScaledStarMeshFilter.mesh, StarRatio);

                //Update Corona Ratio
                foreach (var StarCorona in ScaledStar.GetComponentsInChildren<SunCoronas>())
                {
                    var StarCoronaMeshFilter = (MeshFilter) StarCorona.GetComponent(typeof (MeshFilter));
                    MeshScaler.ScaleMesh(StarCoronaMeshFilter.mesh, StarRatio);
                }

                //Change to Blue Star
                if (Star.StarColor == "Blue")
                {
                    ScaledStar.renderer.material.SetColor("_EmitColor0", new Color(0.357f, 0.588f, 0.405f, 1));
                    ScaledStar.renderer.material.SetColor("_EmitColor1", new Color(0.139f, 0.061f, 1.0f, 1));
                    ScaledStar.renderer.material.SetColor("_SunspotColor", new Color(1.0f, 1.0f, 1.0f, 1));
                    ScaledStar.renderer.material.SetColor("_RimColor", new Color(0.388f, 0.636f, 1.0f, 1.0f));

                    foreach (var StarCorona in ScaledStar.GetComponentsInChildren<SunCoronas>())
                    {
                        StarCorona.renderer.material.mainTexture =
                            GameDatabase.Instance.GetTexture("StarSystems/Resources/BlueStarCorona", false);
                    }
                }

                //Change to Red Star
                if (Star.StarColor == "Red")
                {
                    ScaledStar.renderer.material.SetColor("_EmitColor0", new Color(0.861f, 0.704f, 0.194f, 1));
                    ScaledStar.renderer.material.SetColor("_EmitColor1", new Color(0.398f, 0.071f, 1.0f, 1));
                    ScaledStar.renderer.material.SetColor("_SunspotColor", new Color(0.01f, 0.003f, 0.007f, 1));
                    ScaledStar.renderer.material.SetColor("_RimColor", new Color(0.626f, 0.231f, 0.170f, 1.0f));

                    foreach (var StarCorona in ScaledStar.GetComponentsInChildren<SunCoronas>())
                    {
                        StarCorona.renderer.material.mainTexture =
                            GameDatabase.Instance.GetTexture("StarSystems/Resources/RedStarCorona", false);
                    }
                }

                Debug.Log(Star.name + " created");

            }

            //Create starlight controller
            var StarLightSwitcherObj = new GameObject("StarLightSwitcher", typeof (StarLightSwitcher));
            GameObject.DontDestroyOnLoad(StarLightSwitcherObj);
            foreach (string StarName in StarDict.Keys)
            {
                //Add stars to dictionary
                StarLightSwitcherObj.GetComponent<StarLightSwitcher>()
                    .AddStar(CBDict[StarName], StarDict[StarName].StarColor);
            }

            Debug.Log("Starlight controller created");

            //Create Navball fixer
            var NavBallFixerObj = new GameObject("NavBallFixer", typeof (NavBallFixer));
            GameObject.DontDestroyOnLoad(NavBallFixerObj);

            Debug.Log("Navball fixer created");

            //Create Vessel fixer
            var VesselFixerObj = new GameObject("SaveGameFixer", typeof (VesselFixer));
            GameObject.DontDestroyOnLoad(VesselFixerObj);

            Debug.Log("Vessel fixer created");
        }
    }
}
