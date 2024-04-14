using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StormFeatherSkill : SkillBase
{
    private List<Monster> monsterList = new List<Monster>();
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
        lastPos = target.transform.position;

        Vector3 dir = (lastPos - Owner.transform.position).normalized;

        for (int i = 0; i < SkillData.CastCount; i++)
        {
            Peacock peacock = Managers.Object.Spawn<Peacock>(Owner.transform.position, SkillData.ProjectileNum);

            peacock.SetTarget(target);
            peacock.SetLastPos(lastPos);

            peacock.SetSpawnInfo(Owner, this, dir);
            //isNull? Util.GetRandomDir() :

            await UniTask.WaitForSeconds(0.2f);
        }
    }



    public override void Clear()
    {
    }
}
