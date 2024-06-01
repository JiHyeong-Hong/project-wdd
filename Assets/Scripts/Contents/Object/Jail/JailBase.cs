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

    private SpriteRenderer passiveIcon;
    public SpriteRenderer PassiveIcon
    {
        get
        {
            if (passiveIcon == null)
            {
                passiveIcon = Util.FindChild<Transform>(gameObject, "passiveIcon", true).gameObject.GetComponent<SpriteRenderer>();
            }
            return passiveIcon;
        }
    }

    private int SignCount = 0;

    private bool isColliding = false;
    private Coroutine openCoroutine;
    private Coroutine timerCoroutine;


    public void Init()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SetJailSprite(JailType.Big, jailName);
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

        string passiveName = BreakthroughHelper.Instance.FindPassiveName(skillName);

        Sprite sprite = null;
        Sprite[] multiSprite = Resources.LoadAll<Sprite>($"Art/Jail/{jailType.ToString()}Jail");

        Sprite[] multiPassiveSprite = Resources.LoadAll<Sprite>($"Art/Sign/Passiveicon");



        foreach (Sprite s in multiSprite)
        {
            if (s.name == skillName)
            {
                sprite = s;
                break;
            }
        }

        if (JailSprite != null)
            JailSprite.sprite = sprite;

        foreach (Sprite s in multiPassiveSprite)
        {
            if (s.name == passiveName)
            {
                sprite = s;
                break;
            }
        }

        if(PassiveIcon != null)
            PassiveIcon.sprite = sprite;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isColliding)
            {
                isColliding = true;
                Managers.Object.Hero.keyAndTimer.SetActive(true);

                openCoroutine = StartCoroutine(OpenAfterDelay(3.0f));
                timerCoroutine = StartCoroutine(Util.FillAmount(Managers.Object.Hero.timerMaterial, 3.0f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isColliding)
            {
                isColliding = false;

                Managers.Object.Hero.keyAndTimer.SetActive(false);

                if (openCoroutine != null)
                    StopCoroutine(openCoroutine);

                if (timerCoroutine != null)
                    StopCoroutine(timerCoroutine);
            }
        }
    }

    private IEnumerator OpenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isColliding)
        {
            Debug.Log("Object opened");
            JailAction();
        }
    }

}
