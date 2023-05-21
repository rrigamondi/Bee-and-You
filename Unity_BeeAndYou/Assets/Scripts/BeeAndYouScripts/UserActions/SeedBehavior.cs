using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBehavior : MonoBehaviour
{
    public bool seedActive = false;
    public GameObject seed;
    public GameObject seedPrefab;
    public GameObject flowerPrefab;

    public Vector3 seedSpawnPos;
    public Vector3 seedTrackedPos;

    //temporary
    public int plantingHeight = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      // !!!!!!! temporary, to substitute with digging gesture recognition
      if (seedActive == true)
      {
        seedTrackedPos = seed.transform.position;
        if (seedTrackedPos.x <= plantingHeight) //should also check if user is holding it
        {
          SeedPlanting();
        }
      }
    }

    public void SeedSpawn()
    {
      //spawn the seed, get the rigidbody, set active
      GameObject seed = Instantiate(seedPrefab, seedSpawnPos, Quaternion.identity);
      GameObject seedModel = seed.transform.GetChild(0).gameObject;

      seedModel.GetComponent<Rigidbody>().useGravity = false;
      // !! need to reactivate gravity when user grabs it
      // ! would be great to add a glowing effect with a 2d sprite, that is active when gravity isn't, to attract user attention

      seedActive = true;
    }

    public void SeedPlanting()
    {
      // track current vector3 position of the seed, destroy it and instantiate a flower instead
      seedTrackedPos = seed.transform.position;
      Destroy(seed);

      // !! will need to differentiate flowers with multiple prefabs, could have different seeds to if we have enough time
      Instantiate(flowerPrefab, seedTrackedPos, Quaternion.identity);

      seedActive = false;
    }
}
