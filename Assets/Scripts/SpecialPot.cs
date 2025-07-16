
using Managers;
using PickableItems;
using UnityEngine;

public class PickablePot: PickableItem
{
    public override void Pickup()
    {
        GameManager.Instance.WinGame();
        base.Pickup();
    }
} 

