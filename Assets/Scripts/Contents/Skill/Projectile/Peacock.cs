using UnityEngine;

public class Peacock : Projectile
{
	[SerializeField]
	private Monster target;
	[SerializeField]
	private Vector3 dir;
	private CircleCollider2D col;

    public override bool Init()
	{
		if (!base.Init())
			return false;
		
		if (col == null)
			col = GetComponent<CircleCollider2D>();
		
		Renderer = GetComponentInChildren<SpriteRenderer>();
		SoundManager.Instance.Play(Define.ESoundMainType.Skill1, Define.ESoundType.Peacock);
		isInfinityDuration = true;
		
		return true;
	}

	public void SetTarget(Monster target)
	{
		Boss boss = FindObjectOfType<Boss>();

		if (boss != null)
		{
			this.target = boss;
		}
		else
		{
			this.target = target;
		}

		col.enabled = this.target == null;
	}

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        base.SetSpawnInfo(owner, skill, direction);
		SetTarget(null);
    }

	public void SetSpawnInfo (Creature owner, SkillBase skill, Vector2 direction, bool isBTSkill)
	{
        if (isBTSkill)
        {
            Renderer.sprite = Managers.Resource.Load<Sprite>("Art/PeacockFeather(skil)");
        }

        SetSpawnInfo(owner, skill, direction);
    }

    protected override void Move()
	{
		if (target != null)
		{
			dir = target.transform.position - transform.position;
			var position = Vector2.MoveTowards(transform.position,
				target.transform.position, Skill.SkillData.AttackSpeed * Time.deltaTime);

			float angle = Util.VectorToAngle(dir);
			transform.SetPositionAndRotation(position,Quaternion.Euler(new Vector3(0f, 0f, angle)));
			
			if (Vector2.SqrMagnitude(dir) <= Mathf.Pow(0.5f,2))
			{
				target.OnDamaged(Owner,Skill);
				Managers.Object.Despawn(this);
			}
		}
		else
		{
            transform.Translate(Vector3.up * (Skill.SkillData.AttackSpeed * Time.deltaTime));
		}

		if(!Util.CheckTargetInScreen(transform.position))
			Managers.Object.Despawn(this);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
		{
			col.GetComponent<Monster>().OnDamaged(Owner,Skill);
			Managers.Object.Despawn(this);
		}
	}
}