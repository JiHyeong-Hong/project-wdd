using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Kunai : Projectile
{
    private bool isCollided = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Creature creature = target as Creature;
        if (creature.CreatureType != Define.ECreatureType.Monster)
            return;
        if (isCollided)
            return;

        creature.OnDamaged(Owner, Skill);

        Managers.Object.Despawn(this);
        isCollided = true;
    }
}
