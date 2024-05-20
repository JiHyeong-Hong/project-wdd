using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스컹크 지속 데미지 장판 스킬 클래스. @홍지형
public class SkunkPoison : Projectile
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites; //3~6까지 사용

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
            transform.localScale = new Vector3(2.6f, 2.6f, 2.6f);
        }
        else
        {
            sprites = Resources.LoadAll<Sprite>("Art/Skills/Skunk");
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

    float currentTime = 0.5f;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (currentTime > 0.1f && LayerMask.NameToLayer("Monster") == other.gameObject.layer)
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
