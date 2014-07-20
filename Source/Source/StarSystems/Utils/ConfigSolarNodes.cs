using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarSystems.Data;
using UnityEngine;

namespace StarSystems.Utils
{
    public class ConfigSolarNodes
    {
        private ConfigNode system_config;
        private bool system_config_valid;
        private static ConfigSolarNodes instance;

        private ConfigSolarNodes()
        {
        }

        public static ConfigSolarNodes Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigSolarNodes();
                }
                return instance;
            }
        }

        public KspSystemDefinition GetConfigData()
        {
            KspSystemDefinition kspSystemDefinition;
            if (system_config == null)
            {
                return null;
            }
            else
            {
                if (!system_config.HasData && !system_config_valid)
                {
                    return null;
                }
                else
                {
                    ConfigNode kspNode = system_config.GetNode("KSPSystem");
                    RootDefinition rootDefinition;
                    double sun_solar_mass;
                    SunType sun_solar_type;
                    try
                    {
                        sun_solar_mass = double.Parse(kspNode.GetNode("Root").GetValue("SolarMasses")); 
                    }
                    catch
                    {
                        sun_solar_mass = 7700;
                    }
                    try
                    {
                        sun_solar_type = ((SunType)int.Parse(kspNode.GetNode("Root").GetValue("Type"))); 
                    }
                    catch
                    {
                        sun_solar_type = SunType.Blackhole;
                    }
                    rootDefinition = new RootDefinition(sun_solar_mass, sun_solar_type);
                    try
                    {
                        kspSystemDefinition = new KspSystemDefinition(rootDefinition,
                            double.Parse(kspNode.GetNode("Kerbol").GetValue("semiMajorAxis")));
                    }
                    catch
                    {
                        kspSystemDefinition = new KspSystemDefinition(rootDefinition, 4500000000000);
                    }
                    kspSystemDefinition.Stars = getStars(kspNode.GetNode("StarSystems").GetNodes("StarSystem"));
                }
            }
            return kspSystemDefinition;

        }
        List<StarSystemDefintion> getStars(ConfigNode[] stars_config)
        {
            List<StarSystemDefintion> returnValue = new List<StarSystemDefintion>();
            //Grab star info
            foreach (var star in stars_config)
            {
                if (IsStarValid(star))
                {
                    var sun = star.GetNode("Sun");
                    StarSystemDefintion starSystemDefintion = new StarSystemDefintion();

                    starSystemDefintion.Name = sun.GetNode("CelestialBody").GetValue("name");
                    starSystemDefintion.FlightGlobalsIndex = int.Parse(sun.GetNode("CelestialBody").GetValue("flightGlobalIndex"));
                    starSystemDefintion.SemiMajorAxis = double.Parse(sun.GetNode("Orbit").GetValue("semiMajorAxis"));
                    try
                    {
                        starSystemDefintion.BodyDescription = sun.GetNode("CelestialBody").GetValue("BodyDescription");
                    }
                    catch (Exception e)
                    {
                    }

                    try
                    {
                        starSystemDefintion.Radius = double.Parse(sun.GetNode("CelestialBody").GetValue("Radius"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Radius = 261600000;
                    }
                    try
                    {
                        starSystemDefintion.StarColor =
                            (PlanetColor)
                                EnumUtilities.Parse(typeof (PlanetColor),
                                    sun.GetNode("CelestialBody").GetValue("StarColor"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.StarColor = PlanetColor.Yellow;
                    }
                    try
                    {
                        starSystemDefintion.Mass = double.Parse(sun.GetNode("CelestialBody").GetValue("Mass"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Mass = 1.7565670E28;
                    }
                    try
                    {
                        starSystemDefintion.ScienceMultiplier =
                            float.Parse(sun.GetNode("CelestialBody").GetValue("ScienceMultiplier"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.ScienceMultiplier = 10f;
                    }
                    try
                    {
                        starSystemDefintion.Inclination = double.Parse(sun.GetNode("Orbit").GetValue("inclination"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Inclination = 0;
                    }
                    try
                    {
                        starSystemDefintion.Eccentricity = double.Parse(sun.GetNode("Orbit").GetValue("eccentricity"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Eccentricity = 0;
                    }
                    try
                    {
                        starSystemDefintion.LAN = double.Parse(sun.GetNode("Orbit").GetValue("LAN"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.LAN = 0;
                    }
                    try
                    {
                        starSystemDefintion.ArgumentOfPeriapsis =
                            double.Parse(sun.GetNode("Orbit").GetValue("argumentOfPeriapsis"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.ArgumentOfPeriapsis = 0;
                    }
                    try
                    {
                        starSystemDefintion.MeanAnomalyAtEpoch = double.Parse(sun.GetNode("Orbit").GetValue("meanAnomalyAtEpoch"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.MeanAnomalyAtEpoch = 0;
                    }
                    try
                    {
                        starSystemDefintion.Epoch = double.Parse(sun.GetNode("Orbit").GetValue("epoch"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Epoch = 0;
                    }
                    returnValue.Add(starSystemDefintion);
                }
                else
                {
                    Debug.Log("Star Unable be create lack requirement fields: CelestialBody/name,CelestialBody/flightGlobalIndex,Orbit/semiMajorAxis");
                    continue;
                }
            }
            return returnValue;
        }

        bool IsStarValid(ConfigNode star)
        {
            bool returnValue = false;
            ConfigNode sun = star.GetNode("Sun");
            Debug.Log(star);
            Debug.Log(sun);
            Debug.Log("Valid Solar System Config.");
            if (sun.HasNode("CelestialBody") && sun.HasNode("Orbit"))
            {

                Debug.Log("Keys Found For Sun.");
                if (sun.GetNode("CelestialBody").HasValue("name") &&
                    sun.GetNode("CelestialBody").HasValue("flightGlobalIndex") &&
                    sun.GetNode("Orbit").HasValue("semiMajorAxis"))
                {

                    Debug.Log("Values in the keys Found For Sun.");
                    int flightGlobalIndex;
                    double semiMajorAxis;
                    bool isflightGlobalIndexValueValid = int.TryParse(sun.GetNode("CelestialBody").GetValue("flightGlobalIndex"), out flightGlobalIndex);
                    bool issemiMajorAxisValueValid = double.TryParse(sun.GetNode("Orbit").GetValue("semiMajorAxis"), out semiMajorAxis);
                    if (isflightGlobalIndexValueValid && issemiMajorAxisValueValid &&
                        sun.GetNode("CelestialBody").GetValue("name") != "")
                    {
                        Debug.Log("All Good");
                        returnValue = true;
                    }
                }
            }
            return returnValue;
        }
        public bool IsValid(string configname)
        {
            system_config_valid = false;
            if (configname == "")
            {


                Debug.Log("No Config File listed");
                return false;
            }
            system_config = ConfigNode.Load(string.Format("GameData/StarSystems/Config/{0}.cfg",configname));
            Debug.Log(system_config);
            Debug.Log(string.Format("Config Loading. GameData/StarSystems/Config/{0}.cfg", configname));
            if (!system_config.HasData)
            {
                Debug.Log("Config Has No Data");
                return false;
            }
            Debug.Log("Valid star configs.");
            if (system_config.HasNode("KSPSystem"))
            {
                Debug.Log("Found Master Node.");
                if (system_config.GetNode("KSPSystem").HasNode("Kerbol") && system_config.GetNode("KSPSystem").HasNode("Root") && system_config.GetNode("KSPSystem").HasNode("StarSystems"))
                {

                    Debug.Log("Checking for number systems is it 0 mod will close");
                    ConfigNode[] stars = system_config.GetNode("KSPSystem").GetNode("StarSystems").GetNodes("StarSystem");
                    Debug.Log(string.Format("Number of Solar System Found {0}", stars.Count()));
                    if (stars.Count() != 0)
                    {
                        system_config_valid = true;
                    }
                }
            }
            return system_config_valid;
        }
    }
}
