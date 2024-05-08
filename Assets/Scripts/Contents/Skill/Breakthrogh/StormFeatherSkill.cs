using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StormFeatherSkill : SkillBase
{
    private List<Monster> monsterList = new List<Monster>();
    private GameObject peacockEffect;
    public override void DoSkill()
    {
        DoSkillAsync().Forget();
    }

    private void GetTargets()
    {
        var list = Managers.Object.Monsters;
        monsterList.Clear();

        foreach (var monster in list)
        {
            if (monster.Hp <= 0)
                continue;

            if (Util.CheckTargetInScreen(monster.transform.position))
            {
                monsterList.Add(monster);
            }
        }


    }

    public async UniTask DoSkillAsync()
    {
        GetTargets();

        bool isNull = monsterList.Count == 0;

        int idx = Random.Range(0, monsterList.Count);
        Monster target = !isNull ? monsterList[idx] : null;

        Vector3 lastPos;
        Vector3 dir;
        if (target != null)
        {
            lastPos = target.transform.position;
            dir = (lastPos - Owner.transform.position).normalized;
        }
        else
        {
            dir = Owner.Direction;
        }
        PeacockEffectFindSetActive(true, 0.1f * SkillData.CastCount);

        for (int i = 0; i < SkillData.CastCount; i++)
        {
            Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, 1);

            peacock.SetTarget(target);

            peacock.SetSpawnInfo(Owner, this, dir, true);

            await UniTask.WaitForSeconds(0.1f);
        }
    }

    private void PeacockEffectFindSetActive(bool active, float time)
    {
        Hero hero = Managers.Object.Hero;

        // 이미 피콕 효과가 존재하는지 확인
        peacockEffect = hero.gameObject.transform.Find("PeacockEffect")?.gameObject;

        if (peacockEffect == null)
        {
            peacockEffect = new GameObject("PeacockEffect");
            peacockEffect.transform.SetParent(hero.transform);
            peacockEffect.AddComponent<SpriteRenderer>().sprite = Managers.Resource.Load<Sprite>("Art/PeacockEffect");
        }
        peacockEffect.transform.localPosition = Vector3.zero;

        BreakthroughHelper.Instance.SetActiveObject(peacockEffect, active, time);
    }

    public override void Clear()
    {
    }
}
