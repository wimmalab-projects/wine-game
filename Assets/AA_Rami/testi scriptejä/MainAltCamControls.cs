﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set transition to true to move camera.
/// transition is accessible outside the script as
/// its public state might suggest
/// </summary>
public class MainAltCamControls : MonoBehaviour {

    public Camera Camera; // drag in editor
    public Transform CamPos1, CamPos2; // drag in editor, i used two empty game objects as placeholders from where i can just take position and lerp x axis.
    private bool transition = false, cam1 = false;

    public void Update()
    {
        if (transition == true)
        {
            ChangePos();
        }
    }

    /// <summary>
    /// Lerp to campos1 or campos2 depending on cam1 boolean
    /// </summary>
    public void ChangePos ()
    {
        if (cam1 != true)
        {
            Camera.transform.position = new Vector3(Mathf.Lerp(Camera.transform.position.x, CamPos1.position.x, 0.3f), Camera.transform.position.y, Camera.transform.position.z);

            if ((int)Camera.transform.position.x <= (int)CamPos1.transform.position.x) // lessthan or equal here because lerp doesn't get to exact x value at lower time values
            {
                transition = false;
                cam1 = true;
            }
        }
        else
        {
            Camera.transform.position = new Vector3(Mathf.Lerp(Camera.transform.position.x, CamPos2.position.x, 0.3f), Camera.transform.position.y, Camera.transform.position.z);

            if ((int)Camera.transform.position.x + 1 >= (int)CamPos2.transform.position.x) // plus 1 here because the larp doesn't quite seem to get to equal value at slower time values.
            {
                transition = false;
                cam1 = false;
            }
        }
    }
}
