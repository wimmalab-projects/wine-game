﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIScript : MonoBehaviour
{

    public CanvasGroup inventory;
    public CanvasGroup infoPanel;
    public Text infoPanelText;
    public Text infoPanelTimer;
    public Button harvestButton;
    public Button plantButton;

    private Animator animator;
    private string button;
    private GameObject[] temp;
    private SlotScript slotScript;
    private PlantGround groundScript;
    private GameObject parent;
    private Image infoPanelSprite;
    private string timer;
    private GameObject gameManager;
    private FermentorScript fermentorScript;
    private GameMaster gameMaster;

    // Use this for initialization

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameMaster = gameManager.GetComponent<GameMaster>();
        infoPanelSprite = infoPanel.transform.Find("Plant sprite").GetComponent<Image>();
        temp = GameObject.FindGameObjectsWithTag("Slot");
        slotScript = gameManager.GetComponent<SlotScript>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (infoPanel.alpha == 1)
        {
            if(parent.tag == "Planted")
            {
                timer = groundScript.niceTime;
                infoPanelTimer.text = timer;
                if (groundScript.Timer <= 0)
                {
                    infoPanelTimer.text = "Ready!";
                }
            }
            else if(parent.tag == "Fermenting")
            {
                timer = fermentorScript.niceTime;
                infoPanelTimer.text = timer;
                if (fermentorScript.Timer <= 0)
                {
                    infoPanelTimer.text = "Ready!";
                }
            }
        }
    }

    public void initializeInfoPanel(string name)
    {
        if(parent.tag == "Planted")
        {
            infoPanelText.text = name + " is growing!";
            infoPanelSprite.sprite = Resources.Load<Sprite>("" + name);
        }

        else if(parent.tag == "Fermenting")
        {
            infoPanelText.text = name + " is fermenting";
            infoPanelSprite.sprite = Resources.Load<Sprite>("" + name);
        }
    }

    public void ButtonClicked()
    {
        if (gameManager.GetComponent<GameMaster>().IsInventoryOpen == true)
        {
            button = EventSystem.current.currentSelectedGameObject.name;
            switch (button)
            {
                case "Exit":
                    infoPanel.alpha = 0;
                    animator.SetBool("showInventory", false);
                    break;
                case "Plant":
                    slotScript.Plant();
                    if (SlotScript.didPlant)
                    {
                        animator.SetBool("showInventory", false);
                    }
                    else
                        return;
                    break;
                case "Harvest":
                    if (groundScript.Timer <= 0)
                    {
                        slotScript.Harvest();
                        infoPanel.alpha = 0;
                    }
                    else
                        return;
                    break;
                case "Crush":
                    slotScript.selectGrape();
                    if (SlotScript.didPlant)
                    {
                        animator.SetBool("showInventory", false);
                    }
                    else
                        return;
                    break;
                case "Collect":
                    if (fermentorScript.Timer <= 0)
                    {
                        slotScript.Collect();
                        infoPanel.alpha = 0;
                    }
                    else
                        return;
                    break;
            }
            plantButton.name = "Plant";
            plantButton.GetComponentInChildren<Text>().text = "Plant";
            harvestButton.name = "Harvest";
            harvestButton.GetComponentInChildren<Text>().text = "Harvest";
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameMaster>().IsInventoryOpen = false;
        }
    }

    public void showInventory()
    {
        parent = ColliderHandler.parentGameObject;
        groundScript = parent.GetComponent<PlantGround>();
        fermentorScript = parent.GetComponent<FermentorScript>();

        switch (parent.tag)
        {
            case "NotPlanted":
                inventory.alpha = 1;
                animator.SetBool("showInventory", true);
                break;
            case "Planted":
                infoPanel.alpha = 1;
                initializeInfoPanel(groundScript.plantName);
                break;
            case "NotFermenting":
                inventory.alpha = 1;
                animator.SetBool("showInventory", true);
                plantButton.name = "Crush";
                plantButton.GetComponentInChildren<Text>().text = "Crush";
                break;
            case "Fermenting":
                infoPanel.alpha = 1;
                initializeInfoPanel(gameMaster.GetDescription(fermentorScript.WineType));
                harvestButton.name = "Collect";
                harvestButton.GetComponentInChildren<Text>().text = "Collect";
                break;
        }
    }
}
