using System.Collections;
using UnityEngine;


public enum JailType
{
    Small,
    Big
}

public class JailBase : MonoBehaviour
{
    [SerializeField]
    private string jailName;
    private float enableTime = 2f;
    private float jailTime = 3f;

    private SpriteRenderer sign;

    public SpriteRenderer Sign 
    { 
        get
        {
            if (sign == null)
            {
                sign = transform.Find("Sign").gameObject.GetComponent<SpriteRenderer>();
            }
            return sign;
        }
    }

    private SpriteRenderer jailSprite;
    public SpriteRenderer JailSprite 
    { 
        get
        {
            if(jailSprite == null)
            {
                jailSprite = GetComponent<SpriteRenderer>();
            }
            return jailSprite;
        }
    }

    private int SignCount = 0;

    public void Init()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //SetJailSprite(JailType.Big, "Tiger");
        }
    }

    public virtual void JailAction() 
    {
        GetComponent<BoxCollider2D>().enabled = false;
        Managers.Skill.AddActiveSkill(jailName);

        StartCoroutine(Util.FadeOut(JailSprite, enableTime, ()=> { Destroy(this.gameObject); }));
        StartCoroutine(Util.FadeOut(Sign, enableTime));

        AchievementsHelper.Instance.IncrementAchievement("Jail", SignCount);

    }

    public void SetJailAndSign(JailType jailType, string skillName, int signCount)
    {
        SetJailSprite(jailType, skillName);
        signCount = SignCount;
    }

    public void SetJailSprite(JailType jailType, string skillName)
    {
        jailName = skillName;

        Sprite sprite = null;
        Sprite[] multiSprite = Resources.LoadAll<Sprite>($"Art/Jail/{jailType.ToString()}Jail");

        foreach (Sprite s in multiSprite)
        {
            if (s.name == skillName)
            {
                sprite = s;
                break;
            }
        }

        if (jailSprite == null)
        {
            jailSprite = GetComponent<SpriteRenderer>();
        }
        jailSprite.sprite = sprite;
    }

    float currentTime = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentTime = 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentTime = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            currentTime += Time.deltaTime;
            //Debug.Log("currentTime: " + currentTime);
            if (currentTime >= jailTime)
            {
                JailAction();
            }
        }
    }
}
