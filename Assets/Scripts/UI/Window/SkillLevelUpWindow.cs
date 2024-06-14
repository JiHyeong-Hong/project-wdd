using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Data;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SkillLevelUpWindow : UIWindow
{
    private List<UniTaskCompletionSource<bool>> completionSources = new List<UniTaskCompletionSource<bool>>();
    private List<CancellationTokenSource> cancelTokenSources = new List<CancellationTokenSource>();


    [SerializeField]
    private Transform thumbnailGrid;
    [SerializeField]
    private GameObject thumbnailPrefab;
    [SerializeField]
    private Image ActiveAndPassiveImage;
    [SerializeField]
    private Transform activeGrid;
    [SerializeField]
    private Transform passiveGrid;
    [SerializeField]
    private Transform combinationGrid;
    [SerializeField]
    private GameObject combinationPrefab;

    private List<IView> views = new List<IView>();
    private List<SkillLevelUpPresenter> presenters = new List<SkillLevelUpPresenter>();

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    protected override void OnShow()
    {
        Managers.Game.IsGamePaused = true;
        //Managers.UI.Joystick.gameObject.SetActive(false);
        SetLevelUpUI();
        PreviewImage();
        PreviewCombinationSkill();
        ViewsOnOff(true);
    }

    protected override void OnHide()
    {
        Managers.Game.IsGamePaused = false;
        //Managers.UI.Joystick.gameObject.SetActive(true);
        ViewsOnOff(false);
    }

    private void Init()
    {
        Managers.Pool.CreatePool(ActiveAndPassiveImage.gameObject);

        FindTransform(out thumbnailGrid, "ThumbnailGrid");
        FindTransform(out activeGrid, "ActiveGrid");
        FindTransform(out passiveGrid, "PassiveGrid");
        FindTransform(out combinationGrid, "CombinationGrid");

        for (int i = 0; i < 3; ++i)
        {
            GameObject obj = Instantiate(thumbnailPrefab, thumbnailGrid);
            SkillLevelUpView view = obj.GetComponent<SkillLevelUpView>();
            views.Add(view);

            SkillLevelUpPresenter presenter = new SkillLevelUpPresenter(view, Managers.Skill);
            view.SetPresenter(presenter);
            presenters.Add(presenter);
        }
    }

    private void FindTransform(out Transform target, string name)
    {
        target = Util.FindChild<Transform>(gameObject, name, true);
    }

    private void PreviewCombinationSkill()
    {

    }

    private void PreviewImage()
    {
        List<SkillBase> activeSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Active];
        List<SkillBase> passiveSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Passive];

        foreach (var item in activeSkillList)
        {
            var obj = Managers.Pool.Pop(ActiveAndPassiveImage.gameObject);
            obj.transform.SetParent(activeGrid);

            obj.GetComponent<Image>().sprite = Managers.Resource.GetSkillSprite(item.SkillData.Name);

            WaitPoolPush(obj).Forget();
        }

        foreach (var item in passiveSkillList)
        {
            var obj = Managers.Pool.Pop(ActiveAndPassiveImage.gameObject);
            obj.transform.SetParent(passiveGrid);

            obj.GetComponent<Image>().sprite = Managers.Resource.GetSkillSprite(item.SkillData.Name);

            WaitPoolPush(obj).Forget();
        }
    }

    private async UniTask WaitPoolPush(Poolable waitObject)
    {
        await UniTask.WaitUntil(() => this.isActiveAndEnabled == false);
        waitObject.IsUsing = false;
        Managers.Pool.Push(waitObject);

    }

    private void SetLevelUpUI()
    {
        List<string> list = Util.SelectUniqueElements(Managers.Skill.canPickSkillList, 3);

        //Assert.IsTrue(list.Count != 0, "Skill List is Empty");
        if (list.Count == 0) return;

        for(int i = 0; i < views.Count; ++i)
        {
            bool isSkillFound = false;
            SkillData skillData = Managers.Skill.allSkillDic[list[i]][0].SkillData;
            if (skillData.skillType == 0) continue;

            List<SkillBase> skillsWithSameType = Managers.Skill.usingSkillDic[skillData.skillType];

            foreach (var item in skillsWithSameType)
            {
                if (item.SkillData.Name == list[i])
                {
                    UpdateViewAndPresenter(views[i], presenters[i], item.SkillData);
                    isSkillFound = true;
                    break;
                }
            }

            if (!isSkillFound)
            {
                UpdateViewAndPresenter(views[i], presenters[i], skillData);
            }
        }
    }
    void UpdateViewAndPresenter(IView view, SkillLevelUpPresenter presenter, SkillData skillData)
    {
        view.UpdateUI(Managers.Skill.allSkillDic[skillData.Name][skillData.Level]);
        presenter.SetSkillData(skillData);
    }

    private void ViewsOnOff(bool enable)
    {
        foreach (var item in views)
        {
            item.gameObject.SetActive(enable);
        }
    }
}
