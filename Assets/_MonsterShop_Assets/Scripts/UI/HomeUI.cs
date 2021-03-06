using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeUI : UIController
{
    [HideInInspector]
    public eHomeUIScene curScene;    
    private Monster curEgg;
    private int hatchTaps;
    //private Vector3 camHomePos { get { return GM.CurCamHomePos; } }
    private Vector3 camShopPos;
    //private Vector3 camDungeonPos;
    private int EggHatchCount;
    private int GoldSkipTapped;

    private bool swipeDisabled;
    private bool moneyCountFinished;

    // called via (int)GM.curMonsterSlot
    public SpriteRenderer[] HomeBGs;

    public Sprite[] BGsprites;
    public GameObject CoinPrefab;
    public RectTransform CoinSpawn;
    public RectTransform CoinDestination;
    public RectTransform[] CoinsPos;

    public Animator[] CageTop;
    public GameObject[] RarityStars = new GameObject[3];
    public GameObject AllCages;
    public Animator D_Signature;
    public CameraMovement camMovement;
    public Animator PlayerGold;
    public Animator DLTimerAnim;
    public Animator LevelUpScreen;

    private int totalValue;
    private bool signed = false;
    private bool goldCountSkip;

    private enum BGsprite
    {
        homeLeft,
        homeMiddle,
        homeRight,
        dungeonLeft,
        dungeonMiddle,
        dungeonRight
    }

    private enum eMenus
    {
        PlayerInfo,
        H_MonsterStats,
        XPBar,
        Home,
        Shop,
        Dungeon,
        MiniGameWindow,
        H_BottomButtons,
        S_ShopUI,
        S_BottomButtons,
        S_PurchaseConfirm,
        S_EggMenu,
        S_TappEggButton,
        D_BottomButtons,
        SwipeButtons,
        LoadingScreen,
        LockedButton,
        AddButton,
        PopupInfoWindow,
        UnlockYNButtons,
        ClosePopupButton,
        D_MonsterStats,
        D_SaleConfirm,
        SwipeLeftInput,
        SwipeRightInput,
        D_SaleConfirmButton,
        D_LeaveConfirmButton,
        D_SalesContract,
        Kompendium,
        Petting,
        PetMeSymbol,
        TapMonster,
        PettingInfo,
        PettingXPBar,
        DungeonSellButton,
        H_TrainButtonTimeOut,
        CheatWindow,
        TutorialIntro,
        RunnerLevelChoice,
        PetBackButton,
        SkipGoldButton,
        EggGlow
    }

    private enum  eButtons
    {
        H_Train,
        H_Feed,
        H_Shop,
        S_ExitShop,
        S_PurchaseYes,
        S_PurchaseNo,
        SwipeLeft,
        SwipeRight,
        DungeonSellButton,
        K_PaageLeft,
        K_PageRight
    }

    private enum eTextfields
    {
        GoldCount,
        H_MonsterTypeandStage,
        H_MonsterValue,        
        H_MonsterLevel,
        ShopDialogue,
        DungeonDialogue,
        PopupInfoText,
        D_MonsterTypeandStage,
        D_MonsterValue,
        D_MonsterLevel,
        D_SoldMonster1,
        K_MonsterFluff,
        K_MonsterName,
        K_MonsterRarity,
        K_MonsterHatchCount,
        K_MonsterHighestPrice,
        H_TrainButtonTimer,
        H_DungeonLordTimer
    }

    public enum eHomeUIScene
    {
        Home,
        Eggshop,
        Minigame,
        Dungeonlord,
        Kompendium,
        Petting,
        Cheats,
        Tutorial,
        none
    }

    public void Awake()
    {
        GetGameManager();
        GM.homeUI = this;
        camShopPos = new Vector3(0.0f, 20.0f, -20.00f);
        //camDungeonPos = new Vector3(-10.801f, 20.0f, -10.001f);
    }

    private void Start()
    {
        EggHatchCount = GM.TapsToHatch;
        ShowMonsterStats(GM.CurMonsters[(int)GM.curMonsterSlot].Monster != null);
        SetMonsterXPBarUndLevel();
        SetSlotSymbol();
        SetMonsterValue();
        DisableSwiping(false);
        if (!GM.TutorialOn)
        {
            SetUIStage(eHomeUIScene.Home);
        }
        else
        {
            GM.SendNotification(System.DateTime.Now.AddSeconds(0.5f), "The dungeonlord needs new monsters!", "Open your Monster Shop!");
            SetUIStage(HomeUI.eHomeUIScene.Tutorial);
            GM.TutorialOn = false;
        }
    }

    private void Update()
    {
        //if (!GM.CurMonsters[GM.curMonsterID].IsHappy && curScene == eHomeUIScene.Home)
        //{
        //    if (Input.touchCount >= 1)
        //    {
        //        Vector2 touchPos = Input.GetTouch(0).position;
        //        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        //        Debug.Log("Touch pos: " + touchPos);
        //        RaycastHit touchHit;

        //        if (Physics.Raycast(ray.origin, ray.direction, out touchHit))
        //        {
        //            Debug.Log("Hit: " + touchHit.transform.name);
        //            if (touchHit.transform.CompareTag("Player"))
        //            {
        //                GoToPetSession();
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Dis/Enables the Back button in the petting scene
    /// </summary>
    /// <param name="enable"></param>
    public void TogglePetBackButton(bool enable)
    {
        if (enable)
        {
            EnableMenu((int)eMenus.PetBackButton);
        }
        else
        {
            DisableMenu((int)eMenus.PetBackButton);
        }
    }

    /// <summary>
    /// true = the DL button is not pulsing
    /// </summary>
    /// <param name="still"></param>
    public void UpdateDLButtonAnim(bool still)
    {
        print("DL timer not pulsing: " + still);
        DLTimerAnim.SetBool("still", still);      
    }

    public void UpdateDLTimer(string info)
    {
        SetText((int)eTextfields.H_DungeonLordTimer, info);
    }

    public void UpdatePlayTimer(string time)
    {
        SetText((int)eTextfields.H_TrainButtonTimer, time);
    }

    /// <summary>
    /// Shows the minigame timer
    /// </summary>
    /// <param name="show"></param>
    public void SetPlayTimer(bool show)
    {
        if (show)
        {
            EnableMenu((int)eMenus.H_TrainButtonTimeOut);
            //TrainButtonActive(false);
        }
        else
        {
            DisableMenu((int)eMenus.H_TrainButtonTimeOut);
            //TrainButtonActive(true);
        }
    }
   
    /// <summary>
    /// Shows Petting Symbol and Button to enable Petting Session
    /// </summary>
    /// <param name="show"></param>
    public void SetPettingSymbol(bool show)
    {
        if (show)
        {
            EnableMenu((int)eMenus.PetMeSymbol);
            EnableMenu((int)eMenus.TapMonster);

            //TODO change to sad idle
        }
        else
        {
            DisableMenu((int)eMenus.TapMonster);
            DisableMenu((int)eMenus.PetMeSymbol);
        }
    }

    /// <summary>
    /// Called by CameraMovement: Controls enable/disable status of swipe buttons and controls
    /// </summary>
    public void SetSwipeButtonStatus()
    {
        if (GM.curMonsterSlot == ecurMonsterSlot.left)
        {
            DisableButton((int)eButtons.SwipeLeft);
            DisableMenu((int)eMenus.SwipeLeftInput);
        }
        else if (GM.curMonsterSlot == ecurMonsterSlot.right)
        {
            DisableButton((int)eButtons.SwipeRight);
            DisableMenu((int)eMenus.SwipeRightInput);
        }
        else
        {
            EnableButton((int)eButtons.SwipeLeft);
            EnableMenu((int)eMenus.SwipeLeftInput);

            EnableButton((int)eButtons.SwipeRight);
            EnableMenu((int)eMenus.SwipeRightInput);
        }
    }

    /// <summary>
    /// Disables/Enables all swiping controls and buttons using bool swipeEnabled
    /// </summary>
    public void DisableSwiping(bool disable)
    {
        swipeDisabled = disable;

        if (swipeDisabled)
        {
            DisabelAllSwiping();
        }
        else
        {
            EnableDefaultSwiping();            
        }
    }

    private void EnableDefaultSwiping()
    {
        EnableButton((int)eButtons.SwipeRight);
        EnableMenu((int)eMenus.SwipeRightInput);
        EnableButton((int)eButtons.SwipeLeft);
        EnableMenu((int)eMenus.SwipeLeftInput);
    }

    private void DisabelAllSwiping()
    {
        DisableButton((int)eButtons.SwipeRight);
        DisableMenu((int)eMenus.SwipeRightInput);
        DisableButton((int)eButtons.SwipeLeft);
        DisableMenu((int)eMenus.SwipeLeftInput);
    }

    // Changes UI menus according to scene 
    // Also camera position
    // Later including animations
    //TODO transfer to game manager when home ui screens are split up
    public void SetUIStage(eHomeUIScene newScene)
    {
        curScene = newScene;

        switch (curScene)
        {
            case eHomeUIScene.Home:
                UI_MonsterView();
                Camera.main.transform.position = GM.CurCamHomePos;

                break;
            case eHomeUIScene.Eggshop:
                UI_EggShop();
                GM.CurCamHomePos = Camera.main.transform.position;
                Camera.main.transform.position = camShopPos;

                break;
            case eHomeUIScene.Minigame:
                UI_Minigame();

                break;
            case eHomeUIScene.Dungeonlord:
                GM.CurCamHomePos = Camera.main.transform.position;
                //Camera.main.transform.position = camDungeonPos;
                UI_DungeonLord();

                break;
            case eHomeUIScene.Kompendium:
                UI_Kompendium();

                break;
            case eHomeUIScene.Petting:
                UI_Petting();

                break;
            case eHomeUIScene.none:
                NoUI();

                break;
            case eHomeUIScene.Cheats:
                UI_Cheats();
                break;
            case eHomeUIScene.Tutorial:
                UI_TutorialIntro();
                break;
            default:
                Debug.LogError("No UI scene set!");
                break;
        }
    }

    private void UI_TutorialIntro()
    {
        DisableMenu((int)eMenus.Home);
        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Petting);
        DisableMenu((int)eMenus.Kompendium);
        DisableMenu((int)eMenus.Dungeon);
        DisableMenu((int)eMenus.Shop);
        DisableMenu((int)eMenus.S_EggMenu);
        DisableMenu((int)eMenus.S_BottomButtons);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.D_MonsterStats);
        DisableMenu((int)eMenus.D_BottomButtons);
        DisableMenu((int)eMenus.MiniGameWindow);
        DisableMenu((int)eMenus.PlayerInfo);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.XPBar);
        DisableMenu((int)eMenus.SwipeButtons);

        EnableMenu((int)eMenus.TutorialIntro);
    }

    private void NoUI()
    {
        DisableMenu((int)eMenus.Home);
        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Petting);
        DisableMenu((int)eMenus.Kompendium);
        DisableMenu((int)eMenus.Dungeon);
        DisableMenu((int)eMenus.Shop);
        DisableMenu((int)eMenus.S_EggMenu);
        DisableMenu((int)eMenus.S_BottomButtons);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.D_MonsterStats);
        DisableMenu((int)eMenus.D_BottomButtons);
        DisableMenu((int)eMenus.MiniGameWindow);
        DisableMenu((int)eMenus.PlayerInfo);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.XPBar);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.TutorialIntro);
    }

    private void UI_Petting()
    {
        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Home);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.XPBar);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.PlayerInfo);
        DisableSwiping(true);


        TogglePetBackButton(true);
        EnableMenu((int)eMenus.Petting);
        EnableMenu((int)eMenus.PettingInfo);
        DisableMenu((int)eMenus.PettingXPBar);
        GM.petting = true;
    }

    private void UI_Kompendium()
    {
        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Home);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.XPBar);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.PlayerInfo);
        DisableSwiping(true);

        EnableMenu((int)eMenus.Kompendium);
    }

    private void UI_MonsterView()
    {
        for(int i = 0; i < 3; i++)
        {
            HomeBGs[i].sprite = BGsprites[i];
        }

        //scale monsters
        GM.homeMonsterManager.ScaleMonsterBody(0.15f);

        SetCageVisibility(false);
        EnableMenu((int)eMenus.H_MonsterStats);
        EnableMenu((int)eMenus.H_BottomButtons);
        EnableMenu((int)eMenus.SwipeButtons);
        EnableMenu((int)eMenus.PlayerInfo);
        EnableMenu((int)eMenus.Home);

        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Petting);
        DisableMenu((int)eMenus.Kompendium);
        DisableMenu((int)eMenus.Dungeon);
        //DisableMenu((int)eMenus.DungeonBG);
        DisableMenu((int)eMenus.D_BottomButtons);
        //DisableMenu((int)eMenus.ShopBG);
        DisableMenu((int)eMenus.Shop);
        DisableMenu((int)eMenus.S_EggMenu);
        DisableMenu((int)eMenus.S_BottomButtons);
        DisableMenu((int)eMenus.MiniGameWindow);
        DisableMenu((int)eMenus.D_MonsterStats);
        DisableMenu((int)eMenus.TutorialIntro);

        DisableSwiping(false);

        SetMonsterTexts();
        SetMonsterValue();
        SetGoldCounter();

        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null)
        {
            ShowMonsterStats(false);
        }
        else
        {
            ShowMonsterStats(true);            
        }
    }

    private void UI_EggShop()
    {
        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Home);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.XPBar);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.H_MonsterStats);
        //DisableMenu((int)eMenus.HomeBG);

        //EnableMenu((int)eMenus.ShopBG);
        EnableMenu((int)eMenus.Shop);
        EnableMenu((int)eMenus.S_ShopUI);
        EnableMenu((int)eMenus.S_EggMenu);
        EnableMenu((int)eMenus.S_BottomButtons);
        DisableSwiping(true);
    }

    private void UI_DungeonLord()
    {
        for (int i = 0; i < 3; i++)
        {
            HomeBGs[i].sprite = BGsprites[i+3];
        }

        SetCageVisibility(true);
        //scale monsters
        GM.homeMonsterManager.ScaleMonsterBody(0.125f);

        //DisableMenu((int)eMenus.HomeBG);
        //DisableMenu((int)eMenus.XPBar);
        DisableMenu((int)eMenus.CheatWindow);
        DisableMenu((int)eMenus.Home);
        DisableMenu((int)eMenus.H_MonsterStats);
        DisableMenu((int)eMenus.D_SalesContract);        
        EnableMenu((int)eMenus.D_MonsterStats);

        //EnableMenu((int)eMenus.DungeonBG);
        EnableMenu((int)eMenus.Dungeon);
        EnableMenu((int)eMenus.D_BottomButtons);
        EnableMenu((int)eMenus.SwipeButtons);
    }

    private void UI_Cheats()
    {
        DisableMenu((int)eMenus.Home);
        EnableMenu((int)eMenus.CheatWindow);
    }

    private void UI_Minigame()
    {
        DisableMenu((int)eMenus.Home);
        EnableMenu((int)eMenus.MiniGameWindow);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableSwiping(true);
    }

    public void ShowPetSessionResult()
    { 
        DisableMenu((int)eMenus.PettingInfo);
        EnableMenu((int)eMenus.PettingXPBar);
    }

