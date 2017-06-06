﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIScript : MonoBehaviour
{

    public CanvasGroup inventory;
    public CanvasGroup infoPanel;

    private Animator animator;
    private string button;
    private GameObject[] temp;
    private SlotScript script;
    private Text infoPanelText;
    private PlantGround groundScript;
    private GameObject parent;
    private Image infoPanelSprite;

    void Awake()
    {
        infoPanel = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<CanvasGroup>();
        infoPanelText = infoPanel.GetComponentInChildren<Text>();
        infoPanelSprite = infoPanel.GetComponentInChildren<Image>();
        temp = GameObject.FindGameObjectsWithTag("Slot");
    }

    // Use this for initialization
    void Start()
    {
        foreach (GameObject scripts in temp)
        {
            script = scripts.GetComponent<SlotScript>();
        }
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //IF HELVETTI.. KORJAA!!!
        if (Input.touchSupported == true || Input.GetMouseButton(0) == true)
        {
            if (ColliderHandler.hitDetected == true)
            {
                parent = ColliderHandler.parentGameObject;
                groundScript = parent.GetComponent<PlantGround>();

                if (ColliderHandler.parentGameObject.tag == "NotPlanted")
                {
                    inventory.alpha = 1;
                    animator.SetBool("showInventory", true);
                }
                else if(ColliderHandler.parentGameObject.tag == "Planted")
                {
                    infoPanel.alpha = 1;
                    initializeInfoPanel(groundScript.plantName);
                }
            }
        }
    }

    public void initializeInfoPanel(string name)
    {
        infoPanelText.text = name + " is growing!";
        infoPanelSprite.sprite = Resources.Load<Sprite>("" + name);
    }

    public void ButtonClicked()
    {
        button = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(button);
        switch (button)
        {
            case "Exit":
                infoPanel.alpha = 0;
                animator.SetBool("showInventory", false);
                break;
            case "Plant":
                script.GetComponent<SlotScript>().Plant();
                if (SlotScript.didPlant)
                {
                    animator.SetBool("showInventory", false);
                }
                else
                    return;
                break;
        }
    }
}
