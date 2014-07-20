using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using System.Text;

namespace StarSystems
{

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class StarSystems : MonoBehaviour
    {
        public static Dictionary<string, CelestialBody> CBDict = new Dictionary<string, CelestialBody>();
        public static Dictionary<string, Transform> TFDict = new Dictionary<string, Transform>();
        private static Dictionary<string, StarInfo> StarDict = new Dictionary<string, StarInfo>();
        private static List<string> StandardPlanets = new List<string> { "Moho", "Eve", "Kerbin", "Duna", "Dres", "Jool", "Eeloo" };
        private ConfigNode StarNames;
        public static bool Initialized = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        private void Start()
        {
            //Set config file location
            StarNames = ConfigNode.Load("GameData/StarSystems/Configdata/StarNames.cfg");

            Debug.Log("Reading star info...");

            //Grab star info
            foreach (var star in StarNames.GetNodes("Star"))
            {
                var StarClass = new StarInfo();

                try
                {
                    StarClass.name = star.GetNode("CelestialBody").GetValue("name");
                }
                catch (Exception e)
                {
                    Debug.Log("No star name found, can't create star");
                    continue;
                }

                try
                {
                    StarClass.BodyDescription = star.GetNode("CelestialBody").GetValue("BodyDescription");
                }
                catch (Exception e)
                {

                }
                try
                {
                    StarClass.flightGlobalsIndex = int.Parse(star.GetNode("CelestialBody").GetValue("flightGlobalIndex"));
                }
                catch (Exception e)
                {
                    Debug.Log("No flightGlobalIndex found, can't create star");
                    continue;
                }
                try
                {
                    StarClass.StarColor = star.GetNode("CelestialBody").GetValue("StarColor");
                }
                catch (Exception e)
                {
                    StarClass.StarColor = "Yellow";
                }
                try
                {
                    StarClass.Mass = double.Parse(star.GetNode("CelestialBody").GetValue("Mass"));
                }
                catch (Exception e)
                {
                    StarClass.Mass = 1.7565670E28;
                }
                try
                {
                    StarClass.Radius = double.Parse(star.GetNode("CelestialBody").GetValue("Radius"));
                }
                catch (Exception e)
                {
                    StarClass.Radius = 261600000;
                }
                try
                {
                    StarClass.ScienceMultiplier = float.Parse(star.GetNode("CelestialBody").GetValue("ScienceMultiplier"));
                }
                catch (Exception e)
                {
                    StarClass.ScienceMultiplier = 10f;
                }
                try
                {
                    StarClass.inclination = double.Parse(star.GetNode("Orbit").GetValue("inclination"));
                }
                catch (Exception e)
                {
                    StarClass.inclination = 0;
                }
                try
                {
                    StarClass.eccentricity = double.Parse(star.GetNode("Orbit").GetValue("eccentricity"));
                }
                catch (Exception e)
                {
                    StarClass.eccentricity = 0;
                }
                try
                {
                    StarClass.semiMajorAxis = double.Parse(star.GetNode("Orbit").GetValue("semiMajorAxis"));
                }
                catch (Exception e)
                {
                    Debug.Log("No semiMajorAxis found, can't create star");
                    continue;
                }
                try
                {
                    StarClass.LAN = double.Parse(star.GetNode("Orbit").GetValue("LAN"));
                }
                catch (Exception e)
                {
                    StarClass.LAN = 0;
                }
                try
                {
                    StarClass.argumentOfPeriapsis = double.Parse(star.GetNode("Orbit").GetValue("argumentOfPeriapsis"));
                }
                catch (Exception e)
                {
                    StarClass.LAN = 0;
                }
                try
                {
                    StarClass.meanAnomalyAtEpoch = double.Parse(star.GetNode("Orbit").GetValue("meanAnomalyAtEpoch"));
                }
                catch (Exception e)
                {
                    StarClass.meanAnomalyAtEpoch = 0;
                }
                try
                {
                    StarClass.epoch = double.Parse(star.GetNode("Orbit").GetValue("epoch"));
                }
                catch (Exception e)
                {
                    StarClass.epoch = 0;
                }

                StarDict[StarClass.name] = StarClass;
            }

            //Load Kerbol
            var Kerbol = new StarInfo();
            Kerbol.name = "Kerbol";

            Kerbol.inclination = 0;
            Kerbol.eccentricity = 0;
            try
            {
                Kerbol.semiMajorAxis = double.Parse(ConfigNode.Load("GameData/StarSystems/Configdata/StarNames.cfg").GetNode("Kerbol").GetValue("semiMajorAxis"));
            }
            catch
            {
                Kerbol.semiMajorAxis = 4500000000000;
            }
            Kerbol.LAN = 0;
            Kerbol.argumentOfPeriapsis = 0;
            Kerbol.meanAnomalyAtEpoch = 0;
            Kerbol.epoch = 0;

            Kerbol.Mass = 1.7565670E28;
            Kerbol.Radius = 261600000d;

            Kerbol.flightGlobalsIndex = 200;
            Kerbol.StarColor = "Yellow";
            Kerbol.ScienceMultiplier = 1f;

            Kerbol.BodyDescription = "The Sun is the most well known object in the daytime sky. Scientists have noted a particular burning sensation and potential loss of vision if it is stared at for long periods of time. This is especially important to keep in mind considering the effect shiny objects have on the average Kerbal.";

            StarDict["Kerbol"] = Kerbol;

            Debug.Log("Starinfo loaded");

        }
        public void OnLevelWasLoaded(int level)
        {

            Debug.Log("Level: " + level);

            if (level == 10)
            {
                //Set max zoom
                PlanetariumCamera.fetch.maxDistance = 5000000000;

                Debug.Log("Creating basis for new stars...");

                //Create base for new stars
                foreach (StarInfo Star in StarDict.Values)
                {
                    //Grab Sun Internal PSystemBody 
                    var InternalSunPSB = PSystemManager.Instance.systemPrefab.rootBody;
                    var InternalSunCB = InternalSunPSB.celestialBody;

                    //Instantiate Sun Internal PSystemBody
                    var InternalStarPSB = (PSystemBody)Instantiate(InternalSunPSB);
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



            Debug.Log("Altering sun...");

            //Set Original Sun Parameters
            var OriginalSun = CBDict["Sun"];
            double SolarMasses;

            try
            {
                SolarMasses = double.Parse(ConfigNode.Load("GameData/StarSystems/Configdata/StarNames.cfg").GetNode("BlackHole").GetValue("SolarMasses"));
            }
            catch
            {
                SolarMasses = 7700;
            }

            OriginalSun.Mass = SolarMasses * OriginalSun.Mass;
            OriginalSun.Radius = (2 * (6.74E-11) * OriginalSun.Mass) / (Math.Pow(299792458, 2.0));
            OriginalSun.GeeASL = OriginalSun.Mass * (6.674E-11 / 9.81) / Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gMagnitudeAtCenter = OriginalSun.GeeASL * 9.81 * Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gravParameter = OriginalSun.gMagnitudeAtCenter;

            OriginalSun.scienceValues.InSpaceLowDataValue = OriginalSun.scienceValues.InSpaceLowDataValue * 10f;
            OriginalSun.scienceValues.RecoveryValue = OriginalSun.scienceValues.RecoveryValue * 5f;

            OriginalSun.bodyName = "Billy-Hadrick";

            OriginalSun.bodyDescription = "This recently discovered black hole, named after its discoverer Billy-Hadrick Kerman, is the central point where multiple star systems revolve around.";

            OriginalSun.CBUpdate();

            //Make Sun Black
            var ScaledSun = TFDict["Sun"];
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
                SunCorona.renderer.material.mainTexture = GameDatabase.Instance.GetTexture("StarSystems/Resources/BlackHoleCorona", false);
                var SunCoronaMeshFilter = (MeshFilter)SunCorona.GetComponent(typeof(MeshFilter));
                MeshScaler.ScaleMesh(SunCoronaMeshFilter.mesh, SunRatio);
            }

            Debug.Log("Sun altered");

            //Build out stars
            foreach (var Star in StarDict.Values)
            {

                Debug.Log("Creating " + Star.name + "...");

                var LocalSunCB = CBDict["Sun"];
                var LocalStarCB = CBDict[Star.name];
                var StarRatio = (float)Star.Radius / 261600000f;
                var ScaledStar = TFDict[Star.name];
                var ScaledStarMeshFilter = (MeshFilter)ScaledStar.GetComponent(typeof(MeshFilter));

                //Set Star Variables
                LocalStarCB.Mass = Star.Mass;
                LocalStarCB.Radius = Star.Radius;
                LocalStarCB.GeeASL = LocalStarCB.Mass * (6.674E-11 / 9.81) / Math.Pow(LocalStarCB.Radius, 2.0);
                LocalStarCB.gMagnitudeAtCenter = LocalStarCB.GeeASL * 9.81 * Math.Pow(LocalStarCB.Radius, 2.0);
                LocalStarCB.gravParameter = LocalStarCB.gMagnitudeAtCenter;
                LocalStarCB.bodyDescription = Star.BodyDescription;

                //Set Science parameters
                LocalStarCB.scienceValues.InSpaceLowDataValue = LocalStarCB.scienceValues.InSpaceLowDataValue * Star.ScienceMultiplier;
                LocalStarCB.scienceValues.RecoveryValue = LocalStarCB.scienceValues.RecoveryValue * Star.ScienceMultiplier;

                //Create new Orbitdriver
                OrbitDriver NewOrbitDriver = LocalStarCB.gameObject.AddComponent<OrbitDriver>();

                //Add new OrbitDriver to Kerbol
                LocalStarCB.orbitDriver = NewOrbitDriver;

                //Set OrbitDriver parameters
                LocalStarCB.orbitDriver.name = LocalStarCB.name;
                LocalStarCB.orbitDriver.celestialBody = LocalStarCB;
                LocalStarCB.orbitDriver.referenceBody = LocalSunCB;
                LocalStarCB.orbitDriver.updateMode = OrbitDriver.UpdateMode.UPDATE;
                //LocalStarCB.orbitDriver.QueuedUpdate = true;

                //Create new orbit
                LocalStarCB.orbitDriver.orbit = new Orbit(Star.inclination, Star.eccentricity, Star.semiMajorAxis, Star.LAN, Star.argumentOfPeriapsis, Star.meanAnomalyAtEpoch, Star.epoch, LocalSunCB);

                //Calculate SOI
                LocalStarCB.sphereOfInfluence = (LocalStarCB.orbit.semiMajorAxis * Math.Pow(LocalStarCB.Mass / LocalStarCB.orbit.referenceBody.Mass, (2.0 / 5)));
                LocalStarCB.hillSphere = LocalStarCB.orbit.semiMajorAxis * (1.0 - LocalStarCB.orbit.eccentricity) * Math.Pow((LocalStarCB.Mass / (3.0 * LocalStarCB.orbit.referenceBody.Mass)), 1.0 / 3.0);

                //Update CelestialBody
                LocalStarCB.CBUpdate();

                //Update OrbitDriver
                NewOrbitDriver.UpdateOrbit();

                //Update Star Scale
                MeshScaler.ScaleMesh(ScaledStarMeshFilter.mesh, StarRatio);

                //Update Corona Ratio
                foreach (var StarCorona in ScaledStar.GetComponentsInChildren<SunCoronas>())
                {
                    var StarCoronaMeshFilter = (MeshFilter)StarCorona.GetComponent(typeof(MeshFilter));
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
                        StarCorona.renderer.material.mainTexture = GameDatabase.Instance.GetTexture("StarSystems/Resources/BlueStarCorona", false);
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
                        StarCorona.renderer.material.mainTexture = GameDatabase.Instance.GetTexture("StarSystems/Resources/RedStarCorona", false);
                    }
                }

                Debug.Log(Star.name + " created");

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

            //Create starlight controller
            var StarLightSwitcherObj = new GameObject("StarLightSwitcher", typeof(StarLightSwitcher));
            GameObject.DontDestroyOnLoad(StarLightSwitcherObj);
            foreach (string StarName in StarDict.Keys)
            {
                //Add stars to dictionary
                StarLightSwitcherObj.GetComponent<StarLightSwitcher>().AddStar(CBDict[StarName], StarDict[StarName].StarColor);
            }

            Debug.Log("Starlight controller created");

            //Create Navball fixer
            var NavBallFixerObj = new GameObject("NavBallFixer", typeof(NavBallFixer));
            GameObject.DontDestroyOnLoad(NavBallFixerObj);

            Debug.Log("Navball fixer created");
        }
    }

    public class StarLightSwitcher : MonoBehaviour
    {
        private static Dictionary<CelestialBody, String> StarDistance = new Dictionary<CelestialBody, String>();
        private double DistanceCB;
        private double DistanceStar;

        //Set starlight color varaiables
        private Color StarRed = new Color(0.6f, 0.25f, 0.07f, 1.0f);
        private Color StarBlue = new Color(0.0f, 0.15f, 0.6f, 1.0f);
        private Color StarYellow = new Color(1.0f, 1.0f, 1.0f, 1.0f);


        public void AddStar(CelestialBody StarCB, String StarColor)
        {
            StarDistance[StarCB] = StarColor;
        }

        void Update()
        {

            //If in map mode
            if (PlanetariumCamera.fetch.enabled == true)
            {
                //Get camera position
                Vector3 CameraPosition = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.fetch.GetCameraTransform().position);

                foreach (CelestialBody CB in StarDistance.Keys)
                {
                    //Compare distance between active star and star
                    DistanceCB = FlightGlobals.getAltitudeAtPos(CameraPosition, CB);
                    DistanceStar = FlightGlobals.getAltitudeAtPos(CameraPosition, Sun.Instance.sun);
                    if (DistanceCB < DistanceStar && Sun.Instance.sun != CB)
                    {
                        //Set star as active star
                        Sun.Instance.sun = CB;
                        Planetarium.fetch.Sun = CB;
                        Debug.Log("Active sun set to: " + CB.name);

                        //Set sunflare color
                        if (StarDistance[CB] == "Red")
                        {
                            Sun.Instance.sunFlare.color = StarRed;
                        }
                        if (StarDistance[CB] == "Blue")
                        {
                            Sun.Instance.sunFlare.color = StarBlue;
                        }
                        if (StarDistance[CB] == "Yellow")
                        {
                            Sun.Instance.sunFlare.color = StarYellow;
                        }

                        //Reset solar panels (Credit to Kcreator)
                        foreach (ModuleDeployableSolarPanel panel in FindObjectsOfType(typeof(ModuleDeployableSolarPanel)))
                        {
                            panel.OnStart(PartModule.StartState.Orbital);
                        }
                    }
                }
            }
            else
            {
                //If controlling ship
                if (FlightGlobals.ActiveVessel != null)
                {
                    //Grab ship position
                    Vector3 ShipPosition = FlightGlobals.ActiveVessel.GetTransform().position;

                    foreach (CelestialBody CB in StarDistance.Keys)
                    {
                        //Compare distance between active star and star
                        DistanceCB = FlightGlobals.getAltitudeAtPos(ShipPosition, CB);
                        DistanceStar = FlightGlobals.getAltitudeAtPos(ShipPosition, Sun.Instance.sun);
                        if (DistanceCB < DistanceStar && Sun.Instance.sun != CB)
                        {
                            //Set star to active star
                            Sun.Instance.sun = CB;
                            Planetarium.fetch.Sun = CB;
                            Debug.Log("Active sun set to: " + CB.name);

                            //Set flare color
                            if (StarDistance[CB] == "Red")
                            {
                                Sun.Instance.sunFlare.color = StarRed;
                            }
                            if (StarDistance[CB] == "Blue")
                            {
                                Sun.Instance.sunFlare.color = StarBlue;
                            }
                            if (StarDistance[CB] == "Yellow")
                            {
                                Sun.Instance.sunFlare.color = StarYellow;
                            }

                            //Reset solar panels (Credit to Kcreator)
                            foreach (ModuleDeployableSolarPanel panel in FindObjectsOfType(typeof(ModuleDeployableSolarPanel)))
                            {
                                panel.OnStart(PartModule.StartState.Orbital);
                            }
                        }

                    }
                }
            }
        }
    }
    

}
