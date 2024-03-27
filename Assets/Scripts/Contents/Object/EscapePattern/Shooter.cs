using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �渮���� @ȫ����
public class Shooter : Creature 
{
    private Transform _target;
    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;
    private StunBulletSkill _skill;

    private bool _isFired = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // ��������Ʈ
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // ������ �ð�ȭ
        _lineRenderer = GetComponent<LineRenderer>(); 
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = 0.05f; // �ʺ� ����
        _lineRenderer.endWidth = 0.05f;

        _target = Managers.Object.Hero?.transform; // Ÿ�� ��ġ �ʱ�ȭ
        StartCoroutine(AimAndFireCycle());
        StartCoroutine(CoUpdateAI());
        return true;
    }
    void Update()
    {
        if (!_isFired && _target != null)
        {            
            Aim();
        }

        // 1�� ��� ���̻� ���� �ʰ� �����.
        if (_isFired == true) 
        {
            if(_skill.StunBullet == null) // �߻� �� �����Ѿ��� ���������
                Destroy(gameObject);      // shooter �����Ѵ�.
        }        
    }

    // �����ð� ���� �� �߻� �Ѵ�.
    IEnumerator AimAndFireCycle()
    {
        // ���� �߻� ���
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("[�渮����] ���� ��... " + (i + 1) + "�� ���");
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(Fire());
        StopCoroutine(Fire());

        // �渮���� ��ü�� �ð������� �����.
        _spriteRenderer.enabled = false;
        _lineRenderer.enabled = false;
        
        GetComponentInChildren<TargetCrosshair>().enabled = false;     // �������� �̵��� �����.

        // ���� ����
        StopCoroutine(AimAndFireCycle());    
    }

    // ������ �߻�
    IEnumerator Fire()
    {       
        // ���� ������ �ð�ȭ
        //_lineRenderer.startColor = Color.red;
        //_lineRenderer.endColor = Color.red;

        // ����ü �߻�
        Debug.Log("[�渮����] �߻�!");

        // �����Ѿ� Ŭ���� ����
        _skill = new StunBulletSkill();
        _skill.SetInfo(Managers.Data.SkillDic[9999]); // �ӽ� ��ų ��ȣ. @ȫ���� 240324
        _skill.SetOwner(this);
        _skill.DoSkill();
        //


        yield return new WaitForSeconds(1f);  // �Ѿ� ��ü�� �����Ǳ⸦ ��ٸ���.
      
        _isFired = true; // �߻� ���·� ����
    }

    // ���ݼ��� �÷��̾ �����Ѵ�.
    void Aim()
    {
        // ���ݼ� ȸ��
        Vector2 dir = _target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (dir != Vector2.zero)
            Direction = dir;    // ��ġ���� ����

        // ���� ������ �ð�ȭ
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _target.position);
    }
}
