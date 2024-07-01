using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        //StageManager.Instance.GetStage(1);
        //StageManager.Instance.BroadcastMessage("StartStage");
    }


    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.V)) 
        // {
        //     FindSkillForName("Bear");
        // }
        //
        if(Input.GetKeyDown(KeyCode.V)) 
        {
            Managers.Object.Spawn<Exp>(new Vector3(1, 0, 0f), 31);
            // Managers.Object.Spawn<Magnet>(new Vector3(0, 1, 0f), 41);
            Managers.Object.Spawn<Trumpet>(new Vector3(1, 1, 0f), 51);
            Managers.Object.Spawn<Medkit>(new Vector3(2, 0, 0f), 61);
            Managers.Object.Spawn<BulletproofVest>(new Vector3(2, 0, 0f), 71);
            Managers.Object.Spawn<Gold>(new Vector3(2, 1, 0f), 81);
            Managers.Object.Spawn<Gold>(new Vector3(3, 0, 0f), 82);
            Managers.Object.Spawn<Gold>(new Vector3(3, 1, 0f), 83);
            Managers.Object.Spawn<Key>(new Vector3(3, 2, 0f), 91);
            

            
        }
    }

    public void FindSkillForName(string name)
    {
        foreach (var item in Managers.Skill.usingSkillDic[Define.SkillType.Active])
        {

            if (item.SkillData.Name == name)
            {
                string json = JsonUtility.ToJson(item.SkillData);
                Debug.Log(json);
                //PlayerPrefs.SetString("SkillData", json);
            }

        }
    }
}
