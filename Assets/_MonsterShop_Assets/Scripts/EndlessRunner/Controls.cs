﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    public int CollectedCount;
    public Text CollectedText;
    public Camera Cam;
    public Rigidbody Monster;
    public float verticalSpeed = 1f;
    public float horizontalSpeed = 1f;

    enum Controlscheme { followfinger, pressarrow };
    Controlscheme controlscheme = Controlscheme.followfinger;
    private float newPosX;

    public void Start()
    {
        newPosX = Monster.transform.position.x;
    }
    private void Update()
    {
        switch (controlscheme)
        {
            case Controlscheme.followfinger:
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    newPosX = Cam.ScreenToWorldPoint(touch.position).x;
                }

                if (!Mathf.Approximately(Monster.transform.position.x, newPosX))
                {
                    if (Monster.transform.position.x > newPosX)
                    {
                        //left
                        Monster.velocity = new Vector2(-horizontalSpeed, verticalSpeed);
                    }
                    else if (Monster.transform.position.x < newPosX)
                    {
                        //right
                        Monster.velocity = new Vector2(horizontalSpeed, verticalSpeed);
                    }
                }
                else
                {
                    Monster.velocity = new Vector2(0f, verticalSpeed);
                }

                break;
            case Controlscheme.pressarrow:
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    newPosX = Cam.ScreenToWorldPoint(touch.position).x;
                }

                if (!Mathf.Approximately(Monster.transform.position.x, newPosX))
                {
                    if (Monster.transform.position.x > newPosX)
                    {
                        //left
                        Monster.velocity = new Vector2(-horizontalSpeed, verticalSpeed);
                    }
                    else if (Monster.transform.position.x < newPosX)
                    {
                        //right
                        Monster.velocity = new Vector2(horizontalSpeed, verticalSpeed);
                    }
                }
                else
                {
                    Monster.velocity = new Vector2(0f, verticalSpeed);
                }
                break;
            default:
                break;
        }       

        Cam.transform.position = new Vector3(0f, Monster.transform.position.y + 5.0f, -10f);
    }

    public void UpdateCollectedCount(int value)
    {
        CollectedCount += value;
        CollectedText.text = ""+CollectedCount;
    }

    public void ChangeControls(int control)
    {
        controlscheme = (Controlscheme)control;
        print("controlscheme name = " + controlscheme);
    }
}