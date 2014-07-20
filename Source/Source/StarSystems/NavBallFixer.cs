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
    ///Navballfixer fixes the navball when in interstellar space
    public class NavBallFixer : MonoBehaviour
    {
        private Boolean NavBallCreated = false;
        private GameObject NewNavBall;
        private GameObject NewNavBallGimball;
        private NavBall NavBall;
        private bool NewNavBallEnabled = false;
        void Update()
        {
            createNavBall();
            activeNavBall();
            deactiveNavBall();
            updateNavball();
        }
        /// <summary>
        /// Create a new navball
        /// </summary>
        void createNavBall()
        {
            if (NavBallCreated == false && FlightGlobals.ActiveVessel != null)
            {
                NavBall = GameObject.Find("NavBall").GetComponent<NavBall>();
                var NavBallMesh = NavBall.navBall.GetComponent<MeshFilter>().mesh;

                NewNavBall = new GameObject("NewNavBall");
                GameObject.DontDestroyOnLoad(NewNavBall);

                NewNavBallGimball = new GameObject("NewNavBallGimball");
                GameObject.DontDestroyOnLoad(NewNavBallGimball);

                NewNavBallGimball.transform.position = NavBall.transform.position;

                var NewNavBallMF = NewNavBall.AddComponent<MeshFilter>();
                var NewNavBallMR = NewNavBall.AddComponent<MeshRenderer>();
                NewNavBall.renderer.material = NavBall.navBall.renderer.material;
                NewNavBall.renderer.enabled = false;
                NewNavBallMF.mesh = NavBallMesh;

                NewNavBall.transform.parent = NewNavBallGimball.transform;
                NewNavBall.layer = 12;
                NewNavBall.transform.position = NavBall.navBall.position;
                NewNavBall.transform.localScale = new Vector3(2.1f, 2.1f, 2.1f);

                NavBallCreated = true;
            }
        }
        /// <summary>
        /// Activate new navball when in interstellar space
        /// </summary>
        void activeNavBall()
        {
            if (NavBallCreated == true && NewNavBallEnabled == false && FlightGlobals.ActiveVessel != null)
            {
                if (FlightGlobals.ActiveVessel.mainBody.name == StarSystem.CBDict["Sun"].name)
                {
                    if (NavBall == null)
                    {
                        NavBall = GameObject.Find("NavBall").GetComponent<NavBall>();
                    }
                    NewNavBall.renderer.enabled = true;
                    NavBall.navBall.renderer.enabled = false;
                    NewNavBallEnabled = true;
                }
            }

        }
        /// <summary>
        /// Deactivate navball when exiting flight
        /// </summary>
        void deactiveNavBall()
        {
            //Deactivate navball when exiting interstellar space
            if (NavBallCreated == true && NewNavBallEnabled == true && FlightGlobals.ActiveVessel != null)
            {
                if (FlightGlobals.ActiveVessel.mainBody.name != StarSystem.CBDict["Sun"].name)
                {
                    if (NavBall == null)
                    {
                        NavBall = GameObject.Find("NavBall").GetComponent<NavBall>();
                    }
                    NewNavBall.renderer.enabled = false;
                    NavBall.navBall.renderer.enabled = true;
                    NewNavBallEnabled = false;
                }
            }
            if (NavBallCreated == true && NewNavBallEnabled == true && FlightGlobals.ActiveVessel == null)
            {
                NewNavBall.renderer.enabled = false;
                NewNavBallEnabled = false;
            }
        }
        /// <summary>
        /// update the nav ball
        /// </summary>
        void updateNavball()
        {

            if (NavBallCreated == true && NewNavBallEnabled == true && FlightGlobals.ActiveVessel != null)
            {

                Quaternion FaceMainBody = Quaternion.LookRotation(FlightGlobals.ActiveVessel.mainBody.transform.position);
                NewNavBallGimball.transform.rotation = NavBall.attitudeGymbal;
                NewNavBall.transform.position = NavBall.navBall.position;
                NewNavBall.transform.localRotation = FaceMainBody * Quaternion.Inverse(NavBall.offsetGymbal);
            } 
        }
    }
}
