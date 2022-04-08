using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tank.Human
{
    public class TankShootingHuman : TankShooting
    {

        private string m_FireButton;

        protected override void Start()
        {
            base.Start();
            
            m_FireButton = "Fire" + m_PlayerNumber;
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetAxis(m_FireButton) > 0) //have we pressed fire for the first time?
            {
                StartCoroutine((IEnumerator)base.FireCoroutine());
            }
            else if (Input.GetAxis(m_FireButton) == 0) //we re;eased the button, having not yet fired
            {
                m_Fired = false;
            }
            
            // Track the current state of the fire button and make decisions based on the current launch force.
            // m_AimSlider.value = m_MinLaunchForce;

            // if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) //at max charge, not yet fired
            // {
            //     m_CurrentLaunchForce = m_MaxLaunchForce;
            //     Fire();
            // }
        }
    }
}