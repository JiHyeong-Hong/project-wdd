using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magnet : Item
{
    private float usedTransparency = 0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ItemType = Define.EItemType.Magnet;

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

        if (Renderer != null)
        {
            Color color = Renderer.material.color;
            color.a = usedTransparency;
            Renderer.material.color = color;
        }

        
        SoundManager.Instance.Play(Define.ESoundMainType.Item, Define.ESoundType.Item);
        // ���� �ִ� ��� ����ġ ���� ã��
        Exp[] exps = FindObjectsOfType<Exp>();
        
        foreach (Exp exp in exps)
        {
            if (exp != null)
            {
                exp.StartCoroutine(Move(exp));
            }
        }

        Managers.Object.Despawn(this);
    }
}