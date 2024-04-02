using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFootboard : MonoBehaviour
{
    private Hero hero;

    public float jumpDistance = 6f; 
    public float jumpHeight = 2f; 
    public float jumpDuration = 1.5f; 

    public LayerMask enemyLayer; 
    public float blowAwayRadius = 6f; // �� ���� �ݰ�

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        hero = FindObjectOfType<Hero>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Jump(hero.MoveDir));
        }
    }

    private IEnumerator Jump(Vector2 direction)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f); 
        }

        hero.IsInvincible = true;

        Vector3 start = hero.transform.position;
        Vector3 target = start + new Vector3(direction.x, direction.y, 0) * jumpDistance;
        float elapsed = 0;
        float rotationAmount = 0f;

        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            float heightMultiplier = Mathf.Sin(Mathf.PI * t); 
            hero.transform.position = Vector3.Lerp(start, target, t) + Vector3.up * heightMultiplier * jumpHeight;

            // ȸ�� ȿ�� 
            rotationAmount += 720f * Time.deltaTime / jumpDuration; // �� 720�� ȸ��
            hero.transform.eulerAngles = new Vector3(0, 0, rotationAmount);

            elapsed += Time.deltaTime;
            yield return null;
        }

        hero.transform.eulerAngles = Vector3.zero;  // ȸ�� �ʱ�ȭ
        hero.IsInvincible = false;

        Airborne();

        Destroy(gameObject);
    }

    private void Airborne()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(hero.transform.position, blowAwayRadius, enemyLayer);
        foreach (var enemyCollider in enemies)
        {
            Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // �÷��̾�κ��� ���ͱ����� ������ ���
                Vector2 blowDirection = (enemyRb.transform.position - hero.transform.position).normalized;

                // ���͸� �ڷ� �о�� �ణ ���� ���ϰ�...
                Vector2 force = blowDirection * 10f + Vector2.up * 5f; 
                enemyRb.AddForce(force, ForceMode2D.Impulse);

                // ���Ͱ� ȸ���ϵ���
                enemyRb.AddTorque(10f, ForceMode2D.Impulse); 

                Destroy(enemyCollider.gameObject, 2f); // 2�� �� �Ҹ�
            }
        }
    }
}