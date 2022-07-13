using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaneFiltering : MonoBehaviour
{
    private ARPlaneManager aRPlaneManager;
    private ARRaycastManager aRRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector2 touchPos;
    public Vector3 cameraOffset = new Vector3(0f, -0.4f, 1f);
    public GameObject savedPlane;
    public int score;
    public int ammoTotal = 8;
    public int numberOfTargets;
    public Text scoreText;
    public Text endScore;
    public Text ammoText;
    private Pose fieldPose;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject spawnedAmmo;
    [SerializeField] private GameObject ammo;
    [SerializeField] private NavMeshAgent spawnedTarget;
    [SerializeField] private GameObject ARCamera;
    [SerializeField] private NavMeshAgent target;
    [SerializeField] private Material selected;
    [SerializeField] private Material unselected;
    [SerializeField] private Material activatedField;
    

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
    }
    private void Update()
    {
       if(aRPlaneManager.enabled)
       {
            if (!getTouchPosition(out Vector2 touchPos))
                return;
        
            if (aRRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
            {
                var field = aRPlaneManager.GetPlane(hits[0].trackableId);
                var fieldId = hits[0].trackableId;
                fieldPose = hits[0].pose;
                foreach (var plane in aRPlaneManager.trackables)
                {
                    if (plane.trackableId != fieldId)
                    {
                        Destroy(plane.gameObject);
                    }
                    else
                    {
                        field.GetComponent<MeshRenderer>().material = selected;
                        savedPlane = plane.gameObject;
                        savedPlane.GetComponent<NavMeshBake>().bakeMesh();
                        startButton.SetActive(true);
                    }
                }
                aRPlaneManager.enabled = false;
                aRRaycastManager.enabled = false;
            }
        }
    }

    public void gameStart()
    {
        score = 0;
        ammoTotal = 8;
        ammoText.text = "Ammo: " + ammoTotal.ToString();
        scoreText.text = "Score: " + score.ToString();
        savedPlane.GetComponent<MeshRenderer>().material = activatedField;
        spawnTargets();
        spawnAmmo();
        menuCanvas.SetActive(false);
        endGameMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }
    public void quitGame()
    {
        Application.Quit();
    }

    public void restartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void playAgain()
    {
        //destroy all targets before restarting here
        spawnTargets();
        score = 0;
        ammoTotal = 7;
        menuCanvas.SetActive(false);
        endGameMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }
    public void endGame()
    {
        endScore.text = "Score: " + score.ToString();
        menuCanvas.SetActive(false);
        endGameMenu.SetActive(true);
        inGameMenu.SetActive(false);
    }

    private void spawnTargets()
    {
        for (var i = 0; i < numberOfTargets; i++)
        {
            spawnedTarget = Instantiate(target, fieldPose.position, fieldPose.rotation);
        }
    }
    private void spawnAmmo()
    {
        Vector3 ballPos = ARCamera.transform.position + ARCamera.transform.forward * cameraOffset.z + ARCamera.transform.up * cameraOffset.y;
        spawnedAmmo = Instantiate(ammo, ballPos, Quaternion.identity);
    }

    private Vector3 randomVector()
    {
        var x = UnityEngine.Random.Range(0, fieldPose.position.x);
        var y = 0;
        var z = UnityEngine.Random.Range(0, fieldPose.position.z);
        return new Vector3(x,y,z);
    }

    bool getTouchPosition(out Vector2 touchPos)
    {
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch((0)).position;
            return true;
        }
        touchPos = default;
        return false;
    }
}
