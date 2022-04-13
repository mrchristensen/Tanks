using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Tank;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TankMovementAI : TankMovement
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject me;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int state;
    [SerializeField] private GameObject[] hidingPlaces;
    
    private TankShooting TankShooting;
    
    private const int SEARCH = 0;
    private const int ATTACK = 1;
    private const int RELOAD = 2;

    // Todo: maybe these coroutines belong in an AI brain script?
    public IEnumerator SearchCoroutine()
    {
        while (TargetInsight() == false)
        {
            Debug.Log("Search Coroutine Step");
            agent.SetDestination(enemies[0].transform.position); //Update the position of the enemy
            yield return new WaitForSeconds(.5f);
        }
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(1f);
        agent.destination = gameObject.transform.position;
    }
    
    IEnumerator FleeCoroutine()
    {
        Debug.Log("Starting Flee Coroutine");
        agent.destination = gameObject.transform.position;  // Stop the agent (so if we are already hidden we don't move

        //Exit condition is ammo, in which case we start the search algorithm again
        while (TankShooting.HasAmmo() == false)
        {
            if (TargetInsight())
            {
                float closestHidingPlaceDistance = float.MaxValue;
                Vector3 closestHidingPlace =
                    hidingPlaces[Random.Range(0, hidingPlaces.Length)].transform
                        .position; // Initialize this to something just in case we don't find a hiding place

                //Check all the locations for hiding and see if they break the sight line with the player
                foreach (GameObject hidingPlace in hidingPlaces)
                {
                    if (sightLine(enemies[0].transform, hidingPlace.transform)) continue;

                    if (Vector3.Distance(me.transform.position, hidingPlace.transform.position) <
                        closestHidingPlaceDistance)
                    {
                        closestHidingPlaceDistance =
                            Vector3.Distance(me.transform.position, hidingPlace.transform.position);
                        closestHidingPlace =
                            hidingPlace.transform.position; // TODO: introduce local car here for position
                    }
                }

                //Pick the closest one as the nav target
                agent.SetDestination(closestHidingPlace);

            }

            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(SearchCoroutine());
    }

    private bool sightLine(Transform transform1, Transform transform2)
    {
        Vector3 rayDirection = transform1.position - transform2.position;
        if (Physics.Raycast (transform2.position, rayDirection, out RaycastHit hit)) {
            if (hit.transform == transform1)
            {
                return true;
            }
            
        }
        return false;
    }

    IEnumerator AttackCoroutine()
    {
        StartCoroutine(
            StopMoving()); // TODO: Change this to stop moving when the player is within range.  Better yet, never stop moving unless the play isn't visible, or out of range

        while (TargetInsight())
        {
            Debug.Log("Attack Coroutine Step");

            if (TankShooting.HasAmmo() == false)
            {
                StartCoroutine(FleeCoroutine());
                yield break;
            }

            StartCoroutine(TankShooting.FireCoroutine());
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(SearchCoroutine());

        // else if (no bullets)
        // {
        //     StartCoroutine(ReloadCoroutine());
        //     yield break;
        // }
    }



    // IEnumerator ReloadCoroutine()
    // {
    //     Debug.Log("Reload Coroutine Step");
    //
    //     if ()
    //     {
    //         StartCoroutine(SearchCoroutine());
    //         yield break;
    //     }
    // }

    private bool TargetInsight()
    {
        Vector3 fromPosition = me.transform.position;  //TODO: refactor this with sightLine()
        Vector3 toPosition = enemies[0].transform.position;
        Vector3 direction = toPosition - fromPosition;
        
        Ray ray = new Ray( fromPosition, direction);
        RaycastHit hit;
        // Vector3 endPosition = m_FireTransform.position + ( 120f * m_FireTransform.forward );
 
        Debug.DrawRay(fromPosition, direction, Color.green, 2f);
        if(Physics.Raycast(ray, out hit, 256f))
        {
            if (hit.collider.CompareTag("Tank")) // Todo: make this not a string (const file?)
            {
                Debug.Log("Target is in sight");
                return true;
            }
        }
        return false;
    }
    
    protected override void Init()
    {
        m_SpeedNormal = 9f;
        me = gameObject;
        TankShooting = gameObject.GetComponent<TankShooting>();
        hidingPlaces = GameObject.FindGameObjectsWithTag(Tags.HIDING_PLACE);  //Todo: Is this expensive?
        Debug.Log("Number of hiding places" + hidingPlaces.Length);
    }

    protected override bool Idle()
    {
        return agent.velocity.magnitude > .01;  // todo: check to make sure this actually works
    }

    protected override void Aim()
    {
        Vector3 enemyPosition = enemies[0].gameObject.transform.position;
        enemyPosition -= transform.position;
        enemyPosition.y = 0;
        
        var rotation = Quaternion.LookRotation(enemyPosition);
        m_FireTransform.rotation = Quaternion.Slerp(m_FireTransform.rotation, rotation, m_SpeedNormal * Time.deltaTime);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        Debug.Log("Tank Movement AI Child onEnable()");
        m_Rigidbody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        Aim();
    }

    public void AddEnemyTank(GameObject tank)
    {
        enemies.Add(tank);
    }
}
