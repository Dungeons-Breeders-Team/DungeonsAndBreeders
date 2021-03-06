﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PettingController : MonoBehaviour
{
    //regelt ansprache des animators etc
    public MonsterManager MM;
    public Image HeartMeter;
    public Animator HeartGlow;

    public Image[] XPBars = new Image[3];
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private float monsterStroked;
    private bool strokeDelay;
    private float petTime = 0.15f;

    private void Start()
    {
        GM = GameManager.Instance;
        MM = GM.homeMonsterManager;
        monsterStroked = 0;
        HeartMeter.fillAmount = 0;
    }

    private void Update()
    {
        if (GM.petting)
        {
            if (Input.touchCount >= 1)
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                Ray ray = Camera.main.ScreenPointToRay(touchPos);
                Debug.Log("Touch pos: " + touchPos);
                RaycastHit touchHit;

                if (Physics.Raycast(ray.origin, ray.direction, out touchHit))
                {
                    Debug.Log("Hit: "+ touchHit.transform.name);
                    if (touchHit.transform.CompareTag("Player"))
                    {
                        PetMonster();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                Debug.Log("Touch pos: " + mousePos);
                RaycastHit touchHit;

                if (Physics.Raycast(ray.origin, ray.direction, out touchHit))
                {
                    Debug.Log("Hit: " + touchHit.transform.name);
                    if (touchHit.transform.CompareTag("Player"))
                    {
                        PetMonster();
                    }
                }
            }
        }
    }

    public void SetXPBars()
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel < 7)
        {
            //print("cur XP: " + GM.CurMonsters[(int)GM.curMonsterSlot].MonsterXP);
            float[] fillAmount = new float[3];

            switch (GM.curScreen)
            {
                case eScene.home:
                    fillAmount = GM.homeMonsterManager.XPBarFillAmount();
                    break;
                case eScene.runner:
                    fillAmount = GM.runnerMonsterManager.XPBarFillAmount();
                    break;
                case eScene.slidingpicture:
                    //TODO: put monster manager of sliding picture game here
                    break;
                default:
                    print("cant find monster manager cause I dont know which scene we're in");
                    break;
            }
            XPBars[0].fillAmount = fillAmount[0];
            XPBars[1].fillAmount = fillAmount[1];
            XPBars[2].fillAmount = fillAmount[2];
        }
    }

    public IEnumerator cEndPetSession()
    {
        HeartGlow.SetTrigger("full");
        yield return new WaitForSeconds(1.5f);

        if (GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage != eMonsterStage.Adult)
        {
            SetXPBars();
            GM.homeUI.ShowPetSessionResult();

            GM.CurMonsters[(int)GM.curMonsterSlot].StrokeTimes += 1;
            print("this monster has been stroked " + GM.CurMonsters[(int)GM.curMonsterSlot].StrokeTimes + " times");
            //TODO change monster idle to happy

            MM.SetMonsterXP(GM.XPGainPerPettingSession);
            if (GM.CurMonsters[(int)GM.curMonsterSlot].StrokeTimes % 5 == 0)
            {
                MM.SetMonsterXP(GM.XPAffectionBonus);
            }

            yield return new WaitForSeconds(1.0f);

            //the whole shitty check for levelup and call levelup scene
            while (MM.CheckForMonsterLevelUp())
            {
                SetXPBars();   
                yield return new WaitForSeconds(0.5f);

                if (MM.CheckForStageChange())
                {
                    StartCoroutine(MM.cLevelUpMonster(GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage, MM.MonsterSpawn[(int)GM.curMonsterSlot]));
                    yield return new WaitForSeconds(1.0f);
                }
            }

            SetXPBars();
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            MM.SetMonsterXP(GM.XPGainPerPettingSession);
            if (GM.CurMonsters[(int)GM.curMonsterSlot].StrokeTimes > 1)
            {
                MM.SetMonsterXP(GM.XPAffectionBonus);
            }
        }
        GM.CurMonsters[GM.curMonsterID].IsHappy = true;
        EndSession();
    }

    public void EndSession()
    {
        strokeDelay = false;
        monsterStroked = 0;
        //HeartGlow.SetTrigger("reset");
        HeartMeter.fillAmount = monsterStroked / GM.StrokesPerPettingSession;        
        GM.homeUI.ExitPetSession();
    }

    public void PetMonster()
    {
        if (!strokeDelay && monsterStroked < GM.StrokesPerPettingSession)
        {
            //print("you have pet the monster! :)");
            monsterStroked += 1;
            HeartMeter.fillAmount = monsterStroked / GM.StrokesPerPettingSession;
            MM.monsterAnim[MM.CurMonster.SlotID].SetTrigger("stroke");
            strokeDelay = true;

            if (monsterStroked >= GM.StrokesPerPettingSession)
            {
                GM.petting = false;
                GM.homeUI.TogglePetBackButton(false);
                MM.monsterAnim[MM.CurMonster.SlotID].SetBool("isSad", false);
                print("oh yay, monster is happy");                
                StartCoroutine(cEndPetSession());
            }
            else
            {
                StartCoroutine(cStrokingMonstser());
            }
        }
        else
        {
            print("wait a moment before stroking again");
        }
    }

    private IEnumerator cStrokingMonstser()
    {
        int pos = 0;
        switch(GM.curMonsterID)
        {
            case 0:
                pos = 0;
                break;
            case 1:
                pos = 1;
                break;
            case 2:
                pos = 2;
                break;
            default:
                pos = 1;
                break;
        }
        yield return new WaitForSeconds(0.2f);
        GM.vfx_home.SpawnEffectViaInt(VFX_Home.VFX.Pet, pos);        
        GM.vfx_home.SpawnEffectViaInt(VFX_Home.VFX.PetLow, pos);        
        yield return new WaitForSeconds(petTime);
        strokeDelay = false;
    }
}
