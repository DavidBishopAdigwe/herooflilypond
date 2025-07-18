using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
using PickableItems;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Base class for all player scripts that interact with objects
/// </summary>
public abstract class Interactor : MonoBehaviour
{
    protected virtual void Start()
    {
        InputReader.Instance.InteractPerformed += OnInteractKeyClicked;
    }



    protected abstract void OnInteractKeyClicked(InputAction.CallbackContext obj);
    

    protected virtual void OnDestroy()
    {
        InputReader.Instance.InteractPerformed -= OnInteractKeyClicked;
    }







}


