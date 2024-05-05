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
                case 2:
                    monster_List.Add(item.Value);
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
                
            spawner_List[ran].Spawn(monster.Index);
            //TODO Eung StageLv 테이블로 단계/페이즈에 따라 스폰시간 넣어서 관리
            yield return new WaitForSeconds(1f);
        }
    }

}
