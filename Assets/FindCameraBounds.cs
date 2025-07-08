using System;
using UnityEngine;

public class FindCameraBounds : MonoBehaviour
{
   private void Awake()
   {
      FindObjectsByType<CameraBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
   }
}
