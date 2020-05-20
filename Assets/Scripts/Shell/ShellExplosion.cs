using System;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public int m_MaxBounces = 2;                  
    public float m_ExplosionRadius = 5f;
    private Vector3 oldVelocity;
    
    private int m_BouncesRemaining;
    
    public GameObject m_Tank;
    
    private readonly int PLAYER_LAYER = 9;
    private void Start()
    {
        //Destroy(gameObject, m_MaxLifeTime);
        oldVelocity = GetComponent<Rigidbody>().velocity;
    }

    private void OnEnable()
    {
        m_BouncesRemaining = m_MaxBounces;
    }

    public void SetTank(GameObject tank)
    {
        m_Tank = tank;
    }

    private void OnTriggerEnter(Collider other)
    {

        
    }


    //Bounce or explode
    void OnCollisionEnter (Collision collision) {
        Debug.Log("Hit something");
        
        if (collision.gameObject == m_Tank)
        {
            Debug.Log("Hit the tank that shot the shell (friendly fire)");
            return;
        }
        else if(m_BouncesRemaining > 0 && collision.gameObject.layer != PLAYER_LAYER) //hit something that's not a player and there are more bounces
        {
            m_BouncesRemaining--;

            ContactPoint contact = collision.contacts[0];

            // reflect our old velocity off the contact point's normal vector
            Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);

            // assign the reflected velocity back to the rigidbody
            GetComponent<Rigidbody>().velocity = reflectedVelocity;
            // rotate the object by the same ammount we changed its velocity
            Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
            transform.rotation = rotation * transform.rotation;
            
            oldVelocity = reflectedVelocity; //Update the velocity for the next bounce calculation
        }
        else //if no more bounces or hit a player
        {
            Explode(collision.gameObject);
        }
    }

    private void Explode(GameObject shotTank)
    {
        if (shotTank != null)
        {
            TankHealth targetHealth = shotTank.GetComponent<TankHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(50f); //todo make this not a magic number
            }
        }
        
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        
        
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            foreach (var VARIABLE in colliders)
            {
                Debug.Log(VARIABLE.name);
            }
        
            if (!targetRigidbody)
            {
                continue;
            }
                    
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
        
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
        
            if (!targetHealth)
            {
                continue;
            }
        
            // float damage = CalculateDamage(targetRigidbody.position);
                    
            // targetHealth.TakeDamage(damage);
        }
        
        m_ExplosionParticles.transform.parent = null; //Deattach the child object (so we can remove the shell object while still having the particles and sounds)
                
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
                
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; //0 to 1

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage); //damage can't be less than zero

        return damage;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, oldVelocity, Color.cyan);
        
        //Debug.DrawRay(transform.position, GetComponent<Rigidbody>().velocity);
    }
}