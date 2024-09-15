using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : BaseObject
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Structure;

        return true;
    }

    // Manager reset 시 삭제.
    public void ResetDatas()
    {
        Managers.Object.Despawn(this);
    }
}
