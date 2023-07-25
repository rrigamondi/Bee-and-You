using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_script : MonoBehaviour
{
  SpriteRenderer spriteRenderer;
  public Sprite spriteA;
  public Sprite spriteB;
  public Sprite spriteC;
  public Sprite spriteD;
  public Sprite spriteE;
  public Sprite spriteF;
  public Sprite spriteG;
  public Sprite spriteH;
  public GameObject canvas;
  public GameObject backBut;
  public GameObject eventSys;
  public int screenCounter = 0;

  public GameObject leftControlSelect;
  public GameObject leftControlMove;


    // Start is called before the first frame update
    void Start()
    {
      spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
      screenCounter = 0;

      leftControlSelect.SetActive(true);
      leftControlMove.SetActive(false);

      A_Intro();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void A_Intro()
    {
      backBut.SetActive(false);
      spriteRenderer.sprite = spriteA;
    }

    public void B_Controls()
    {
      backBut.SetActive(true);
      spriteRenderer.sprite = spriteB;
    }

    public void C_HeartInfo()
    {
      spriteRenderer.sprite = spriteC;
    }

    public void D_BreathInfo()
    {
      spriteRenderer.sprite = spriteD;
    }

    public void E_FirstSeed()
    {
      canvas.SetActive(false);
      spriteRenderer.sprite = spriteE;

      eventSys.GetComponent<SeedBehavior>().SeedSpawn();

      leftControlSelect.SetActive(false);
      leftControlMove.SetActive(true);
    }

    public void F_FirstPlanting()
    {
      spriteRenderer.sprite = spriteF;
    }

    public void G_Pollinator()
    {
      spriteRenderer.sprite = spriteG;
    }

    public void H_Pollinating()
    {
      spriteRenderer.sprite = spriteH;
    }

    public void NextButton()
    {
      screenCounter += 1;

      if (screenCounter == 1){B_Controls();}
      else if (screenCounter == 2){screenCounter = 3;D_BreathInfo();}
      else if (screenCounter == 3){D_BreathInfo();}
      else if (screenCounter == 4){E_FirstSeed();}
    }

    public void BackButton()
    {
      screenCounter -= 1;

      if (screenCounter == 1){B_Controls();}
      else if (screenCounter == 2){screenCounter = 1;B_Controls();}
      else if (screenCounter == 3){D_BreathInfo();}
      else if (screenCounter == 4){E_FirstSeed();}
      else if (screenCounter == 0){A_Intro();}
    }

}
