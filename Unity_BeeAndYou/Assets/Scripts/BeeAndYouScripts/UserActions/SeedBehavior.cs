using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SeedBehavior : MonoBehaviour
{
    bool seedActive = false;
    bool firstPlanting = true;
    bool secondPlanting = false;

    GameObject seed;
    GameObject seedModel;
    public GameObject seedPrefab;

    public GameObject flowerPrefab;
    public GameObject flowerTypeBee;
    public GameObject flowerTypeButterfly;
    public GameObject flowerTypeDragonfly;

    public Vector3 seedSpawnPos;
    public GameObject firstSeedSpawner;
    Vector3 seedSpawningPos;
    Vector3 seedTrackedPos;

    //public Vector3 seedSpawnPos = (55,33,55);
    //Vector3 seedTrackedPos = (0,50,0);

    //temporary
    public float plantingHeight = 0.05f;

    public GameObject UI;


    public GameObject beeControl;
    public GameObject butterflyControl;
    public GameObject dragonflyControl;

    private int beeAmount;
    private int butterflyAmount;
    private int dragonflyAmount;

    private GameObject beeLanding;
    private GameObject butterflyLanding;
    private GameObject dragonflyLanding;

    private int beeLandingActiveSpots;
    private int butterflyLandingActiveSpots;
    private int dragonflyLandingActiveSpots;

    public GameObject landingPrefab;

    Vector3 plantLanding;

    private GameObject currentFlower;

    private string currentType;

    // Start is called before the first frame update
    void Start()
    {
      //seedSpawnPos = new Vector3(55.0f,33.0f,55.0f);
      seedTrackedPos = new Vector3(55.0f,33.0f,55.0f);
      Destroy(seed);
      seedActive = false;

      beeAmount = beeControl.GetComponent<FlockController>()._childAmount;
      butterflyAmount = butterflyControl.GetComponent<FlockController>()._childAmount;
      dragonflyAmount = dragonflyControl.GetComponent<FlockController>()._childAmount;

      beeLanding = beeControl.transform.GetChild(0).gameObject;
      butterflyLanding = butterflyControl.transform.GetChild(0).gameObject;
      dragonflyLanding = dragonflyControl.transform.GetChild(0).gameObject;

      beeLandingActiveSpots = beeLanding.GetComponent<LandingSpotController>()._activeLandingSpots;
      butterflyLandingActiveSpots = butterflyLanding.GetComponent<LandingSpotController>()._activeLandingSpots;
      dragonflyLandingActiveSpots = dragonflyLanding.GetComponent<LandingSpotController>()._activeLandingSpots;


    }

    public void SeedSpawn()
    {
      //spawn the seed, get the rigidbody, set active
      if (firstPlanting)
      {
        seedSpawningPos = firstSeedSpawner.GetComponent<Transform>().position;
      }
      else {
        seedSpawningPos = seedSpawnPos;
      }

      seed = Instantiate(seedPrefab, seedSpawningPos, Quaternion.identity);
      seedModel = seed.transform.GetChild(0).gameObject;

      seedModel.GetComponent<Rigidbody>().useGravity = false;

      if (firstPlanting)
      {
        seedModel.GetComponent<SeedGrabbed>().firstGrab = true;

      }
      else {
        seedModel.GetComponent<SeedGrabbed>().firstGrab = false;
        secondPlanting = false;
      }
      seedActive = true;

    }

    public void FirstGrab()
    {
      firstPlanting = false;
      secondPlanting = true;
      UI.GetComponent<UI_script>().F_FirstPlanting();
    }

    public void SeedPlanting()
    {
      var randomPick = Random.Range(0, 150);
      if (randomPick < 70)
      {
        currentType = "bee";
        flowerPrefab = flowerTypeBee;
      }
      else if (randomPick > 125)
      {
        currentType = "dragonfly";
        flowerPrefab = flowerTypeDragonfly;
      }
      else
      {
        currentType = "butterfly";
        flowerPrefab = flowerTypeButterfly;
      }

      // track current vector3 position of the seed, destroy it and instantiate a flower instead
      //seedTrackedPos = seed.transform.position;
      seed.SetActive(false);
      Destroy(seed);
      seedTrackedPos.y = -0.1f;
      seedActive = false;

      Vector3 center = Vector3.zero; // center of the circle
      float radius = 5.2f; // radius of the circle

      float distance = Vector3.Distance(seedTrackedPos, center);

      if (distance > radius)
      {
          if (secondPlanting)
          {
            firstPlanting = true;
            secondPlanting = false;
          }

          SeedSpawn();

          return;

          // seed = Instantiate(seedPrefab, seedSpawningPos, Quaternion.identity);
          // seedModel = seed.transform.GetChild(0).gameObject;
          //
          // seedModel.GetComponent<Rigidbody>().useGravity = false;
          // seedActive = true;
          // SeedPlanting();
          // return;
      }

      currentFlower = Instantiate(flowerPrefab, seedTrackedPos, Quaternion.identity) as GameObject;

      if (UI.activeInHierarchy && secondPlanting)
      {
        UI.GetComponent<UI_script>().G_Pollinator();
      }
      else if (!secondPlanting)
      {
        UI.SetActive(false);
      }

      plantLanding = transform.TransformPoint(currentFlower.transform.GetChild(0).gameObject.GetComponent<Transform>().position);

      PollinatorSpawn();
    }

    public void PollinatorSpawn()
    {
      if (currentType == "bee")
      {
        (Instantiate (landingPrefab, plantLanding, Quaternion.identity) as GameObject).transform.parent = beeLanding.transform;

        beeAmount +=1;
        beeLandingActiveSpots +=1;

        beeControl.GetComponent<FlockController>()._childAmount = beeAmount;
        beeLanding.GetComponent<LandingSpotController>()._activeLandingSpots = beeLandingActiveSpots;

        Debug.Log("bees?");
      }
      else if (currentType == "butterfly")
      {
        (Instantiate (landingPrefab, plantLanding, Quaternion.identity) as GameObject).transform.parent = butterflyLanding.transform;

        butterflyAmount +=1;
        butterflyLandingActiveSpots +=1;

        butterflyControl.GetComponent<FlockController>()._childAmount = butterflyAmount;
        butterflyLanding.GetComponent<LandingSpotController>()._activeLandingSpots = butterflyLandingActiveSpots;

        Debug.Log("butterfly");
      }
      else if (currentType == "dragonfly")
      {
        (Instantiate (landingPrefab, plantLanding, Quaternion.identity) as GameObject).transform.parent = dragonflyLanding.transform;

        dragonflyAmount +=1;
        dragonflyLandingActiveSpots +=1;

        dragonflyControl.GetComponent<FlockController>()._childAmount = dragonflyAmount;
        dragonflyLanding.GetComponent<LandingSpotController>()._activeLandingSpots = dragonflyLandingActiveSpots;

        Debug.Log("dragonfly");
      }

      Invoke("ServeNextSeed", 5.0f);
    }

    public void ServeNextSeed()
    {
      firstPlanting = false;
      UI.GetComponent<UI_script>().H_Pollinating();

      SeedSpawn();
    }



    // Update is called once per frame
    void Update()
    {
      // !!!!!!! temporary, to substitute with digging gesture recognition
      if (seedActive == false && firstPlanting == false)
      {

      }
      else
      {
        //GetComponent<Rigidbody>().position
        seedTrackedPos = seedModel.GetComponent<Rigidbody>().position;
        if (seedTrackedPos.y <= plantingHeight) //should also check if user is holding it
        {
          SeedPlanting();
        }
      }
    }
}
