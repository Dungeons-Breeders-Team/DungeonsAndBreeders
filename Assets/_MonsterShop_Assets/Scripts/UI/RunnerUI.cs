﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RunnerUI : UIController
{
    //public Text CollectedText;
    //public Text GameEndText;
    //public Text CollectedFeedbackText;
    public Animator LevelUpScreen;
    bool gameEnd;
    private enum eMenus
    {
        StartButton,
        InGameStuff,
        ResultScreen,
        EndResultButton
    }

    private enum eButtons
    {
        LeftButton,
        RightButton,
        StartButton
    }

    private enum eTextfields
    {
        CollectedCount,
        OnCollectFeedback,
        GameOverFeedback,
        CollectedNo,
        XPValue,
        StartText,
        MonsterLevel
    }

    private void Awake()
    {
        StartMenu();
        gameEnd = false;
    }
    
    private void Start()
    {
        GetGameManager();
        GM.runnerUI = this;
    }

    private void StartMenu()
    {
        EnableMenu((int)eMenus.StartButton);
        DisableButton((int)eButtons.LeftButton);
        DisableButton((int)eButtons.RightButton);
        DisableButton((int)eButtons.StartButton);
        SetText((int)eTextfields.StartText, "");
        StartCoroutine(cEnableStart());
    }

    public IEnumerator cEnableStart()
    {
        yield return new WaitForSeconds(0.5f);
        GM.runnerUI.EnableStartButton();
    }

    // Called by StartButton
    public void StartPressed()
    {
        DisableMenu((int)eMenus.StartButton);        
        EnableButton((int)eButtons.LeftButton);
        EnableButton((int)eButtons.RightButton);
        StartCoroutine(GM.runnerMonsterManager.cSpawnMonsterinRunner());
    }

    public void SetGameEndText(string text)
    {
        GM.runnerUI.SetText((int)eTextfields.GameOverFeedback, text);
    }

    // called after game over or win 
    public IEnumerator cShowResult()
    {
        //yield return new WaitForSeconds(0.5f);

        if (!gameEnd)
        {
            gameEnd = true;

            SetXPBars();

            // set result scene menus and values
            DisableMenu((int)eMenus.InGameStuff);
            GM.runnerMonsterManager.SpawnCurrentMonster(GM.runnerMonsterManager.ResultMonsterSpawn);
            SetText((int)eTextfields.CollectedNo, GM.runnerController.CollectedCount + " x");
            SetText((int)eTextfields.XPValue, GM.runnerController.CollectedXP + " XP");
            EnableMenu((int)eMenus.ResultScreen);
            // move camera on result scene
            Camera.main.transform.position = GM.runnerController.ResultCamPos;

            yield return new WaitForSeconds(1f);

            // gain xp and update xp bar
            GM.runnerMonsterManager.SetMonsterXP(GM.runnerController.CollectedXP);
            SetXPBars();

            yield return new WaitForSeconds(0.5f);

            // as long as there is a lvl up
            while (GM.runnerMonsterManager.CheckForMonsterLevelUp())
            {
                if (GM.runnerMonsterManager.CheckForStageChange())
                {
                    StartCoroutine(GM.runnerMonsterManager.cLevelUpMonster(GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage, GM.runnerMonsterManager.ResultMonsterSpawn));
                    yield return new WaitForSeconds(1.0f);
                }
                SetXPBars();
                yield return new WaitForSeconds(0.5f);
            }

            //GM.runnerMonsterManager.CalculateMonsterValue();
            yield return new WaitForSeconds(0.25f);
        // Enable EndResultButton for player to tap and go back to home scene
        EnableMenu((int)eMenus.EndResultButton);
        }
    }

    // EndResultButtons function
    public void TapResult()
    {
        GM.RestartNotifs();
        GM.LoadHomeScene();
    }

    // Endabling Startbutton after a short wait to make sure everything has loaded
    public void EnableStartButton()
    {
        //print("enabling start button");
        SetText((int)eTextfields.StartText, "Tap to start");
        EnableButton((int)eButtons.StartButton);
    }
}
