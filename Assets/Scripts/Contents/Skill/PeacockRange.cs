using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CircleCollider2D))]
public class PeacockRange : MonoBehaviour
{
    private CircleCollider2D collider;

    private Monster[] targets;
    private List<Monster> colList;
    private int targetNum;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        colList = new List<Monster>();
        collider.isTrigger = true;
        collider.enabled = false;
    }

    public void SetData(float radius, int targetNum)
    {
        this.targetNum = targetNum;
        targets = new Monster[targetNum];
        collider.radius = radius;
    }

    public void CheckTarget()
    {
        for (int i = 0; i < targets.Length; i++)
            targets[i] = null;
        
        colList.Clear();
        collider.enabled = true;
    }

    public async UniTask<Monster[]> GetTargets()
    {
        await UniTask.DelayFrame(2);
        collider.enabled = false;
        SelectRandomTarget();

        return targets;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
        {
            var monster = col.GetComponent<Monster>();
           
            if (monster.Hp <= 0)
                return;
            
            colList.Add(monster);
        }
    }
    
    /// <summary>
    /// 범위 내 랜덤한 타겟을 정한다.
    /// </summary>
    private void SelectRandomTarget()
    {
        int cnt = 0;
        while (cnt < targetNum)
        {
            if (colList.Count == 0)
                break;
            
            int randomIdx = Random.Range(0, colList.Count);
            targets[cnt++] = colList[randomIdx];
            colList.RemoveAt(randomIdx);
        }
    }
    
}
