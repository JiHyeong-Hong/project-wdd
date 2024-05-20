using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallJail : JailBase
{
    public override void JailAction()
    {
        base.JailAction();
        Debug.Log("Small Jail Action");
    }
}
