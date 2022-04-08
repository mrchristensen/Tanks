using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Tank;
using UnityEngine;
using UnityEngine.AI;

public class TankMovementAI : TankMovement
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject me;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int state;
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
            yield return new WaitForSeconds(2f);
        }
        
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        while (TargetInsight())
        {
            Debug.Log("Attack Coroutine Step");
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
        Vector3 fromPosition = me.transform.position;
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
