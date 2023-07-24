using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBioManager : MonoBehaviour
{
    public GameObject dataSource;
    public GameObject windZone;
    public GameObject windVFX;

    private int multiplier = 20;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      var data = dataSource.GetComponent<Hybrid8Test>().outputResp*multiplier;

      float normal = Mathf.InverseLerp(1*multiplier, 1100*multiplier, data);
      float mainValue = -Mathf.Lerp(-5, 5, normal);
      float freqValue = -Mathf.Lerp(-2, 2, normal)/2;

      if (data == 0)
      {
        mainValue = 1f;
        freqValue = 0.5f;
      }

      windZone.GetComponent<WindZone>().windMain = mainValue;
      windZone.GetComponent<WindZone>().windPulseFrequency = freqValue;
    }


}
