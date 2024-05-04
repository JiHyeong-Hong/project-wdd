using UnityEngine;

// �渮���Ͱ� ��� �����Ѿ� �߻�ü Ŭ����. TODO: �߻�ü ��Ʈ �߰� �ʿ���. @ȫ����
public class StunBullet : Projectile
{
    private Hero _target;    
    private Vector2 _targetPosition;    // �߻�� �� ��ǥ ��ġ�� ����
    private float _initialAngle;        // �ʱ� ������ ����
                                        
    private BoxCollider2D col;
    private float moveSpeed = 5f;
    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (col == null)
            col = GetComponent<BoxCollider2D>();

        Renderer = GetComponentInChildren<SpriteRenderer>();
        isInfinityDuration = true;

        return true;
    }

    public void SetTarget(Hero target)
    {
        this._target = target;
        if (target != null)
        {
            // ��ǥ���� ��ġ, �߻�ü�� �ٶ󺸴� ���� ����
            _targetPosition = target.transform.position;
            var dir = _targetPosition - (Vector2)transform.position;
            _initialAngle = Util.VectorToAngle(dir);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _initialAngle));           
        }
    }

    protected override void Move()
    {
        if (_target != null)
        {            
            // ��ǥ ��ġ�� ���� �������� �̵��Ѵ�.
            var position = Vector2.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            transform.SetPositionAndRotation(position, Quaternion.Euler(new Vector3(0f, 0f, _initialAngle)));

            // ��ǥ ��ġ�� �������� ���
            if((Vector2)transform.position == _targetPosition)
            {
                Managers.Object.Despawn(this); // �߻�ü ����.
            }
        }
    }

    // ��ǥ���� �浹���� ���
    private void OnTriggerEnter2D(Collider2D col)
    {        
        if (((1 << (int)Define.ELayer.Hero) & (1 << col.gameObject.layer)) != 0)
        {
            col.GetComponent<Hero>().OnDamaged(Managers.Object.Hero, Skill);
            Managers.Object.Despawn(this);
        }
    }
}