// sets monster slot symbols if slot is locked or empty
    public void SetSlotSymbol()
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null && !GM.CurMonsters[(int)GM.curMonsterSlot].Unlocked)
        {
            EnableMenu((int)eMenus.LockedButton);
            DisableMenu((int)eMenus.AddButton);
        }
        else if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null && GM.CurMonsters[(int)GM.curMonsterSlot].Unlocked)
        {
            EnableMenu((int)eMenus.AddButton);
            DisableMenu((int)eMenus.LockedButton);
        }
        else
        {
            DisableMenu((int)eMenus.LockedButton);
            DisableMenu((int)eMenus.AddButton);
        }
    }

    public void TrainTimerClicked()
    {
        SetPopInfoWindowStatus(true, "This monster is tired!\n Try again when the timer has run out.");
        DisableSwiping(true);
        EnablePopupInfoCloseButton();
    }

    public void ShowMonsterStats(bool show)
    {
        SetMonsterTexts();
        SetMonsterValue();
        SetMonsterXPBarUndLevel();
        SetMonsterRarityStars();

        if (show)
        {
            EnableMenu((int)eMenus.H_MonsterStats);
            if (!GM.CurMonsters[(int)GM.curMonsterSlot].IsHappy)
            {
                EnableMenu((int)eMenus.PetMeSymbol);
                EnableMenu((int)eMenus.TapMonster);
            }
            else
            {
                DisableMenu((int)eMenus.PetMeSymbol);
                DisableMenu((int)eMenus.TapMonster);
            }
        }
        else
        {
            DisableMenu((int)eMenus.H_MonsterStats);
            DisableMenu((int)eMenus.PetMeSymbol);
            DisableMenu((int)eMenus.TapMonster);
        }
    }

    public void AddMonster()
    {
        GoToEggShop();
    }

    public void EnablePopupInfoCloseButton()
    {
        EnableMenu((int)eMenus.ClosePopupButton);        
    }

    public void SetPopInfoWindowStatus(bool open, string message = "")
    {
        if (open)
        {
            EnableMenu((int)eMenus.PopupInfoWindow);
            SetText((int)eTextfields.PopupInfoText, message);
        }
        else
        {
            DisableMenu((int)eMenus.PopupInfoWindow);
        }
    }

    // opens the lock-info-window if player pressed on padlock symbol
    public void PressedPadlockSymbol()
    {
        DisableSwiping(true);
        DisableMenu((int)eMenus.H_BottomButtons);
        //EnableMenu((int)eMenus.PopupInfoWindow);
        if (GM.PlayerMoney >= GM.CurMonsters[(int)GM.curMonsterSlot].UnlockPrice)
        {
            SetPopInfoWindowStatus(true, 
                "This slot is locked." +
                "\nUnlock it for " + GM.CurMonsters[(int)GM.curMonsterSlot].UnlockPrice + " Money?");
            EnableMenu((int)eMenus.UnlockYNButtons);
        }
        else
        {
            SetPopInfoWindowStatus(true, 
                "This slot is locked." +
                "\nUnlocking costs" + GM.CurMonsters[(int)GM.curMonsterSlot].UnlockPrice + " Money.");
            EnableMenu((int)eMenus.ClosePopupButton);            
        }
    }

    // Unlocks a slot after player pressed yes
    public void UnlockSlot()
    {
        GM.ChangePlayerGold(-GM.CurMonsters[(int)GM.curMonsterSlot].UnlockPrice);
        GM.CurMonsters[(int)GM.curMonsterSlot].Unlocked = true;
        SetSlotSymbol();
        CloseUnlockInfo();
    }

    // If player doesnt want or cant unlock the slot
    public void CloseUnlockInfo()
    {
        DisableMenu((int)eMenus.ClosePopupButton);
        DisableMenu((int)eMenus.UnlockYNButtons);
        EnableMenu((int)eMenus.H_BottomButtons);
        DisableMenu((int)eMenus.PopupInfoWindow);
        DisableSwiping(false);
    }

    // Updating Monster and Player Values 
    public void SetMonsterValue()
    {     
        int curValue = GM.homeMonsterManager.CalculateMonsterValue(); 

        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null)
        {            
            GM.homeUI.SetText((int)eTextfields.H_MonsterValue, "");
            GM.homeUI.SetText((int)eTextfields.D_MonsterValue, "");
        }
        else if (GM.CurMonsters[(int)GM.curMonsterSlot].Sold)
        {
            GM.homeUI.SetText((int)eTextfields.D_MonsterValue, "Sold for " + curValue);
        }
        else
        {
            GM.homeUI.SetText((int)eTextfields.H_MonsterValue, "Value: " + curValue);
            GM.homeUI.SetText((int)eTextfields.D_MonsterValue, "Value: " + curValue);
            //GM.homeUI.SetText((int)eTextfields.MonsterLevel, "" + GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel);
            //print("Value: " + GM.CurMonsters[(int)GM.curMonsterSlot].MonsterValue);
        }
    }

    public void SetMonsterTexts()
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null)
        {          
            GM.homeUI.SetText((int)eTextfields.H_MonsterTypeandStage, "");
            GM.homeUI.SetText((int)eTextfields.D_MonsterTypeandStage, "No monster \navailable");
        }
        else
        {           
            SetText((int)eTextfields.H_MonsterTypeandStage, GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage + " " + GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MonsterName);
            SetText((int)eTextfields.D_MonsterTypeandStage, GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage + " " + GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MonsterName);
            SetMonsterValue();
            SetMonsterXPBarUndLevel();
        }
    }

    public void SetMonsterXPBarUndLevel()
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null || GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage >= eMonsterStage.Egg)
        {
            DisableMenu((int)eMenus.XPBar);
            GM.homeUI.SetText((int)eTextfields.H_MonsterLevel, "");
        }
        else if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster != null && GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage != eMonsterStage.Egg)
        {
            EnableMenu((int)eMenus.XPBar);
            SetText((int)eTextfields.H_MonsterLevel, "Lvl " + GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel);
            SetXPBars();
        }
        else
        {
            DisableMenu((int)eMenus.XPBar);
            SetText((int)eTextfields.H_MonsterLevel, "Lvl " + GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel);
            SetXPBars();
        }
    }    

    public void SetMonsterRarityStars()
    {
        switch (GM.CurMonsters[(int)GM.curMonsterSlot].Rarity)
        {
            case eRarity.normal:
                RarityStars[0].SetActive(true);
                RarityStars[1].SetActive(false);
                RarityStars[2].SetActive(false);
                break;
            case eRarity.rare:
                RarityStars[0].SetActive(true);
                RarityStars[1].SetActive(true);
                RarityStars[2].SetActive(false);

                break;
            case eRarity.legendary:
                RarityStars[0].SetActive(true);
                RarityStars[1].SetActive(true);
                RarityStars[2].SetActive(true);

                break;
            default:
                RarityStars[0].SetActive(false);
                RarityStars[1].SetActive(false);
                RarityStars[2].SetActive(false);

                break;
        }
    }

    public void SetMonsterlevel_Dungeon()
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null)
        {
            GM.homeUI.SetText((int)eTextfields.D_MonsterLevel, "");
        }
        else
        {
            SetText((int)eTextfields.D_MonsterLevel, "Lvl " + GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel);
        }
    }

    public void SetGoldCounter()
    {
        //PlayerGold.SetTrigger("gain");
        SetText((int)eTextfields.GoldCount, "" + GM.PlayerMoney);
    }

    /// <summary>
    /// Sets the dungeonlords dialogue depending on the offered monsters stage
    /// </summary>
    public void SetDungeonDialogue()
    {
        string textline;

        if (GM.CurMonsters[(int)GM.curMonsterSlot].Sold)
        {
            textline =
            "Can't wait to put this one to work.";
        }
        else
        {
            switch (GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage)
            {
                case eMonsterStage.Baby:
                    textline =
                        "Eh, canon fodder.";
                    break;
                case eMonsterStage.Teen:
                    textline =
                        "Moody lil guy.";
                    break;
                case eMonsterStage.Adult:
                    textline =
                        "Ah, a useful one!";
                    break;
                case eMonsterStage.Egg:
                    textline =
                        "I'm not buying this!";
                    break;
                case eMonsterStage.none:
                    textline =
                        "Got anything for me?";
                    break;
                default:
                    textline =
                        "Eh?!";
                    break;
            }
        }
        SetText((int)eTextfields.DungeonDialogue, textline);
    }

    //Button Inputs for menu changes

    public void GoToKompendium()
    {
        GM.monsterKompendium.SetButtons();
        GM.monsterKompendium.ShowEntry(0);
        SetUIStage(eHomeUIScene.Kompendium);
        //TODO set info to be the fist monster in list
    }

    public void GoToPetSession()
    {
        StartCoroutine(cWaitForZoom(false));
    }

    public void ExitPetSession()
    {
        GM.petting = false;
        StartCoroutine(cWaitForZoom(true));
    }

    /// <summary>
    /// Waiting for the zoom from Petting session
    /// </summary>
    /// <param name="zoomedIn"></param>
    /// <returns></returns>
    private IEnumerator cWaitForZoom(bool zoomedIn)
    {
        SetUIStage(eHomeUIScene.none);  

        yield return new WaitForSeconds(0.15f);
        if (zoomedIn)
        {
            
            if (GM.CurMonsters[GM.curMonsterID].IsHappy)
                GM.SetPetTimer(GM.curMonsterID);
            GM.monsterTimer.CheckDateTimes();
            camMovement.Zoom(false);
            SetUIStage(eHomeUIScene.Home);
        }
        else
        {
            SetUIStage(eHomeUIScene.Petting);            
        }
    }

    public void GoToHome()
    {
        SetUIStage(eHomeUIScene.Home);
    }

    public void GoToEggShop()
    {
        SetUIStage(eHomeUIScene.Eggshop);
        SetText((int)eTextfields.ShopDialogue, 
            "Click the egg you want to buy!");
    }

    public void GoToMinigames()
    {
        SetUIStage(eHomeUIScene.Minigame);
    }

    public void GoToDungeonlord()
    {
        if (GM.DLIsGone)
        {
            print("DL is not available right now");
            //TODO show info, that he will be available later?
        }
        else
        {
            SetUIStage(eHomeUIScene.Dungeonlord);
            SetDungeonDialogue();

            GM.CancelAllNotifs();
            GM.homeUI.SetMonsterTexts();
            GM.homeUI.SetMonsterValue();
            GM.homeUI.SetMonsterlevel_Dungeon();

            if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null)
            {
                GM.homeUI.SellButtonActive(false);
            }
            else
            {
                GM.homeUI.SellButtonActive(true);
            }
        }
    }

    public void ExitKompendium()
    {
        SetUIStage(eHomeUIScene.Home);
    }

    public void ExitMinigameMenu()
    {
        SetUIStage(eHomeUIScene.Home);
    }

    public void ExitEggShop()
    {
        SetUIStage(eHomeUIScene.Home);
    }

    //Button Inputs EggShop
    public void ChooseEgg(Monster thisMonster)
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].Monster != null && GM.PlayerMoney >= thisMonster.BaseCost)
        {
            SetText((int)eTextfields.ShopDialogue, 
                "There is already a monster in this spot!");
        }
        else if (GM.PlayerMoney < thisMonster.BaseCost)
        {
            SetText((int)eTextfields.ShopDialogue, 
                "It seems you cannot afford this egg!");
        }
        else if (thisMonster == null)
        {
            SetText((int)eTextfields.ShopDialogue,
                "Sorry, I don't have this kind of egg right now!");
        }
        else
        {
            if (GM.CurMonsters[(int)GM.curMonsterSlot].Unlocked)
            {
                curEgg = thisMonster;
                SetText((int)eTextfields.ShopDialogue, 
                    "You really wanna buy this egg?");
                // Make Button "selected"
                //DisableMenu(Menus[(int)eMenus.S_EggMenu]);
                DisableMenu((int)eMenus.S_BottomButtons);
                EnableMenu((int)eMenus.S_PurchaseConfirm);
            }
            else
            {
                SetText((int)eTextfields.ShopDialogue, 
                    "You haven't unlocked this spot yet!");
            }
        }
    }

    public void ConfirmEggPurchase(bool isTrue)
    {
        if (isTrue)
        {
            //put egg in empty slot position
            GM.CurMonsters[(int)GM.curMonsterSlot].Monster = curEgg;
            GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage = eMonsterStage.Egg;
                       
            SetText((int)eTextfields.H_MonsterTypeandStage, 
                GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage + " " + 
                GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MonsterName);
            SetText((int)eTextfields.D_MonsterTypeandStage, 
                GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage + " " + 
                GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MonsterName);

            GM.CurMonsters[(int)GM.curMonsterSlot].BaseValue = GM.CurMonsters[(int)GM.curMonsterSlot].Monster.BaseValue;
            GM.CurMonsters[(int)GM.curMonsterSlot].XPCap = GM.CurMonsters[(int)GM.curMonsterSlot].Monster.XP_Cap;
            SetMonsterTexts();
            SetMonsterValue();
            GM.homeUI.SetSlotSymbol();
            DisableMenu((int)eMenus.S_EggMenu);
            DisableMenu((int)eMenus.S_BottomButtons);
            SetText((int)eTextfields.ShopDialogue, 
                "Alright, one egg to go!");
            GM.ChangePlayerGold(-GM.CurMonsters[(int)GM.curMonsterSlot].Monster.BaseCost);

            StartCoroutine(GM.homeMonsterManager.cSpawnEgg());
            StartCoroutine(cStartEggHatchMinigame());            
        }
        else
        {
            StartCoroutine(cCancelEggPurchas());
        }
        DisableMenu((int)eMenus.S_PurchaseConfirm);
    }

    public IEnumerator cStartEggHatchMinigame()
    {
        //wait until egg is spawned
        yield return new WaitForSeconds(0.35f);
        //SetText((int)eTextfields.ShopDialogue, 
        //    "Tap egg to hatch it!");
        DisableMenu((int)eMenus.S_ShopUI);
        DisableMenu((int)eMenus.S_EggMenu);
        DisableMenu((int)eMenus.S_BottomButtons);
        DisableMenu((int)eMenus.PlayerInfo);
        EnableMenu((int)eMenus.S_TappEggButton);
    }

    /// <summary>
    /// Deactivates and greys out the TrainButton
    /// </summary>
    /// <param name="isActive"></param>
    public void TrainButtonActive(bool isActive)
    {
        if (isActive)
            EnableButton((int)eButtons.H_Train);
        else
            DisableButton((int)eButtons.H_Train);
    }

    public void EnableEggGlow(bool enable)
    {
        if (enable)
        {
            EnableMenu((int)eMenus.EggGlow);
        }
        else
        {
            DisableMenu((int)eMenus.EggGlow);
        }
    }

    public void TapEgg()
    {
        hatchTaps += 1;

        if (hatchTaps == EggHatchCount)
        {
            GM.CurMonsters[GM.curMonsterID].PetTimerEnd = System.DateTime.Now;
            GM.CurMonsters[GM.curMonsterID].PlayTimerEnd = System.DateTime.Now;
            GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel = 1;            
            GM.homeMonsterManager.SetEggRarity();
            StartCoroutine(GM.homeMonsterManager.cHatchEgg(GM.homeMonsterManager.BabySpawn));
            DisableMenu((int)eMenus.S_TappEggButton);
            GM.monsterKompendium.MonsterEntry[CurKompendiumEntrySlot()].MonsterHatchCount += 1;
            SetMonsterTexts();
            SetMonsterValue();
            SetMonsterXPBarUndLevel();
            SetSlotSymbol();
            hatchTaps = 0;
        }
        else
        {
            StartCoroutine(GM.HomeCam.cShake(0.05f, 0.1f));
            if (hatchTaps < 3)
                GM.homeMonsterManager.CrackEgg(hatchTaps);
            GM.vfx_home.SpawnEffect(VFX_Home.VFX.TapEgg, VFX_Home.Position.EggHatching);
        }
    }

    public IEnumerator cCancelEggPurchas()
    {
        EnableMenu((int)eMenus.S_BottomButtons);
        SetText((int)eTextfields.ShopDialogue, 
            "Can I get you another one?");
        yield return new WaitForSeconds(0.5f);
    }

    //Button Inputs extra menus in home (Minigame, Pet, Feed?)
    public void ChooseRunner()
    {
        EnableMenu((int)eMenus.RunnerLevelChoice);
    }

    public void ChooseRunnerLevel(int scene)
    {
        GM.CancelAllNotifs();
        GM.CurMonsters[GM.curMonsterID].IsTired = true;
        GM.curScreen = eScene.runner;
        SceneManager.LoadScene(scene);
    }

    public void ExitLevelChoice()
    {
        DisableMenu((int)eMenus.RunnerLevelChoice);
    }

    public void FeedButtonPressed()
    {
        print("Feeding time");
    }

    /// <summary>
    /// Set Sell button active if there is a monster to sell in the current slot
    /// </summary>
    /// <param name="isActive"></param>
    public void SellButtonActive(bool isActive)
    {
        if (isActive)
        {
            EnableMenu((int)eMenus.DungeonSellButton);
        }
        else
        {
            DisableMenu((int)eMenus.DungeonSellButton);
        }
    }

    //Button Inputs Dungeonlord    

   /// <summary>
   /// Player pressed Sell monster Button in Dungeon screen
   /// </summary>
    public void SellMonsterButton()
    {
        //disable swipe buttons
        DisableSwiping(true);
        DisableMenu((int)eMenus.SwipeButtons);
        DisableMenu((int)eMenus.D_BottomButtons);
        //enable y/n menu        
        SetPopInfoWindowStatus(true, 
            "Are you sure you want to sell this monster?");
        EnableMenu((int)eMenus.D_SaleConfirm);
        SellButtonActive(false);
    }

    /// <summary>
    /// If an adult monster is sold, the entry in the kompendium is unlocked/updated
    /// </summary>
    private void WriteValuesToKompendium()
    {
        int entrySlot = CurKompendiumEntrySlot();
        GM.UnlockedLogEntries[entrySlot] = true;
        int newPrice = Mathf.RoundToInt(GM.CurMonsters[(int)GM.curMonsterSlot].MonsterValue);

        if (GM.monsterKompendium.MonsterEntry[entrySlot].MonsterHighestPrice < newPrice)
            GM.monsterKompendium.MonsterEntry[entrySlot].MonsterHighestPrice = newPrice;
    }

    public int CurKompendiumEntrySlot()
    {
        int entrySlot = (int)GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MonsterType;

        switch (GM.CurMonsters[(int)GM.curMonsterSlot].Rarity)
        {
            case eRarity.normal:

                break;
            case eRarity.rare:
                entrySlot += 1;
                break;
            case eRarity.legendary:
                entrySlot += 2;
                break;
            default:
                break;
        }

        return entrySlot;
    }

    /// <summary>
    /// Player confirms/cancels sale of monster in dungeon screen
    /// </summary>
    public void ConfirmMonsterSale(bool yes)
    {
        DisableMenu((int)eMenus.D_SaleConfirm);

        if (yes)
        {
            GM.CancelNotification(GM.curMonsterID);


            GM.DLIsGone = true;
            GM.CurMonsters[(int)GM.curMonsterSlot].Sold = true;
            if (GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage == eMonsterStage.Adult)
            {
                WriteValuesToKompendium();
            }
            SetText((int)eTextfields.DungeonDialogue, "Sign and it's mine!");
            ShowContract();
        }
        else        
        {
            //enable swipe buttons
            DisableSwiping(false);
            EnableMenu((int)eMenus.SwipeButtons);
            EnableMenu((int)eMenus.D_BottomButtons);
            EnableMenu((int)eMenus.DungeonSellButton);
            //close dis y/n shit
        }

        DisableMenu((int)eMenus.D_SaleConfirm);
        SetPopInfoWindowStatus(false);
    }

    /// <summary>
    /// Cage is put over monsters to show they are sold
    /// </summary>
    private IEnumerator cMonsterCage()
    {
        SellButtonActive(false);
        SetText((int)eTextfields.DungeonDialogue, "Mine.");

        SetText((int)eTextfields.D_MonsterValue,
            "Sold for " + Mathf.RoundToInt(GM.CurMonsters[(int)GM.curMonsterSlot].MonsterValue));

        yield return new WaitForSeconds(0.2f);
        DropCage(GM.CurMonsters[(int)GM.curMonsterSlot].SlotID);
        yield return new WaitForSeconds(0.2f);
        GM.homeMonsterManager.monsterAnim[GM.curMonsterID].SetBool("isSad", true);
        yield return new WaitForSeconds(0.3f);
        DisableMenu((int)eMenus.D_SaleConfirm);
        EnableMenu((int)eMenus.D_BottomButtons);
        EnableMenu((int)eMenus.SwipeButtons);  
        DisableSwiping(false);        
    }
    private void ShowContract()
    {        
        string msg = GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage +" "
            + GM.CurMonsters[(int)GM.curMonsterSlot].Monster.MonsterName + "\n"
            + "Selling Price: " + Mathf.RoundToInt(GM.CurMonsters[(int)GM.curMonsterSlot].MonsterValue);

        totalValue = Mathf.RoundToInt(GM.CurMonsters[(int)GM.curMonsterSlot].MonsterValue);
        SetText((int)eTextfields.D_SoldMonster1, msg);
        EnableMenu((int)eMenus.D_SalesContract);
    }

    private void SetCageVisibility(bool isVisible)
    {
        AllCages.SetActive(isVisible);        
    }

    private void DropCage(int slotID)
    {
        CageTop[slotID].SetTrigger("falling");
        CageTop[slotID].SetBool("down", true);
    }

    /// <summary>
    /// Player pressed Go Home button in Dungeonlord menu
    /// </summary>
    public void ExitDungeonMenu()
    {
        // nothing has been sold to the dungeon lord
        if (!GM.DLIsGone)
        {
            GM.RestartNotifs();
            SetUIStage(eHomeUIScene.Home);
        }
        else
        {        
            StartCoroutine(cExitDungeon());
        }
    }

    private IEnumerator cExitDungeon()
    {
        SetText((int)eTextfields.DungeonDialogue, "I'm gonna be busy, come back later.");
        yield return new WaitForSeconds(2f);
        foreach (MonsterSlot slot in GM.CurMonsters)
        {
            if (slot.Sold)
            {
                GM.homeMonsterManager.DeleteMonsterBody(slot.SlotID);
                slot.ResetValues();
            }
        }
        SetSlotSymbol();
        GM.RestartNotifs();
        GM.SetDLTimer();        
        GM.monsterTimer.CheckDateTimes();
        SetUIStage(eHomeUIScene.Home);
    }

    /// <summary>
    /// Player taps the contract to "sign"
    /// </summary>
    public void SignContract()
    {
        if (!signed)
        {
            SetText((int)eTextfields.DungeonDialogue, "It's a deal!");
            StartCoroutine(cSignMonsterSaleContract());
        }
    }

    /// <summary>
    /// spawns coins while moneycountfinished is false
    /// </summary>
    /// <param name="timeperCoin"></param>
    /// <returns></returns>
    private IEnumerator cSpawnCoins(int coinsToSpawn, float timeperCoin)
    {
        int coins = 0;
        while (coins < coinsToSpawn && !moneyCountFinished)
        {
            coins += 1;
            int rand = Random.Range(0, CoinsPos.Length);
            GameObject newCoin = Instantiate(CoinPrefab, CoinSpawn);
            newCoin.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newCoin.GetComponent<RectTransform>().anchoredPosition = CoinsPos[rand].anchoredPosition;
            yield return null;
        }
        print(coins+" coins spawned");
        moneyCountFinished = true;
    }

    public IEnumerator cSetGoldCounter(int oldValue)
    {
        float timeperCoin = 0f;

        if (totalValue <= 500)
            timeperCoin = 0.005f;
        else if (totalValue <= 1000)
            timeperCoin = 0.0025f;
        else
            timeperCoin = 0.00125f;
      
        StartCoroutine(cSpawnCoins(Mathf.RoundToInt(totalValue), timeperCoin));
        //print("new coins: " + diff);

        yield return new WaitForSeconds(0.76f);

        while (GM.PlayerMoney >= oldValue && !goldCountSkip)
        {
            SetText((int)eTextfields.GoldCount, "" + oldValue);            
            oldValue++;
            yield return null;
        }   
    }

    /// <summary>
    /// Called by SignContract()
    /// </summary>
    /// <returns></returns>
    public IEnumerator cSignMonsterSaleContract()
    {
        signed = true;
        D_Signature.SetTrigger("sign");
        yield return new WaitForSeconds(1.0f);

        moneyCountFinished = false;
        GM.ChangePlayerGold(+totalValue, GM.PlayerMoney);       
        
        EnableMenu((int)eMenus.SkipGoldButton);
       
        while (!moneyCountFinished)
            yield return null;

        yield return new WaitForSeconds(2.5f);
        signed = false;
        D_Signature.SetTrigger("done");
        yield return new WaitForSeconds(0.01f);
        DisableMenu((int)eMenus.D_SalesContract);
        DisableMenu((int)eMenus.SkipGoldButton);
        StartCoroutine(cMonsterCage());
        goldCountSkip = false;
    }

    public void SkipGoldCount()
    {
        GoldSkipTapped += 1;

        if (GoldSkipTapped == 2)
        {
            DisableMenu((int)eMenus.SkipGoldButton);
            moneyCountFinished = true;
            goldCountSkip = true;
            GoldSkipTapped = 0;
            GM.ChangePlayerGold(0);
        }
    }

    public void WatchAdButton()
    {
        //GM.CurMonsters[GM.curMonsterID].IsHappy;

       print("watch an ad and get money");
        //SpawnCoins();
    }

    /// <summary>
    /// This is for testing bullshit; spawns a monster atm
    /// </summary>
    /// <param name="monster"></param>
    public void TestButton(Monster monster)
    {
        if (GM.CurMonsters[(int)GM.curMonsterSlot].Unlocked && GM.CurMonsters[(int)GM.curMonsterSlot].Monster == null)
        {
            GM.CurMonsters[(int)GM.curMonsterSlot].Monster = monster;
            GM.CurMonsters[(int)GM.curMonsterSlot].MonsterLevel = 1;
            GM.CurMonsters[(int)GM.curMonsterSlot].GoldModificator = GM.CurMonsters[(int)GM.curMonsterSlot].Monster.GoldModificator[(int)GM.CurMonsters[(int)GM.curMonsterSlot].Rarity];
            GM.homeMonsterManager.SetEggRarity();
            GM.CurMonsters[(int)GM.curMonsterSlot].BaseValue = GM.CurMonsters[(int)GM.curMonsterSlot].Monster.BaseValue;
            GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage = eMonsterStage.Baby;
            SetMonsterTexts();
            SetMonsterValue();
            SetMonsterXPBarUndLevel();
            SetSlotSymbol();
            GM.homeMonsterManager.SpawnCurrentMonster(GM.homeMonsterManager.MonsterSpawn[(int)GM.curMonsterSlot]);
            TrainButtonActive(true);
        }
    }
    public void AnotherCheat()
    {
        GM.CurMonsters[(int)GM.curMonsterSlot].MonsterStage = eMonsterStage.Adult;
        GM.homeMonsterManager.DeleteMonsterBody(GM.CurMonsters[(int)GM.curMonsterSlot].SlotID);
        GM.homeMonsterManager.SpawnCurrentMonster(GM.homeMonsterManager.MonsterSpawn[(int)GM.curMonsterSlot]);
    }
}
