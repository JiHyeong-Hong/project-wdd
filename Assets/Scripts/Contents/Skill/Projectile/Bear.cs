using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Projectile
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;
		canMove = false;
		Renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		return true;
	}
}
