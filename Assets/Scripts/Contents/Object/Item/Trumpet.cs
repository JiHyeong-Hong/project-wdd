using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trumpet : BaseObject
{
    public override bool Init()
    {
        if (!base.Init())
            return false;

        //ObjectType = Define.EObjectType.Item;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseObject target = other.GetComponent<BaseObject>();
        if (target.IsValid() == false)
            return;

        Hero hero = target as Hero;
        if (hero == null)
            return;

        // È¹µæ½Ã ÀÏ¾î³ª´Â ÀÏµé
        // ¾ó·è¸» ÃâÇö - È÷Æ® - ¼Ò¸ê

        Managers.Object.Despawn(this);
    }
}
