﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RunnerController : MonoBehaviour
{
    public EndlessRunnerVars vars;

    public float CollectableValue;
    public float VerticalSpeed;
    public float HorizontalSpeed;
    [Tooltip("Speed multiplier for every checkpoint")]
    public float SpeedModifier = 1f;    
    [Tooltip("Added value for the powerup per Checkpoint")]
    public float ValueModifier = 1f;    
    [Tooltip("Multiplier for reward after reaching the goal")]
    public float GoalReward = 1f;

    [Header("Do no touch unless you are The Programmer")]
    public float CollectedCount;
    public Text CollectedText;
    public Text GameEndText;
    [Tooltip("Temporary feedback text for XP gain")]
    public Text CollectedFeedbackText;
    [Tooltip("Prefabs of the tiles of this level")]
    public GameObject[] LevelTiles;
    public int curTile;
    public GameObject LevelSpawn;

    //Spawned when player collects item
    //public GameObject FeedbackPrefab;
    //public RectTransform FeedbackSpawm;

    void Start()
    {
        vars.ResetValues();
        GameEndText.text = "";
        InstantiateFirstTiles();
    }
       
    public void InstantiateFirstTiles()
    {
        Instantiate(LevelTiles[curTile], LevelSpawn.transform);
        Instantiate(LevelTiles[curTile + 1], LevelSpawn.transform);
    }


    public void UpdateCollectedCount(float value)
    {
        CollectedCount += value;
        CollectedText.text = "" + Mathf.RoundToInt(CollectedCount);
    }

    public IEnumerator cOnCollectFeedback()
    {
        CollectedFeedbackText.text = "+" + CollectableValue + "XP";
        yield return new WaitForSeconds(0.1f);
        CollectedFeedbackText.text = "";

        //GameObject feedback = Instantiate(FeedbackPrefab, FeedbackSpawm);
        //feedback.transform.SetParent(FeedbackSpawm, true);
        //Destroy(feedback, 0.3f);
    }

    public IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(0.5f);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }


    //
    //
    public void ResetValues()
    {
        CollectedCount = 0;
        curTile = 0;
    }

    public void ModifySpeed()
    {
        VerticalSpeed *= SpeedModifier;
    }

    public void AddCollectableValue()
    {
        CollectableValue += ValueModifier;
    }

    public void CollectedCountWinModifier()
    {
        CollectedCount *= GoalReward;
    }
}
