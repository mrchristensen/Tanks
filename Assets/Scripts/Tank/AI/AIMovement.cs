using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : TankMovement
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject me;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int state;
    
    private const int SEARCH = 0;
    private const int ATTACK = 1;
    private const int RELOAD = 2;

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

    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        me = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         agent.SetDestination(hit.point);
        //     }
        //     // agent.SetDestination(enemies[0].transform.TransformVector(0, 0, 0));
        // }
    }

    private void FixedUpdate()
    {
        // agent.velocity = agent.desiredVelocity * m_SpeedNormal * Time.deltaTime;
    }


    public void addEnemyTank(GameObject tank)
    {
        enemies.Add(tank);
    }
}
