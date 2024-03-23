using System;
using UnityEngine;

public class EquipmentSlot : Slot
{
    public EquipmentType Type;
    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = Type.ToString() + " Slot";
    }
}
