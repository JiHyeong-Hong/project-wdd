using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : BaseObject
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Spawner;

        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(int ID)
    {
        Managers.Object.Spawn<Monster>(transform.position, ID);
    }
}
