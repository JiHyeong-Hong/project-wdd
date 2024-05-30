using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class SpawnManager
{
    
    public Camera camera;
    private float size_y_;
    private float size_x_;

    public float Bottom
    {
        get
        {
            return size_y_ * -1 + camera.gameObject.transform.position.y;
        }
    }
    public float Top
    {
        get
        {
            return size_y_ + camera.gameObject.transform.position.y;
        }
    }
    public float Left
    {
        get
        {
            return size_x_ * -1 + camera.gameObject.transform.position.x;

        }
    }
    public float Right
    {
        get
        {
            return size_x_ + camera.gameObject.transform.position.x;
        }
    }
    public float Height
    {
        get
        {
            return size_y_ * 2;
        }
    }
    public float Width
    {
        get
        {
            return size_x_ * 2;
        }
    }
    
    public List<Spawner> spawner_List = new List<Spawner>();

    public void Init()
    {
        camera = Camera.main;
        size_y_ = camera.orthographicSize;
        size_x_ = camera.orthographicSize * Screen.width / Screen.height;
        SetSpawnPos();

        // CreateSpawner(); // 240502 홍지형
    }

    public void CreateTop()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 pos;
            switch (i)
            {
                case 0:
                    pos = new Vector2(Width/4 * -1, Top + 2.5f);
                    break;
                case 1:
                    pos = new Vector2(Width/4, Top + 2.5f);
                    break;
                default:
                    pos = new Vector2(Width/4, Top + 2.5f);
                    break;
            }
            var sp = Managers.Object.Spawn<Spawner>(pos, 0);
            spawner_List.Add(sp);
        }
    }
    public void CreateBottom()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 pos;
            switch (i)
            {
                case 0:
                    pos = new Vector2(Width/4 * -1, Bottom - 2.5f);
                    break;
                case 1:
                    pos = new Vector2(Width/4, Bottom - 2.5f);
                    break;
                default:
                    pos = new Vector2(Width/4, Bottom - 2.5f);
                    break;
            }
            var sp = Managers.Object.Spawn<Spawner>(pos, 0);
            spawner_List.Add(sp);
        }
    }
    public void CreateLeft()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos;
            switch (i)
            {
                case 0:
                    pos = new Vector2(Left - 2.5f,camera.gameObject.transform.position.y + Height/3);
                    break;
                case 1:
                    pos = new Vector2(Left- 2.5f, camera.gameObject.transform.position.y);
                    break;
                case 2:
                    pos = new Vector2(Left- 2.5f, camera.gameObject.transform.position.y - Height/3);
                    break;
                
                default:
                    pos = Vector2.zero;
                    break;
            }
            var sp = Managers.Object.Spawn<Spawner>(pos, 0);
            spawner_List.Add(sp);
        }
    }
    public void CreateRight()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos;
            switch (i)
            {
                case 0:
                    pos = new Vector2(Right + 2.5f,camera.gameObject.transform.position.y + Height/3);
                    break;
                case 1:
                    pos = new Vector2(Right + 2.5f, camera.gameObject.transform.position.y);
                    break;
                case 2:
                    pos = new Vector2(Right + 2.5f, camera.gameObject.transform.position.y - Height/3);
                    break;
                
                default:
                    pos = Vector2.zero;
                    break;
            }
            var sp = Managers.Object.Spawn<Spawner>(pos, 0);
            spawner_List.Add(sp);
        }
    }
    
    public void CreateSpawner()
    {
        CreateTop();
        CreateBottom();
        CreateLeft();
        CreateRight();
    }

    public List<Data.MonsterData> monster_List = new List<MonsterData>();

    public IEnumerator Spawn()
    {
        foreach (var item in Managers.Data.MonsterDic)
        {
            switch (item.Value.Type)
            {
                case 1:
                    monster_List.Add(item.Value);
                    break;
                // case 2:
                //     monster_List.Add(item.Value);
                    break;
                default:
                    break;
            }
        }

        while (true)
        {
            int ran = Random.Range(0, spawner_List.Count);
            int ran2 = Random.Range(0, monster_List.Count);
            Data.MonsterData monster = monster_List[ran2];
                
            spawner_List[ran].Spawn(monster.MonsterID);
            //TODO Eung StageLv 테이블로 단계/페이즈에 따라 스폰시간 넣어서 관리
            yield return new WaitForSeconds(1f);
        }
    }

    private List<Vector2> SpawnPos = new List<Vector2>();
    private float spawnDistance = 1.0f;
    private void SetSpawnPos()
    {
        // Get screen bounds in world coordinates relative to player position
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float heightSize = screenTopRight.y;
        float widthSize = screenTopRight.x;
        
        Vector2 bottomLeft = new Vector2(screenBottomLeft.x + (widthSize / 4), screenBottomLeft.y - spawnDistance);
        Vector2 bottomRight = new Vector2(screenTopRight.x - (widthSize / 4), screenBottomLeft.y - spawnDistance);
        
        Vector2 leftTop = new Vector2(screenBottomLeft.x - spawnDistance, screenTopRight.y - (heightSize / 6));
        Vector2 leftMid = new Vector2(screenBottomLeft.x - spawnDistance, screenBottomLeft.y + (heightSize / 2));
        Vector2 leftBottom = new Vector2(screenBottomLeft.x - spawnDistance, screenBottomLeft.y + (heightSize / 6));

        Vector2 rightTop = new Vector2(screenTopRight.x + spawnDistance, screenTopRight.y - (heightSize / 6));
        Vector2 rightMid = new Vector2(screenTopRight.x + spawnDistance, screenBottomLeft.y + (heightSize / 2));
        Vector2 rightBottom = new Vector2(screenTopRight.x + spawnDistance, screenBottomLeft.y + (heightSize / 6));

        Vector2 topLeft = new Vector2(screenBottomLeft.x + (widthSize / 4), screenTopRight.y + spawnDistance);
        Vector2 topRight = new Vector2(screenTopRight.x - (widthSize / 4), screenTopRight.y + spawnDistance);

        Vector2 v2 = Managers.Object.Hero.transform.position;
        SpawnPos.Add(v2 -bottomLeft);
        SpawnPos.Add(v2 -bottomRight);

        SpawnPos.Add(v2 - leftTop);
        SpawnPos.Add(v2 - leftMid);
        SpawnPos.Add(v2 - leftBottom);

        SpawnPos.Add(v2 - rightTop);
        SpawnPos.Add(v2 - rightMid);
        SpawnPos.Add(v2 - rightBottom);

        SpawnPos.Add(v2 - topLeft);
        SpawnPos.Add(v2 - topRight);
    }
        
    private Vector2 SpawnPosNew()
    {
        int idx = Random.Range(0, 10);
        Vector2 v2 = Managers.Object.Hero.transform.position;
        return SpawnPos[idx] + v2;
        //Vector3 v3 = SpawnPos[idx];
        //Vector2 newPos Managers.Object.Hero.transform.position - v3;
        //Vector2 newPos = SpawnPos[idx];/* + new Vector2(Managers.Object.Hero.transform.position.x , Managers.Object.Hero.transform.position.y);*/
        //return SpawnPos[idx];
    }
    public void SpawnNew(int spawnId)
    {
        Vector2 pos = SpawnPosNew();
        Debug.LogWarning($"SpawnPos : {pos}");
        Managers.Object.Spawn<Monster>(pos, spawnId);
    }

}
