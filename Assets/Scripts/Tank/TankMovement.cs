using UnityEngine;

namespace Tank
{
    public abstract class TankMovement : MonoBehaviour
    {
        protected float m_SpeedNormal;
        
        public int m_PlayerNumber = 1;
        public Transform m_FireTransform;
        public Transform m_BodyTransform;
        private float m_SpeedSlow = 2f;
        // public float m_TurnSpeed = 180f;
        public AudioSource m_MovementAudio;
        public AudioClip m_EngineIdling;
        public AudioClip m_EngineDriving;
        public float m_PitchRange = 0.2f;
        protected float m_SpeedCurrent;
        
        protected Rigidbody m_Rigidbody;
        
        private float m_OriginalPitch;
        private Vector3 point;
        

        protected abstract void Init();

        protected abstract bool Idle();
        
        protected virtual void Awake()
        {
            Debug.Log("Parent awake");
            Init();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void OnEnable ()
        {
            Debug.Log("Tank Movement Parent onEnable()");
            m_SpeedCurrent = m_SpeedNormal;
        }

        private void OnDisable ()
        {
            m_Rigidbody.isKinematic = true;
        }

        private void Start()
        {
            m_OriginalPitch = m_MovementAudio.pitch;
        }

        private void Update()
        {
            EngineAudio();
        }
        
        protected abstract void Aim();

        private void EngineAudio()
        {
            // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
            if (Idle()) //idling
            {
                if(m_MovementAudio.clip == m_EngineDriving)
                {
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
            else //driving
            {
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }

        private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2( a.x - b.x, a.y - b.y) * Mathf.Rad2Deg;
        }

        public void SetSpeedSlow()
        {
            Debug.Log(m_SpeedSlow);
            m_SpeedCurrent = m_SpeedSlow;
        }

        public void SetSpeedNormal()
        {
            Debug.Log("Parent Movement Set Normal Speed to: " + m_SpeedNormal);
            m_SpeedCurrent = m_SpeedNormal;
            // m_SpeedCurrent = Mathf.Lerp(m_SpeedSlow, m_SpeedNormal, Time.deltaTime * 100f);
        }
    }
}