using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
    private int ammo;
    private int maxAmmoCount = 4;
    private float fireCooldownTime = 3f;
    
    // private IEnumerator fireCoroutine;


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

        ammo = maxAmmoCount;
    }

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        // if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) //at max charge, not yet fired
        // {
        //     m_CurrentLaunchForce = m_MaxLaunchForce;
        //     Fire();
        // }
        if (Input.GetAxis(m_FireButton) > 0 && ammo > 0 && m_Fired == false) //have we pressed fire for the first time?
        {
            StartCoroutine(FireCoroutine());
        }
        else if (Input.GetAxis(m_FireButton) == 0) //we re;eased the button, having not yet fired
        {
            m_Fired = false;
        }
    }


    private IEnumerator FireCoroutine()
    {
        m_Fired = true;
        ammo--;
        
        // Instantiate and launch the shell.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
        //todo: slow down the tank
        yield return new WaitForSeconds(fireCooldownTime);
        
        //todo: check to see if another bullet has been fired by this time
        ammo = maxAmmoCount;
    }
}