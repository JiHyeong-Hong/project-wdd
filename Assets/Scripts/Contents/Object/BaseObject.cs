using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public CircleCollider2D Collider { get; protected set; }
    public Animator Animator { get; protected set; }
    public Rigidbody2D RigidBody { get; private set; }
    public SpriteRenderer Renderer { get; protected set; }

    public float ColliderRadius { get { return Collider != null ? Collider.radius : 0.0f; } }
    public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

    public int DataTemplateID { get; set; }

    bool _lookLeft = true;
    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            Flip(!value);
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = gameObject.GetComponent<CircleCollider2D>();
        Animator = gameObject.GetComponent<Animator>();
        RigidBody = gameObject.GetComponent<Rigidbody2D>();
        Renderer = gameObject.GetComponent<SpriteRenderer>();

        return true;
    }

    #region Battle
    public virtual void OnDamaged(BaseObject attacker, SkillBase skill)
    {

    }

    public virtual void OnDead(BaseObject attacker, SkillBase skill)
    {

    }
    #endregion

    #region Animation
    protected virtual void PlayAnimation(Define.ECreatureState state)
    {

    }

    protected void Flip(bool flag)
    {
        if (Renderer == null)
            return;

        Renderer.flipX = flag;
    }
    #endregion
}
