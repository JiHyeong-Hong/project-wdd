using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSkill : SkillBase
{
	private readonly float DISTANCE_OFFSET = 3f;
	private readonly float ANGLE_OFFSET = 70f;
    
    public override void DoSkill()
    {
	    var hero = Managers.Object.Hero;
	    var dir = Owner.Direction;
	    var angle = hero.Pivot.eulerAngles.z;
	    bool isFlip = dir.x < 0;
	    
	    Bear bear = Managers.Object.Spawn<Bear>((Vector2)Owner.transform.position + Owner.Direction * DISTANCE_OFFSET, SkillData.ProjectileId);
	    
	    bear.transform.localScale = isFlip ? new Vector3(-1, 1, 1) : Vector3.one;
	    bear.transform.eulerAngles = new Vector3(0, 0, isFlip ? angle - ANGLE_OFFSET : angle + ANGLE_OFFSET );
	    
	    // test.position = (Vector2)transform.position + _moveDir * 3f;
	    // bool isFlip = _moveDir.x < 0;
	    // test.localScale = isFlip ? new Vector3(-1, 1, 1) : Vector3.one;
	    // test.eulerAngles = new Vector3(0, 0, isFlip ? Pivot.eulerAngles.z - 70 : Pivot.eulerAngles.z + 70 );
    }
    
    
    public override void Clear()
    {
        
    }
}
