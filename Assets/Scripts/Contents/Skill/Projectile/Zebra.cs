using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 얼룩말 발사체 클래스. @홍지형
public class Zebra : Projectile
{
    private CapsuleCollider2D _collider;
    
    float boxWidth = 5f; // 가상의 네모칸 너비
    float boxHeight = 9f; // 가상의 네모칸 높이
    float moveDuration = 3f; // 이동하는 데 걸리는 시간

    private Vector3 lastPlayerPosition;
    Vector2 centerPosition; 
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Hero의 중심위치를 찾는다.
        centerPosition = Managers.Object.Hero.CenterPosition;
        
       
        return true;
    }

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        base.SetSpawnInfo(owner, skill, direction);

        _collider = GetComponent<CapsuleCollider2D>();
        _collider.enabled = true;

        int minus = (direction.x >= 0) ? 1 : -1;

        if (Mathf.Abs(direction.x) < 0.001f && direction.y < 0)
            minus = -1;
        if (Mathf.Abs(direction.y) < 0.001f && direction.x < 0)
            minus = -1;

        LookLeft = (minus == 1);

        // 얼룩말 GameObject의 부모를 플레이어 GameObject로 설정한다.
        if (Managers.Object.Hero.gameObject != null)
        {
            transform.SetParent(Managers.Object.Hero.gameObject.transform, true);
        }

        // MoveZebra();
    }

    // 애니메이션, 이동
    private void MoveZebra()
    {        
        //Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);
        Vector2 topLeft = centerPosition + new Vector2(-boxWidth / 2, boxHeight / 2);
        //Vector2 bottomRight = centerPosition + new Vector2(boxWidth / 2, -boxHeight / 2);
        Vector2 bottomLeft = centerPosition + new Vector2(-boxWidth / 2, -boxHeight / 2);

        // 얼룩말 동작.
        Sequence sequence = DOTween.Sequence()
               .Append(Renderer.DOFade(1f, 0f))
                  // .Append(transform.DOLocalMove(bottomLeft, moveDuration)).SetEase(Ease.Linear)               
                  // .Append(transform.mo(bottomLeft, moveDuration)).SetEase(Ease.Linear)               
                  // .Append(transform.DOMove(centerPosition, moveDuration)).SetEase(Ease.Linear)               
               .Append(transform.DOMove(topLeft, moveDuration).SetEase(Ease.Linear)) // 왼쪽 위로 이동
               .Append(transform.DOMove(bottomLeft, moveDuration).SetEase(Ease.Linear)) // 왼쪽 아래로 이동
               .AppendCallback(() =>
               {
                   // _collider.enabled = false;
                   Animator.SetInteger("state", 4);
               })
               .InsertCallback(moveDuration, () =>
               {
                   Managers.Object.Despawn(this);
               });
        sequence.Restart();
    }

    private void Update()
    {
        //Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);
        Vector2 topLeft = centerPosition + new Vector2(-boxWidth / 2, boxHeight / 2);
        //Vector2 bottomRight = centerPosition + new Vector2(boxWidth / 2, -boxHeight / 2);
        Vector2 bottomLeft = centerPosition + new Vector2(-boxWidth / 2, -boxHeight / 2);

        transform.rotation = Quaternion.identity;

        if (Managers.Object.Hero.transform.position != lastPlayerPosition)
        {
            // 플레이어의 위치가 변경을때만 동작
            // MoveZebra(); 
            transform.DOMove(topLeft, moveDuration).SetEase(Ease.Linear);
            transform.DOMove(bottomLeft, moveDuration).SetEase(Ease.Linear);
            lastPlayerPosition = Managers.Object.Hero.transform.position; // 위치 업데이트
        }
    }

    // 충돌 시 이벤트 트리거
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            monster.OnDamaged(Owner, Skill);
        }
    }

    // 디버깅용.
    void OnDrawGizmos()
    {
        Vector2 centerPosition = Managers.Object.Hero.CenterPosition;

        Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);
        Vector2 topLeft = centerPosition + new Vector2(-boxWidth / 2, boxHeight / 2);
        Vector2 bottomRight = centerPosition + new Vector2(boxWidth / 2, -boxHeight / 2);
        Vector2 bottomLeft = centerPosition + new Vector2(-boxWidth / 2, -boxHeight / 2);

        Gizmos.color = Color.red;

        // 네모칸의 경계 그리기
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(topRight, bottomLeft);
    }

}
