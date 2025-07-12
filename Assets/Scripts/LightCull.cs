using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[RequireComponent(typeof(Light2D))]
public class LightCull: MonoBehaviour
{
    [SerializeField] private LayerMask[] affectedLayers;
    
    private Light2D _light;

    private void Awake()
    {
        int[] masks = new int[affectedLayers.Length];
        for (int i = 0; i < affectedLayers.Length; i++)
        {
            masks[i] = affectedLayers[i];
        }
        _light.SetLayers(masks);
    }
} 

