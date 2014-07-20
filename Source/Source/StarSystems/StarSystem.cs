﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarSystems.Creator;
using StarSystems.Data;
using StarSystems.Utils;
using UnityEngine;

namespace StarSystems
{
    public class StarSystem : MonoBehaviour
    {
        public static Dictionary<string, CelestialBody> CBDict = new Dictionary<string, CelestialBody>();
        public static Dictionary<string, Transform> TFDict = new Dictionary<string, Transform>();
        private static Dictionary<string, Star> StarDict = new Dictionary<string, Star>();

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
                Kerbol.Name = "Kerbol";
                Kerbol.Inclination = 0;
                Kerbol.Eccentricity = 0;
                Kerbol.SemiMajorAxis = kspSystemDefinition.SemiMajorAxis;
                Kerbol.LAN = 0;
                Kerbol.ArgumentOfPeriapsis = 0;
                Kerbol.MeanAnomalyAtEpoch = 0;
                Kerbol.Epoch = 0;
                Kerbol.Mass = 1.7565670E28;
                Kerbol.Radius = 261600000d;
                Kerbol.FlightGlobalsIndex = 0;
                Kerbol.StarColor = PlanetColor.Yellow;
                Kerbol.ScienceMultiplier = 1f;
                Kerbol.OrignalStar = true;
                Kerbol.BodyDescription =
                    "The Sun is the most well known object in the daytime sky. Scientists have noted a particular burning sensation and potential loss of vision if it is stared at for long periods of time. This is especially important to keep in mind considering the effect shiny objects have on the average Kerbal.";
                kspSystemDefinition.Stars.Add(Kerbol);
                Debug.Log("Ksp Solar System Defintions loaded");
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

                switch (level)
                {
                    case 10://prerender before main menu
                        PlanetariumCamera.fetch.maxDistance = 5000000000;
                        Debug.Log("Creating basis for new stars...");

                        //Create base for new stars
                        foreach (StarSystemDefintion star in kspSystemDefinition.Stars)
                        {
                            //Grab Sun Internal PSystemBody 
                            var InternalSunPSB = PSystemManager.Instance.systemPrefab.rootBody;
                            var InternalSunCB = InternalSunPSB.celestialBody;

                            //Instantiate Sun Internal PSystemBody
                            var InternalStarPSB = (PSystemBody)Instantiate(InternalSunPSB);
                            StarDict.Add(star.Name, new Star(star, InternalStarPSB, InternalSunPSB));
                        }

                        PSystemManager.Instance.systemPrefab.rootBody.flightGlobalsIndex = -1;

                        Debug.Log("Basis for new stars created");

                        //PsystemReady trigger
                        PSystemManager.Instance.OnPSystemReady.Add(OnPSystemReady);
                        break;
                    case 8://tracking station
                        break;
                    case 5://space center
                        //Set sun to Kerbol when loading space center
                        Sun.Instance.sun = CBDict["Kerbol"];
                        Planetarium.fetch.Sun = CBDict["Kerbol"];
                        break;
                    case 2://main menu
                        Initialized = false;
                        break;
                        
                }
            }
        }

        public void OnPSystemReady()
        {

            Debug.Log("Event: OnPSystemReady Init...");
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


            CenterRoot.Instance.OnPSystemReady(kspSystemDefinition.Root, CBDict["Sun"], TFDict["Sun"]);


            //Build out stars
            foreach (var starDefinition in kspSystemDefinition.Stars)
            {
                Debug.Log("Creating " + starDefinition.Name + "...");
                var LocalSunCB = CBDict["Sun"];
                var LocalStarCB = CBDict[starDefinition.Name];
                var StarTrasform = TFDict[starDefinition.Name];
                var starCreator = StarDict[starDefinition.Name];
                starCreator.OnPSystemReady(LocalSunCB, LocalStarCB, StarTrasform);
                Debug.Log(starDefinition.Name + " created");
            }

            //Create starlight controller
            var StarLightSwitcherObj = new GameObject("StarLightSwitcher", typeof (StarLightSwitcher));
            GameObject.DontDestroyOnLoad(StarLightSwitcherObj);
            foreach (string StarName in StarDict.Keys)
            {

                var starDefinition = kspSystemDefinition.Stars.Find(item => item.Name == StarName);
                //Add stars to dictionary
                StarLightSwitcherObj.GetComponent<StarLightSwitcher>()
                    .AddStar(CBDict[StarName], starDefinition.StarColor);
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