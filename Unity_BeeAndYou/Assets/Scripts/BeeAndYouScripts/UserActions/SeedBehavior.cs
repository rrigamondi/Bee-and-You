using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SeedBehavior : MonoBehaviour
{
    bool seedActive = false;
    bool firstPlanting = true;
    GameObject seed;
    GameObject seedModel;
    public GameObject seedPrefab;
    public GameObject flowerPrefab;

    public Vector3 seedSpawnPos;
    public GameObject firstSeedSpawner;
    Vector3 seedSpawningPos;
    Vector3 seedTrackedPos;

    //public Vector3 seedSpawnPos = (55,33,55);
    //Vector3 seedTrackedPos = (0,50,0);

    //temporary
    public float plantingHeight = 0.05f;

    public GameObject UI;

    // Start is called before the first frame update
    void Start()
    {
      //seedSpawnPos = new Vector3(55.0f,33.0f,55.0f);
      seedTrackedPos = new Vector3(55.0f,33.0f,55.0f);
      Destroy(seed);
      seedActive = false;
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
      }
      seedActive = true;
    }

    public void FirstGrab()
    {
      firstPlanting = false;
      UI.GetComponent<UI_script>().F_FirstPlanting();
    }

    public void SeedPlanting()
    {
      // track current vector3 position of the seed, destroy it and instantiate a flower instead
      //seedTrackedPos = seed.transform.position;
      seed.SetActive(false);
      Destroy(seed);
      seedTrackedPos.y = -0.1f;
      // !! will need to differentiate flowers with multiple prefabs, could have different seeds to if we have enough time
      Instantiate(flowerPrefab, seedTrackedPos, Quaternion.identity);

      if (UI.activeInHierarchy)
      {
        UI.GetComponent<UI_script>().G_Pollinator();
      }
      seedActive = false;
    }

    // Update is called once per frame
    void Update()
    {
      // !!!!!!! temporary, to substitute with digging gesture recognition
      if (seedActive == false && firstPlanting == false)
      {
        SeedSpawn();
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
