using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ��踻 �߻�ü Ŭ����. @ȫ����
public class Zebra : Projectile
{
    private CapsuleCollider2D _collider;
    
    float boxWidth = 5f; // ������ �׸�ĭ �ʺ�
    float boxHeight = 9f; // ������ �׸�ĭ ����
    float moveDuration = 3f; // �̵��ϴ� �� �ɸ��� �ð�

    private Vector3 lastPlayerPosition;
    Vector2 centerPosition; 
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Hero�� �߽���ġ�� ã�´�.
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

        // ��踻 GameObject�� �θ� �÷��̾� GameObject�� �����Ѵ�.
        if (Managers.Object.Hero.gameObject != null)
        {
            transform.SetParent(Managers.Object.Hero.gameObject.transform, true);
        }

        // MoveZebra();
    }

    // �ִϸ��̼�, �̵�
    private void MoveZebra()
    {        
        //Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);
        Vector2 topLeft = centerPosition + new Vector2(-boxWidth / 2, boxHeight / 2);
        //Vector2 bottomRight = centerPosition + new Vector2(boxWidth / 2, -boxHeight / 2);
        Vector2 bottomLeft = centerPosition + new Vector2(-boxWidth / 2, -boxHeight / 2);

        // ��踻 ����.
        Sequence sequence = DOTween.Sequence()
               .Append(Renderer.DOFade(1f, 0f))
                  // .Append(transform.DOLocalMove(bottomLeft, moveDuration)).SetEase(Ease.Linear)               
                  // .Append(transform.mo(bottomLeft, moveDuration)).SetEase(Ease.Linear)               
                  // .Append(transform.DOMove(centerPosition, moveDuration)).SetEase(Ease.Linear)               
               .Append(transform.DOMove(topLeft, moveDuration).SetEase(Ease.Linear)) // ���� ���� �̵�
               .Append(transform.DOMove(bottomLeft, moveDuration).SetEase(Ease.Linear)) // ���� �Ʒ��� �̵�
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
            // �÷��̾��� ��ġ�� ���������� ����
            // MoveZebra(); 
            transform.DOMove(topLeft, moveDuration).SetEase(Ease.Linear);
            transform.DOMove(bottomLeft, moveDuration).SetEase(Ease.Linear);
            lastPlayerPosition = Managers.Object.Hero.transform.position; // ��ġ ������Ʈ
        }
    }

    // �浹 �� �̺�Ʈ Ʈ����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << other.gameObject.layer)) != 0)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            monster.OnDamaged(Owner, Skill);
        }
    }

    // ������.
    void OnDrawGizmos()
    {
        Vector2 centerPosition = Managers.Object.Hero.CenterPosition;

        Vector2 topRight = centerPosition + new Vector2(boxWidth / 2, boxHeight / 2);
        Vector2 topLeft = centerPosition + new Vector2(-boxWidth / 2, boxHeight / 2);
        Vector2 bottomRight = centerPosition + new Vector2(boxWidth / 2, -boxHeight / 2);
        Vector2 bottomLeft = centerPosition + new Vector2(-boxWidth / 2, -boxHeight / 2);

        Gizmos.color = Color.red;

        // �׸�ĭ�� ��� �׸���
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(topRight, bottomLeft);
    }

}
