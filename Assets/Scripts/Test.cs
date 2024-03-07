using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    private float cooltime = 1f;
    private float tick;
    
   
    void Update()
    {
        tick += Time.deltaTime;

        if (tick >= cooltime)
        {
            Debug.Log("발동");
            tick = 0f;
        }
    }
}
