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
                    StarSystemDefintion starSystemDefintion = new StarSystemDefintion();

                    starSystemDefintion.Name = star.GetNode("CelestialBody").GetValue("name");
                    starSystemDefintion.FlightGlobalsIndex = int.Parse(star.GetNode("CelestialBody").GetValue("flightGlobalIndex"));
                    starSystemDefintion.SemiMajorAxis = double.Parse(star.GetNode("Orbit").GetValue("semiMajorAxis"));
                    try
                    {
                        starSystemDefintion.BodyDescription = star.GetNode("CelestialBody").GetValue("BodyDescription");
                    }
                    catch (Exception e)
                    {
                    }

                    try
                    {
                        starSystemDefintion.Radius = double.Parse(star.GetNode("CelestialBody").GetValue("Radius"));
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
                                    star.GetNode("CelestialBody").GetValue("StarColor"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.StarColor = PlanetColor.Yellow;
                    }
                    try
                    {
                        starSystemDefintion.Mass = double.Parse(star.GetNode("CelestialBody").GetValue("Mass"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Mass = 1.7565670E28;
                    }
                    try
                    {
                        starSystemDefintion.ScienceMultiplier =
                            float.Parse(star.GetNode("CelestialBody").GetValue("ScienceMultiplier"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.ScienceMultiplier = 10f;
                    }
                    try
                    {
                        starSystemDefintion.Inclination = double.Parse(star.GetNode("Orbit").GetValue("inclination"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Inclination = 0;
                    }
                    try
                    {
                        starSystemDefintion.Eccentricity = double.Parse(star.GetNode("Orbit").GetValue("eccentricity"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Eccentricity = 0;
                    }
                    try
                    {
                        starSystemDefintion.LAN = double.Parse(star.GetNode("Orbit").GetValue("LAN"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.LAN = 0;
                    }
                    try
                    {
                        starSystemDefintion.ArgumentOfPeriapsis =
                            double.Parse(star.GetNode("Orbit").GetValue("argumentOfPeriapsis"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.ArgumentOfPeriapsis = 0;
                    }
                    try
                    {
                        starSystemDefintion.MeanAnomalyAtEpoch = double.Parse(star.GetNode("Orbit").GetValue("meanAnomalyAtEpoch"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.MeanAnomalyAtEpoch = 0;
                    }
                    try
                    {
                        starSystemDefintion.Epoch = double.Parse(star.GetNode("Orbit").GetValue("epoch"));
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
            if (star.HasNode("CelestialBody") && star.HasNode("Orbit"))
            {
                if (star.GetNode("CelestialBody").HasValue("name") &&
                    star.GetNode("CelestialBody").HasValue("flightGlobalIndex") &&
                    star.GetNode("Orbit").HasValue("semiMajorAxis"))
                {
                    int flightGlobalIndex;
                    double semiMajorAxis;
                    bool isflightGlobalIndexValueValid = int.TryParse(star.GetNode("CelestialBody").GetValue("flightGlobalIndex"), out flightGlobalIndex);
                    bool issemiMajorAxisValueValid = double.TryParse(star.GetNode("Orbit").GetValue("semiMajorAxis"), out semiMajorAxis);
                    if (isflightGlobalIndexValueValid && issemiMajorAxisValueValid &&
                        star.GetNode("CelestialBody").GetValue("name") != "")
                    {
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
                return false;
            }
            system_config = ConfigNode.Load(string.Format("GameData/StarSystems/Config/{0}.cfg",configname));
            if (!system_config.HasData)
            {
                return false;
            }
            Debug.Log("Valid star configs.");
            if (system_config.HasNode("KSPSystem"))
            {
                if (system_config.HasNode("Kerbol") && system_config.HasNode("Root") && system_config.HasNode("StarSystems"))
                {
                    ConfigNode[] stars = system_config.GetNode("StarSystems").GetNodes("StarSystem");
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
