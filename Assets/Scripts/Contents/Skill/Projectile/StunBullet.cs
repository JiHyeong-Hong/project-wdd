using UnityEngine;

// 길리슈터가 쏘는 마취총알 발사체 클래스. TODO: 발사체 아트 추가 필요함. @홍지형
public class StunBullet : Projectile
{
    private Hero _target;    
    private Vector2 _targetPosition;    // 발사될 때 목표 위치를 저장
    private float _initialAngle;        // 초기 방향을 저장
                                        
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
            // 목표물의 위치, 발사체가 바라보는 각도 설정
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
            // 목표 위치를 향해 직선으로 이동한다.
            var position = Vector2.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            transform.SetPositionAndRotation(position, Quaternion.Euler(new Vector3(0f, 0f, _initialAngle)));

            // 목표 위치에 도달했을 경우
            if((Vector2)transform.position == _targetPosition)
            {
                Managers.Object.Despawn(this); // 발사체 삭제.
            }
        }
    }

    // 목표물과 충돌했을 경우
    private void OnTriggerEnter2D(Collider2D col)
    {        
        if (((1 << (int)Define.ELayer.Hero) & (1 << col.gameObject.layer)) != 0)
        {
            col.GetComponent<Hero>().OnDamaged(Managers.Object.Hero, Skill);
            Managers.Object.Despawn(this);
        }
    }
}
