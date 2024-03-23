using Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// 길리슈터가 쏘는 마취총 스킬 클래스. TODO: 발사체 아트 추가 필요함. @홍지형
public class StunBulletSkill : SkillBase
{
    private StunBullet _bullet;
    public StunBullet StunBullet { get { return _bullet; } }

    public override void DoSkill()
    {        
        GetTargets();
    }

    private void GetTargets()
    {
        var _target = Managers.Object.Hero; // Hero를 타겟으로 삼는다.

        if (Util.CheckTargetInScreen(_target.transform.position) == false)
        {
            Debug.Log("[StunBulletSkill] Hero not found!");
            return;
        }

        StunBullet stunBullet = Managers.Object.Spawn<StunBullet>(Owner.transform.position, 1);

        stunBullet.SetTarget(_target);
        stunBullet.SetSpawnInfo(Owner, this, Vector2.zero);
    }

    public override void Clear()
    {

    }
}