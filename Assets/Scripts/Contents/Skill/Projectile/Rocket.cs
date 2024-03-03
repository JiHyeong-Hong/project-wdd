using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : Projectile
{
    private bool isCollided = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            if (isCollided)
                return;

            List<Monster> monsters = Managers.Object.FindMonsterByRange(transform.position, 1.2f);
            foreach (Monster monster in monsters)
            {
                monster.OnDamaged(Owner, Skill);
            }
            Managers.Resource.Instantiate("Area/Circle").GetOrAddComponent<Area>().SetInfo(transform.position, 0.5f);

            Managers.Object.Despawn(this);
            isCollided = true;
        }
    }
}
