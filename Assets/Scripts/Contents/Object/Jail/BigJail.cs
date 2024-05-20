using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigJail : JailBase
{
    public override void JailAction()
    {
        base.JailAction();
        Debug.Log("Big Jail Action");
    }
}
