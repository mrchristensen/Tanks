using System;
using Unity.UNetWeaver;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Transform m_FireTransform;
    public Transform m_BodyTransform;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;
    private string m_LookAxisHorizontalName;
    private string m_LookAxisVerticalName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private Vector3 m_LookVector;
    private Vector3 m_MoveVector;
    private float m_OriginalPitch;
    private Vector3 point;
    private Quaternion m_RotateAngle;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_LookAxisHorizontalName = "LookHorizontal" + m_PlayerNumber;
        m_LookAxisVerticalName = "LookVertical" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        
        m_MoveVector = new Vector3(Input.GetAxisRaw(m_TurnAxisName), 0, Input.GetAxisRaw(m_MovementAxisName));
        Vector3 tempVector = new Vector3(Input.GetAxisRaw(m_LookAxisHorizontalName), 0, Input.GetAxisRaw(m_LookAxisVerticalName));
        if (tempVector.magnitude > .6)
        {
            m_LookVector = tempVector;
        }

        if (m_MoveVector.magnitude > .00000001)
        {
            m_RotateAngle = Quaternion.LookRotation(m_MoveVector);
        }


        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) //idling
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


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
        Aim();
    }
    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        // Vector3 movement = m_MoveVector * m_MovementInputValue * m_Speed * Time.deltaTime;
        Vector3 movement = m_MoveVector * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        Debug.DrawRay(m_FireTransform.position, m_MoveVector, Color.green);
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        // float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        //
        // Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        //
        // m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        //
        //
        
        
     
        // Smoothly rotate towards the target point.
        m_BodyTransform.rotation = Quaternion.Slerp(m_BodyTransform.rotation, m_RotateAngle, 8f * Time.deltaTime);

        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }
    
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2( a.x - b.x, a.y - b.y) * Mathf.Rad2Deg;
    }
    
    private void Aim()
    {

        // Vector3 MouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z));
        var rotation = Quaternion.LookRotation(m_LookVector);
        m_FireTransform.rotation = Quaternion.Slerp(m_FireTransform.rotation, rotation, m_Speed * Time.deltaTime);
        // m_FireTransform.rotation = Quaternion.Euler(new Vector3(0, m_FireTransform.rotation.eulerAngles.y, 0));
        
        
        //Works perfectly for the middle of the screen
        // var middleOfScreen = new Vector2(Screen.width/2, Screen.height/2);
        // var middleOfScreen = new Vector2(Camera.main.WorldToScreenPoint(m_FireTransform.position).x, Camera.main.WorldToScreenPoint(m_FireTransform.position).y);
        // var mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        // var target = mousePos - middleOfScreen;
        // var flipped = new Vector3(target.x, 0f, target.y);
        // m_FireTransform.LookAt(flipped);
        
        
        
        
        
        
        
        
        
        // Vector3 mouse_pos;
        // Vector3 object_pos;
        // float angle;
        //
        // mouse_pos = Input.mousePosition;
        // mouse_pos.z = Vector3.Distance(Camera.main.transform.position, transform.position); //The distance between the camera and object
        // mouse_pos.z = m_FireTransform.position.z; //The distance between the camera and object
        // object_pos = Camera.main.WorldToScreenPoint(m_FireTransform.position);
        // mouse_pos.x = mouse_pos.x - object_pos.x;
        // mouse_pos.z = mouse_pos.z - object_pos.z;
        // mouse_pos.y = m_FireTransform.position.y;
        // angle = Mathf.Atan2(mouse_pos.x, mouse_pos.z) * Mathf.Rad2Deg;
        // m_FireTransform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        
        
        //Get the Screen positions of the object
        // Vector3 positionOnScreen = Camera.main.WorldToViewportPoint (m_FireTransform.position);
         
        //Get the Screen position of the mouse
        // Vector3 mouseOnScreen = Camera.main.ScreenToViewportPoint(Input.mousePosition);
         
        //Get the angle between the points 
        // float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);


        // var mouse = Input.mousePosition;

        // Vector3 aimPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, mouse.z));
        // aimPosition.y = m_FireTransform.position.y;
        
        // m_FireTransform.LookAt(aimPosition);
        
        
        
        //Ta Daaa
        // m_FireTransform.rotation =  Quaternion.Euler (aimPosition);
        
        
        
        // var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        
        
        // Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        //
        // Vector3 mouse = Input.mousePosition;
        // Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        //
        //
        //
        // Vector3 aimPosition = new Vector3(mouse.x - screenPoint.x , m_FireTransform.localPosition.y, mouse.z - screenPoint.z);
        
        // mousePosition.y = m_FireTransform.localPosition.y;
        // Quaternion rot = Quaternion.LookRotation (mousePosition - m_FireTransform.position, m_FireTransform.TransformDirection(Vector3.forward));
        // m_FireTransform.rotation = new Quaternion(0, 0, rot.z, rot.w);
        
        Debug.DrawRay(m_FireTransform.position, m_LookVector, Color.red);
        // Debug.DrawRay(m_FireTransform.position, mouse_pos, Color.red);
        // Debug.DrawRay(m_FireTransform.position, aimPosition, Color.green);
        // Debug.DrawRay(m_FireTransform.position, mouse, Color.red);
        // Debug.DrawRay(m_FireTransform.position, screenPoint, Color.blue);

        // point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // point.Set(point.x, point.y, point.z);
        //
        // m_FireTransform.LookAt(point);
    }
}