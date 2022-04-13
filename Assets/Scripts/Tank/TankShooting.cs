using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tank
{
    public abstract class TankShooting : MonoBehaviour
    {
        public Rigidbody m_Shell;
        public Transform m_FireTransform;
        public Slider m_AimSlider;
        public Slider m_AmmoSlider;
        public LineRenderer m_AimingLine;
        public AudioSource m_ShootingAudio;
        public AudioClip m_ChargingClip;
        public AudioClip m_FireClip;
        public float m_ShellLaunchForce = 15f;
        public int m_PlayerNumber;  // Todo: make this come from somewhere that makes sense (I don't even know where this is currently coming from
        private TankMovement m_TankMovement;
        private float m_ChargeSpeed;
        protected bool m_Fired;
        protected int ammo;
        private int maxAmmoCount = 4;
        private float speedCooldownTime = .2f;
        private float fireCooldownTime = 3f;

        private void OnEnable()
        {
            // m_AimSlider.value = m_MinLaunchForce;
            ammo = maxAmmoCount;
            SetAmmoUI();  // Update the UI to match the current ammo
        }

        protected virtual void Start()
        {
            m_TankMovement = GetComponent<TankMovement>();
            m_AmmoSlider.maxValue = maxAmmoCount;
        }

        protected virtual void Update()
        {
            DrawRay();
        }

        public void DrawRay()
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
        }

        public IEnumerator FireCoroutine()
        {
            if (ammo <= 0 || m_Fired == true)  // If we have fired recently or don't have ammo, don't shoot
            {
                yield break;
            }
            
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

        public bool HasAmmo()
        {
            return ammo > 0;
        }

        private void SetAmmoUI()
        {
            m_AmmoSlider.value = ammo;
        }
    }
}