﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_Home : MonsterManager
{
    public GameObject[] Lock;
    public GameObject[] Plus;
    public Transform EggSpawn;

    public void Awake()
    {
        GetGameManager();
        GM.homeMonsterManager = this;
        //print("home mm has been started");
    }
    public void Start()
    {
        SetSlotSymbol();
        SpawnAllCurrentMonsters();
        CalculateMonsterValue();
    }

    public void SetSlotSymbol()
    {
        for (int i = 0; i < 3; i++)
        {
            if (GM.CurMonsters[i].Monster == null && !GM.CurMonsters[i].Unlocked)
            {
                Lock[i].SetActive(true);
                Plus[i].SetActive(false);
            }
            else if (GM.CurMonsters[i].Monster == null && GM.CurMonsters[i].Unlocked)
            {
                Plus[i].SetActive(true);
                Lock[i].SetActive(false);
            }
            else
            {
                Plus[i].SetActive(false);
                Lock[i].SetActive(false);
            }
        }
    }
    public void SpawnAllCurrentMonsters()
    {
        print("spawning monsters");

        for (int i = 0; i< GM.CurMonsters.Length;i++)
        {
            if (GM.CurMonsters[i].Monster == null)
            {
                print("slot " + i + " is empty");
                //print("rarity: " + (int)GM.CurMonsters[i].Rarity);
                //print("stage: " + (int)GM.CurMonsters[i].MonsterStage);
                //print("spawn: " + MonsterSpawn[i].name);
            }
            else
            {
                print("slot " + i + " is NOT empty");
                //print("rarity: " + (int)GM.CurMonsters[i].Rarity);
                //print("stage: " + (int)GM.CurMonsters[i].MonsterStage);
                //print("spawn: " + MonsterSpawn[i].name);
                SpawnAnyMonster(GM.CurMonsters[i].Monster.CreaturePrefabs[(int)GM.CurMonsters[i].Rarity, (int)GM.CurMonsters[i].MonsterStage], MonsterSpawn[i]);
            }
        }
    }

    //For the egg hatching
    public void SetEggRarity()
    {
        //TODO Rarity errechnen
        GM.CurMonsters[(int)GM.curMonsterSlot].Rarity = (eRarity)Random.Range(0, 3);

        switch (GM.CurMonsters[(int)GM.curMonsterSlot].Rarity)
        {
            case eRarity.normal:
                GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current = newLevelThreshold();
                break;
            case eRarity.rare:
                GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current = newLevelThreshold(GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MultiplicatorRare);
                
                break;
            case eRarity.legendary:
                GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current = newLevelThreshold(GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MultiplicatorLegendary);

                break;
            default:
                break;
        }
        print(GM.CurMonsters[(int)GM.curMonsterSlot].Rarity);
    }

    public float[] newLevelThreshold(float multiplicator = 1.00f)
    {
        for (int i = 0; i < GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current.Length; i++)
        {
            GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current[i] = GM.CurMonsters[(int)GM.curMonsterSlot].Monster.LevelThreshold_normal[i] * multiplicator;
            print(i+" = "+GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current[i]);
        }
        return GM.CurMonsters[(int)GM.curMonsterSlot].LevelThreshold_current;
    }

    public IEnumerator cSpawnEgg()
    {
        //effect for spawning
        //yield return new WaitForSeconds(0.5f);
        CurMonster.MonsterStage = eMonsterStage.Egg;
        SpawnAnyMonster(CurMonster.Monster.EggPrefab, EggSpawn);
        yield return null;
    }

    public IEnumerator cHatchEgg(Transform monsterSpawn)
    {
        CurMonster.MonsterStage = eMonsterStage.Baby;
        //play effect for level up            
        Destroy(monsterBody[SlotID], 0.25f);
        yield return new WaitForSeconds(0.3f);
        GM.homeMonsterManager.SpawnCurrentMonster(monsterSpawn);
        yield return new WaitForSeconds(0.3f);
        GM.homeUI.SetUIStage(HomeUI.eHomeUIScene.Home);
        GM.homeMonsterManager.SpawnCurrentMonster(MonsterSpawn[SlotID]);
        yield return null;
    }
}
