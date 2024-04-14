using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// (무한맵) 맵 재배치 기능 클래스. @홍지형
public class MapManager : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;
     
        Vector3 playerPos = Managers.Object.Hero.transform.position;
        Vector3 myPos = transform.position;
     
        switch (transform.tag)
        {
            case "Map":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 64);

                    // Debug.Log($"{gameObject.name}: (diffX > diffY) transform:{transform.position}");
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 64);
                   //  Debug.Log($"{gameObject.name}:  (diffX < diffY) transform:{transform.position}");
                }
                else
                {
                    transform.Translate(dirX * 64, dirY * 64, 0);
                }


                break;
            // TODO: 추후 필요하면 사용, Area 크기 다르게 할 필요 있음.
            case "Enemy":

                break;
        }
        

    }
   

}
