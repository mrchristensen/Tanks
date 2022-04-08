using System;
using Tank;
using Tank.Human;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint; 
    
    public Boolean m_AI = false;
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;

    private TankMovement m_Movement;       
    private TankShooting m_TankShooting;
    private TankMovementAI m_TankMovementAI;  // Todo: make this abstract and inheritted
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        String name = m_AI ? "COM" : "PLAYER";

        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_TankShooting = m_Instance.GetComponent<TankShooting>();
        m_TankMovementAI = m_Instance.GetComponent<TankMovementAI>();  // Todo: make this not AI spesific
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_TankShooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">" + name +" " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        foreach (var meshRenderer in renderers)
        {
            meshRenderer.material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_TankShooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_TankShooting.enabled = true;

        if (m_AI)
        {
            m_TankMovementAI.StartCoroutine(m_TankMovementAI.SearchCoroutine());
        }

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;
        m_TankShooting.DrawRay(); //Draw prediction line at the beginning or rounds (after resetting the position)

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
