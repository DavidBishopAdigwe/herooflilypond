
using Singletons;
using PickableItems;
using UnityEngine;

public class FinalPotion: PickableItem
{
    public override void Pickup()
    {
        base.Pickup();
        GameManager.Instance.WinGame();
    }
} 

