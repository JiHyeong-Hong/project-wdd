using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// ±×¹°¸Á Å¬·¡½º. @È«ÁöÇü
public class Net : Monster
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        CreatureType = ECreatureType.Monster;
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature == null || creature.CreatureType != Define.ECreatureType.Hero)
            return;
        
        target.OnDamaged(this, null);
    }

}