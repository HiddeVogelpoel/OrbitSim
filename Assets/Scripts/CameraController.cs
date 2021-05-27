using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    float mainSpeed = 100.0f; //regular speed
    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    public float followSharpness = 0.1f;
    public Text followText;

    bool follow;
    public Transform target;
    Vector3 _followOffset;

    void Awake()
    {
        followText.enabled = false;
    }

    void Update()
    {
        //Keyboard commands
        // float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
       
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space))
            { //If player wants to move on X and Z axis only
                transform.Translate(p);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(p);
            }
        }

        transform.LookAt(target);
    }

    void LateUpdate()
    {
        if (follow)
        {

            // Apply that offset to get a target position.
            Vector3 targetPosition = target.position + _followOffset;

            // Keep our y position unchanged.
            targetPosition.y = transform.position.y;

            // Smooth follow.    
            transform.position += (targetPosition - transform.position) * followSharpness;
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (follow)
        {
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 0.01f, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, -0.01f, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-0.01f, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(0.01f, 0, 0);
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                if (Vector3.Distance(target.position, transform.position) > 100)
                {
                    p_Velocity += new Vector3(0, 0, 0.5f);
                }
                else
                {
                    p_Velocity += new Vector3(0, 0, 0.05f);
                }
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                if (Vector3.Distance(target.position, transform.position) > 100)
                {
                    p_Velocity += new Vector3(0, 0, -0.5f);
                }
                else
                {
                    p_Velocity += new Vector3(0, 0, -0.05f);
                }
            }

            _followOffset += p_Velocity;
        }
        else
        {

            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 1, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, -1, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(1, 0, 0);
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                if (Vector3.Distance(target.position, transform.position) > 100)
                {
                    p_Velocity += new Vector3(0, 0, 50);
                }
                else
                {
                    p_Velocity += new Vector3(0, 0, 5);
                }
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                if (Vector3.Distance(target.position, transform.position) > 100)
                {
                    p_Velocity += new Vector3(0, 0, -50);
                }
                else
                {
                    p_Velocity += new Vector3(0, 0, -5);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    target = hit.transform;
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log(Vector3.Distance(target.position, transform.position));
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            follow = !follow;
            _followOffset = transform.position - target.position;

            followText.enabled = follow;
            
        }
        return p_Velocity;
    }
}