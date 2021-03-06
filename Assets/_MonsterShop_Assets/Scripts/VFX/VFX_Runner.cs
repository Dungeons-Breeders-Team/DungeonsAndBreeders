﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Runner : MonoBehaviour
{

    [Header("List of Positions to spawn the VFX")]
    public GameObject[] SpawnPosition = new GameObject[3];
    public enum Position         //One enum for each Position, needs to be the same order
    {
        //ResultLevelUp,
        //ResultGrowthUp,        

        //NumberofPositions needs to be the last in list!
        NumberofPositions
    }

    [Header("List of VFX prefabs")]
    public GameObject[] VFXEffect = new GameObject[4];
    public enum VFX             //One enum name for each prefab, needs to be the same order
    {
        Orb_Pickup,
        Runner_Death,
        Runner_Run,
        Confetti,

        //NumberofVFX needs to be the last in list!
        NumberofVFX
    }

    private void Start()
    {
        GameManager.Instance.vfx_runner = this;        

        if (VFXEffect.Length != (int)VFX.NumberofVFX)
        {
            Debug.LogError("Not enough VFX assigned");
        }
        if (SpawnPosition.Length != (int)Position.NumberofPositions)
        {
            Debug.LogError("Not enough Transforms assigned");
        }
    }

    public void SpawnEffektAtPosition(VFX effect, Vector3 position)
    {
        GameObject newVFX = GameObject.Instantiate(this.VFXEffect[(int)effect], transform.position, transform.rotation) as GameObject;
        newVFX.name = "" + effect;
        newVFX.transform.position = position;
        //newVFX.transform.SetParent(SpawnPosition[(int)position].transform);
    }

    public void SpawnEffektAtObject(VFX effect, GameObject parent)
    {
        GameObject newVFX = GameObject.Instantiate(this.VFXEffect[(int)effect], transform.position, transform.rotation) as GameObject;
        newVFX.name = "" + effect;
        newVFX.transform.position = parent.transform.position;
        newVFX.transform.SetParent(parent.transform);
    }

    /// <summary>
    /// Spawn effekt using the enum lists
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="position"></param>
    public void SpawnEffect(VFX effect, Position position)
    {
        GameObject newVFX = GameObject.Instantiate(this.VFXEffect[(int)effect], transform.position, transform.rotation) as GameObject;
        newVFX.name = "" + effect;
        newVFX.transform.position = SpawnPosition[(int)position].transform.position;
        newVFX.transform.SetParent(SpawnPosition[(int)position].transform);
        //print("Spawned VFX " + newVFX.name + " under " + SpawnPosition[(int)position].name);
    }

    /// <summary>
    /// Spawn effekt via Button etc where enums cannot be used
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="position"></param>
    public void SpawnEffectViaInt(VFX effect, int position)
    {
        GameObject newVFX = GameObject.Instantiate(this.VFXEffect[(int)effect], transform.position, transform.rotation) as GameObject;
        newVFX.name = "" + effect;
        newVFX.transform.position = SpawnPosition[(int)position].transform.position;
        newVFX.transform.SetParent(SpawnPosition[(int)position].transform);
        //print("Spawned VFX " + newVFX.name + " under " + SpawnPosition[(int)position].name);
    }
}
