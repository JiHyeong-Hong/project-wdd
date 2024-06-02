using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcquisitionPopup : PopupBase
{
    [SerializeField]
    private int moveSpeed;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private Image skillImage;
    private TMP_Text desc;
    private Vector2 originPosition;
    private Coroutine coroutine;
    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        skillImage = Util.FindChild<Image>(gameObject, "SkillImage", false);
        desc = Util.FindChild<TMP_Text>(gameObject, "DESC", false);

        originPosition = rt.anchoredPosition;

        SetData(Define.SkillType.Active, "Tiger");
    }

    protected override void OnShow()
    {
        base.OnShow();
        coroutine = StartCoroutine(MoveTo());
    }

    protected override void OnHide()
    {
        rt.anchoredPosition = originPosition;
        canvasGroup.alpha = 1;

        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    public void SetData(Define.SkillType type, string skillName)
    {
        Sprite sprite = null;
        Sprite[] multiSprite = null;

        switch (type)
        {
            case Define.SkillType.Active:
                multiSprite = Resources.LoadAll<Sprite>($"Art/Skills/{skillName}");
                sprite = multiSprite[0];
                break;
            case Define.SkillType.Passive:
                multiSprite = Resources.LoadAll<Sprite>($"Art/Sign/Passiveicon");
                foreach (Sprite s in multiSprite)
                {
                    if (s.name == skillName)
                    {
                        sprite = s;
                        break;
                    }
                }
                break;
            case Define.SkillType.Breakthrough:
                break;
            default:
                break;
        }


        skillImage.sprite = sprite;
        //desc.text = "Lv." + Managers.Skill.usingSkillDic[type].Where(x => x.SkillData.Name == skillName).Select(x => x.SkillData.Level).FirstOrDefault().ToString();

        Show();
    }


    private IEnumerator MoveTo()
    {
        Vector2 target = new Vector2(-rt.rect.width + rt.anchoredPosition.x, rt.anchoredPosition.y);

        while (rt.anchoredPosition != target)
        {
            MoveTo(target);
            yield return null;  
        }

        target = new Vector2(rt.anchoredPosition.x, rt.rect.width + rt.anchoredPosition.y);

        StartCoroutine(Util.FadeOut(canvasGroup, 1,
                () =>
                {
                    Hide();
                }));

        while (rt.anchoredPosition != target)
        {
            MoveTo(target);
            yield return null;
        }
    }

    private void MoveTo(Vector2 dir)
    {
        rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, dir, moveSpeed * Time.deltaTime);
    }


}
