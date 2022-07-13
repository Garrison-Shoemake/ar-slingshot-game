using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Rigidbody))]
public class AmmoController : MonoBehaviour
{
    //force of the throw
    public float throwForce = 100f;

    // damping factors, keeps from going too straight
    public float throwDirectionX = 0.17f;
    public float throwDirectionY = 0.67f;
    public int score;
    public int ammoTotal;
    public int numberOfTargets;
    public Text scoreText;
    public Text ammoText;

    // camera offset for the ball
    public Vector3 cameraOffset = new Vector3(0f, -0.4f, 1f);

    private Vector3 startPosition;
    private Vector3 direction;
    private float startTime;
    private float endTime;
    private float duration;
    private bool throwStarted = false;
    private bool directionChosen = false;
    private NavMeshAgent hitAgent;

    [SerializeField] private GameObject ARCamera;
    [SerializeField] private ARSessionOrigin sessionOrigin;

    Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCamera = sessionOrigin.transform.Find("AR Camera").gameObject;
        transform.parent = ARCamera.transform;
        ResetBall();
        score = GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().score;
        scoreText = GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().scoreText;
        ammoTotal = GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().ammoTotal;
        ammoText = GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().ammoText;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            startTime = Time.time;
            throwStarted = true;
            directionChosen = false;
        } else if (Input.GetMouseButtonUp(0))
        {
            ammoTotal--;
            ammoText.text = "Ammo: " + ammoTotal.ToString();
            endTime = Time.time;
            duration = endTime - startTime;
            direction = Input.mousePosition - startPosition;
            directionChosen = true;
            if (ammoTotal == 0)
            {
                score = 0;
                ammoTotal = 8;
                GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().endGame();
            }
            if (numberOfTargets == 0)
                GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().endGame();
    
        }

        if (directionChosen)
        {
            rb.mass = 1;
            rb.useGravity = true;

            rb.AddForce(
                ARCamera.transform.forward * throwForce / duration +
                -ARCamera.transform.up * direction.y * throwDirectionY + 
                -ARCamera.transform.right * direction.x * throwDirectionX
            );
            
            startTime = 0.0f;
            duration = 0.0f;

            startPosition = new Vector3(0,0,0);
            direction = new Vector3(0,0,0);

            throwStarted = false;
            directionChosen = false;
        }
        if (Time.time - endTime >= 2 && Time.time - endTime <= 6)
        {
            if (ammoTotal > 0)
                ResetBall();
        }
    }

    private void ResetBall()
    {
        rb.mass = 0;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        endTime = 0.0f;

        Vector3 ballPos = ARCamera.transform.position + ARCamera.transform.forward * cameraOffset.z + ARCamera.transform.up * cameraOffset.y;
        transform.position = ballPos;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Target")
        {
            hitAgent = collision.gameObject.GetComponent<NavMeshAgent>();
            hitAgent.enabled = false;
            Destroy(collision.gameObject);
            score += 10;
            scoreText.text = "Score: " + score.ToString();
            numberOfTargets--;
            if (numberOfTargets == 0)
                GameObject.Find("AR Session Origin").GetComponent<PlaneFiltering>().endGame();
        }
    }
}
