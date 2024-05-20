using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : BaseObject
{
    public GameObject damageTextPrefab; // ������ �ؽ�Ʈ ������

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
        Monster monster = Managers.Object.Spawn<Monster>(transform.position, ID);
        if (monster != null)
        {
            monster.SetDamageTextPrefab(damageTextPrefab);
            Debug.Log("SetDamageTextPrefab called, damageTextPrefab: " + damageTextPrefab);

            // �߰����� Ȯ��
            if (monster.GetComponent<Monster>().damageTextPrefab == null)
            {
                Debug.LogError("Failed to set damageTextPrefab");
            }
        }
        else
        {
            Debug.LogError("Failed to spawn monster");
        }
    }
}
