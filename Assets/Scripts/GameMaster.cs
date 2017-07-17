﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [Header("Camera System")]
    // Cameras for different views // Drag in editor
    public GameObject TownCamera;
    public GameObject FarmCamera;
    public GameObject BreweryCamera;
    public GameObject CrushCamera;

    //[Header("Grape Crush")]
    //public Camera GrapeCrush; // Drag in editor
    //public GameObject PlayGrapeCrushGO; // Drag in editor

    [Header("XP system")]
    public Text exp;
    public Text expneeded;
    public Text level;
    public Text playername;

    // invisible in inspector
    private GUIScript guiScript;
    private CurtainControls curtainControls;
    public bool CrushisActive;
    public bool WineSold { get; set; }
    // set and compare to this
    public GameState State { get; set; }

    // set and reset when inventory opens closes
    // this is to stop colliders activating on mouse / touch clicks trough ui
    public bool IsInventoryOpen;

    /// <summary>
    /// List our gamestates here
    /// set default state and inventory state
    /// to prevent clicking trough ui
    /// </summary>
    public enum GameState
    {
        Farm,
        Town,
        Brewery,
        GrapeCrush
    };

    public enum PlantState
    {
        NotPlanted,
        JustPlanted,
        Overgrowth,
        Trimmed,
        Grapes
    };

    public enum FermentationState
    {
        NotFermentating,
        Fermenting,
        Fermented
    }

    public enum ClarificationState
    {
        NotClarificating,
        Clarificating,
        Clarificated
    }

    public enum Winetype
    {
        [Description("White wine")]
        WhiteWine,
        [Description("Rose wine")]
        RoseWine,
        [Description("Red wine")]
        RedWine,
    }

    public enum BottlingState
    {
        NotBottling,
        Bottling,
        Bottled
    }

    void Awake()
    {
        curtainControls = GameObject.FindGameObjectWithTag("Curtain").GetComponent<CurtainControls>();
        guiScript = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<GUIScript>();
    }

    private void Start()
    {
        State = GameState.Farm;
        //CrushCamera.transform.parent.gameObject.SetActive(false);
        CrushisActive = false;
        WineSold = false;
        IsInventoryOpen = false;
    }

    void GoToTown()
    {
        curtainControls.FadeToBlack(Camera.main, TownCamera.transform.Find("MainCam").GetComponent<Camera>());
        State = GameState.Town;
    }
    void GoToFarm()
    {
        curtainControls.FadeToBlack(Camera.main, FarmCamera.transform.Find("MainCam").GetComponent<Camera>());
        State = GameState.Farm;
    }
    void GoToBrewery()
    {
        curtainControls.FadeToBlack(Camera.main, BreweryCamera.transform.Find("MainCam").GetComponent<Camera>());
        State = GameState.Brewery;
    }

    void PlayGrapeCrush()
    {
        State = GameState.GrapeCrush;
        CrushCamera.transform.parent.gameObject.SetActive(true);
        curtainControls.FadeToBlack(Camera.main, CrushCamera.transform.Find("MainCam").GetComponent<Camera>());
        CrushisActive = true;
    }

    public void ViewInventory(GameObject go)
    {
        Debug.Log("teststetst");
        guiScript.showInventory();
        IsInventoryOpen = true;
    }

    // Get fermentation state enums description for nicer name
    public string GetDescription(Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        DescriptionAttribute attribute
                = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    as DescriptionAttribute;

        return attribute == null ? value.ToString() : attribute.Description;
    }

    #region CustomerSystem
    [Header("Customer System")]
    public GameObject CustomerSystem; // set in editor.
    public GameObject CurrentClient; // set by cpFarmButton.cs load this into farmview client

    void ClientClick(GameObject go)
    {
        if (go.GetComponent<Client>().AtFarm == false)
        { 
            CustomerSystem.GetComponent<CustomerPanel>().clientSender = go.GetComponent<Client>();
            CustomerSystem.GetComponent<CustomerPanel>().ActivatePanel();
            CustomerSystem.GetComponent<CustomerPanel>().ScaleExplanationContent();
        }
    }
    public enum ClientType
    {
        Farmer,
        Restaurant
    };

    void ResetGame()
    {
        // Asiakkaat
        // Tehty viini
    }

    // load farm client to farm.
    public void LoadFarmClient()
    {
        CurrentClient.GetComponent<Client>().AtFarm = true;
        GameObject go = Instantiate(CurrentClient);
        go.transform.position = GameObject.Find("CustomerSpawn").transform.position;
        go.layer = GameObject.Find("CustomerSpawn").layer;
    }

    #endregion

    #region CameraRailsSystemMethods

    public void FarmCameraTransition()
    {
        FarmCamera.GetComponent<MainAltCamControls>().transition = true;
    }

    public void TownCameraTransition()
    {
        TownCamera.GetComponent<MainAltCamControls>().transition = true;
    }

    public void BreweryCameraTransition()
    {
        BreweryCamera.GetComponent<MainAltCamControls>().transition = true;
    }

    public void CrushCameraTransition()
    {
        CrushCamera.GetComponent<MainAltCamControls>().transition = true;
    }

    #endregion

    #region wine compare
    [Header("Wine Comparison System")]
    public GameObject OurWine;

    /// <summary>
    /// Compare two wines and return the number of matches
    /// </summary>
    /// <param name="wine1"></param>
    /// <param name="wine2"></param>
    /// <returns></returns>
    public int CompareWines(Wine wineA, Wine wineB)
    {
        bool match = false;

        //wineA.ComparisonMatrix = wineA.CreateMatrix(); // make sure comparison wine matrix is not null
        //wineB.ComparisonMatrix = wineB.CreateMatrix();

        int similiarities = 0;

        // new comparsion loop wihtout LINQ (LINQ reportedly doesn't work properly with mobile)
        for (int i = 0; i < wineA.ComparisonMatrix.Count; i++)
        {
            for (int x = 0; x < wineB.ComparisonMatrix.Count; x++)
            {

                if (wineA.ComparisonMatrix[i].Length != wineB.ComparisonMatrix[x].Length)
                    break;

                for (int y = 0; y < wineA.ComparisonMatrix[i].Length; y++)
                {
                    if (wineA.ComparisonMatrix[i][y] != wineB.ComparisonMatrix[x][y])
                    {
                        match = false;
                        break;
                    }
                    else
                    {
                        match = true;
                    }
                }

                if (match == true)
                {
                    similiarities++;
                    match = false;
                }
            }
        }

        return similiarities;
    }

    #endregion

    #region shop

    /// <summary>
    /// call this from methodcaller handler to open close shop.
    /// </summary>
    public void Shop()
    {
        Debug.Log(transform.parent);
        if (transform.GetComponent<Shop>().shop.activeSelf == false)
        {
            transform.GetComponent<Shop>().OpenShop();
        }
        else
        {
            transform.GetComponent<Shop>().CloseShop();
        }
    }

    #endregion
}

