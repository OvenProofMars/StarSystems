using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarSystems.Data;
using UnityEngine;

namespace StarSystems.Utils
{
    public class StarLightSwitcher : MonoBehaviour
    {
        private static Dictionary<CelestialBody, PlanetColor> StarDistance = new Dictionary<CelestialBody, PlanetColor>();
        private double DistanceCB;
        private double DistanceStar;

        //Set starlight color varaiables
        private Color StarRed = new Color(0.6f, 0.25f, 0.07f, 1.0f);
        private Color StarBlue = new Color(0.0f, 0.15f, 0.6f, 1.0f);
        private Color StarYellow = new Color(1.0f, 1.0f, 1.0f, 1.0f);


        public void AddStar(CelestialBody StarCB, PlanetColor StarColor)
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
                        if (StarDistance[CB] == PlanetColor.Red)
                        {
                            Sun.Instance.sunFlare.color = StarRed;
                        }
                        if (StarDistance[CB] == PlanetColor.Blue)
                        {
                            Sun.Instance.sunFlare.color = StarBlue;
                        }
                        if (StarDistance[CB] == PlanetColor.Yellow)
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
                            if (StarDistance[CB] == PlanetColor.Red)
                            {
                                Sun.Instance.sunFlare.color = StarRed;
                            }
                            if (StarDistance[CB] == PlanetColor.Blue)
                            {
                                Sun.Instance.sunFlare.color = StarBlue;
                            }
                            if (StarDistance[CB] == PlanetColor.Yellow)
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
