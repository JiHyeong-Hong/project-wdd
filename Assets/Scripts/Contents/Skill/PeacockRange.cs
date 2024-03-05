using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CircleCollider2D))]
public class PeacockRange : MonoBehaviour
{
    [SerializeField] private CircleCollider2D collider;

    private Monster[] targets;
    private List<Monster> colList;
    private int targetNum;
    
    public void Init()
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
        
        collider.enabled = true;
    }

    public async Task<Monster[]> GetTargets()
    {
        await Task.Delay(TimeSpan.FromSeconds(2f / 60f));
        collider.enabled = false;
        SelectRandomTarget();

        return targets;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        colList.Clear();
        if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
        {
            colList.Add(col.GetComponent<Monster>());
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
