using Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// �渮���Ͱ� ��� ������ ��ų Ŭ����. TODO: �߻�ü ��Ʈ �߰� �ʿ���. @ȫ����
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
        var _target = Managers.Object.Hero; // Hero�� Ÿ������ ��´�.

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