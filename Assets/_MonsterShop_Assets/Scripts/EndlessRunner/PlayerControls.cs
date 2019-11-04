﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public RunnerController controller;
    public Camera Cam;
    public Rigidbody Monster;
    private float verticalSpeed { get { return controller.VerticalSpeed; } }
    private float horizontalSpeed { get { return controller.HorizontalSpeed; } }

    [SerializeField] private bool pointerdown;
    [SerializeField] private float horizontalDirection;

    public void Start()
    {
        //newPosX = Monster.transform.position.x;
    }
    private void Update()
    {
        Cam.transform.position = new Vector3(0f, Monster.transform.position.y + 5.0f, -10f);

        if (!pointerdown)
        {
            Monster.velocity = new Vector2(0f, verticalSpeed);
        }
        else
        {
            Monster.velocity = new Vector2(horizontalSpeed * horizontalDirection, verticalSpeed);
        }
    }

    public void OnPointerDown(bool left)
    {
        pointerdown = true;

        if (left)
            horizontalDirection = -1;
        else
            horizontalDirection = +1;
    }

    public void OnPointerUp()
    {
        pointerdown = false;
    }
}