using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public Slider m_AmmoSlider;
    public LineRenderer m_AimingLine;
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_ShellLaunchForce = 15f;

    private TankMovement m_TankMovement;
    private string m_FireButton;         
    // private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
    private int ammo;
    private int maxAmmoCount = 4;
    private float speedCooldownTime = .2f;
    private float fireCooldownTime = 3f;
    
    // private IEnumerator fireCoroutine;


    private void OnEnable()
    {
        // m_AimSlider.value = m_MinLaunchForce;
        ammo = maxAmmoCount;
        SetAmmoUI(); //Update the UI to match the current ammo
    }


    private void Start()
    {
        m_TankMovement = GetComponent<TankMovement>();
        m_FireButton = "Fire" + m_PlayerNumber;
        m_AmmoSlider.maxValue = maxAmmoCount;
    }

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        // m_AimSlider.value = m_MinLaunchForce;

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

    private void DrawRay()
    {
        Ray ray = new Ray( m_FireTransform.position, m_FireTransform.forward );
        RaycastHit raycastHit;
        // Vector3 endPosition = m_FireTransform.position + ( 120f * m_FireTransform.forward );
 
        if( Physics.Raycast( ray, out raycastHit, 256f ) ) {
            Vector3 endPosition = raycastHit.point;
            m_AimingLine.SetPosition( 0, m_FireTransform.position );
            m_AimingLine.SetPosition( 1, endPosition );
            
        }
 
    }

    private void FixedUpdate()
    {
        DrawRay();
    }


    private IEnumerator FireCoroutine()
    {
        m_Fired = true;
        ammo--;
        int ammoCountOnFire = ammo;
        SetAmmoUI();
        
        // Instantiate and launch the shell.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_ShellLaunchForce * m_FireTransform.forward;
        shellInstance.gameObject.GetComponent<ShellExplosion>().SetTank(this.gameObject);
        
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // m_CurrentLaunchForce = m_MinLaunchForce;
        
        m_TankMovement.SetSpeedSlow();
        yield return new WaitForSeconds(speedCooldownTime);
        if (ammoCountOnFire == ammo) //if the ammo is the same as when we fired then no new shell has been fired
        {
            m_TankMovement.SetSpeedNormal(); //thus set the tank to normal speed
        }
        
        yield return new WaitForSeconds(fireCooldownTime - speedCooldownTime);
        
        if (ammoCountOnFire == ammo) //if the ammo is the same as when we fired then no new shell has been fired
        {
            ammo = maxAmmoCount; //thus refill the ammo
            SetAmmoUI();
        }
    }

    private void SetAmmoUI()
    {
        m_AmmoSlider.value = ammo;
    }
}