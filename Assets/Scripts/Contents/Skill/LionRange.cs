using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LionRange : MonoBehaviour
{
	private CircleCollider2D col;
	
	private List<Monster> colList;
	private List<Monster> hitList;
	private List<Monster> stunList;

	private float ccRadius;
	private float radius;

	private void Awake()
	{
		colList = new List<Monster>();
		hitList = new List<Monster>();
		stunList = new List<Monster>();

		col = GetComponent<CircleCollider2D>();
		col.isTrigger = true;
		col.enabled = false;
	}

	public void SetData(float radius, float ccRadius)
	{
		this.radius = radius;
		this.ccRadius = ccRadius;

		col.radius = Math.Max(radius, ccRadius);
	}

	public void CheckTarget()
	{
		colList.Clear();
		hitList.Clear();
		stunList.Clear();
		
		col.enabled = true;
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (((1 << (int)Define.ELayer.Monster) & (1 << col.gameObject.layer)) != 0)
		{
			colList.Add(col.GetComponent<Monster>());
		}
	}

	/// <summary>
	/// 나중에 애니메이션에 맞춰서
	/// 바꿔야함
	/// </summary>
	/// <returns></returns>
	public async UniTask<(List<Monster> hitList,List<Monster> stunList)> GetTargets()
	{
		await UniTask.DelayFrame(2);
		col.enabled = false;

		//스턴,데미지 유닛 구분
		foreach (var col in colList)
		{
			if(col.Hp <= 0)
				continue;
			
			var sqr = Vector2.SqrMagnitude(transform.position - col.transform.position);
			
			if (sqr <= Mathf.Pow(radius, 2))
			{
				hitList.Add(col);
			}
			
			if(sqr <= Mathf.Pow(ccRadius, 2))
			{
				stunList.Add(col);
			}
		}
		
		return (hitList,stunList);
	}
}