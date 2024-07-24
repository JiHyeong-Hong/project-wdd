using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ũ ���� ������ ���� ��ų Ŭ����. @ȫ����
public class SkunkPoison : Projectile
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites; //3~6���� ���

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        return true;
    }

    public override void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction)
    {
        base.SetSpawnInfo(owner, skill, direction);

        StartCoroutine(LoopAnimation());
    }

    public void SetSpawnInfo(Creature owner, SkillBase skill, Vector2 direction, bool isBTSkill)
    {
        if (isBTSkill)
        {
            sprites = Resources.LoadAll<Sprite>("Art/Skills/SkunkBT");
            Animator.SetBool("isBreakthrough", true);
            transform.localScale = new Vector3(2.6f, 2.6f, 2.6f);
        }
        else
        {
            sprites = Resources.LoadAll<Sprite>("Art/Skills/Skunk");
            Animator.SetBool("isNormal", true);
        }

        SetSpawnInfo(owner, skill, direction);
    }

    private IEnumerator LoopAnimation()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        int index = 0;
        while (true)
        {
            spriteRenderer.sprite = sprites[3 + index];

            index = (index + 1) % 4;
            yield return wait;
        }
    }



    private void Update()
    {
   

    }

    float currentTime = 1;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (currentTime > 0.5f && LayerMask.NameToLayer("Monster") == other.gameObject.layer)
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            monster.OnDamaged(Owner, Skill);
            currentTime = 0;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
