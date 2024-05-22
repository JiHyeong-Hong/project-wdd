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
        if(Input.GetKeyDown(KeyCode.V)) 
        {
            FindSkillForName("Bear");
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
