using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Data;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using static UnityEditor.Progress;
using System.Linq;
using System.Collections;

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
        // Managers.Game.IsGamePaused = true; // 삭제예정
        //Managers.UI.Joystick.gameObject.SetActive(false);

        // for (int i = 0; i < views.Count; ++i) // 백업
        for (int i = 0; i < Managers.Skill.sampleSkillList.Count; ++i)
        {
            SkillData skillData = Managers.Skill.sampleSkillList[i].SkillData;
            if (skillData.skillType == 0) continue;
            if (presenters[i].Model.skillData == null)
            {
                UpdateViewAndPresenter(views[i], presenters[i], skillData);
                continue;
            }

            if (CheckSameSkillUI(presenters[i]) == true) { continue; }

            UpdateViewAndPresenter(views[i], presenters[i], skillData);
        }

        PreviewImage();
        PreviewCombinationSkill();
        ViewsOnOff(true);
    }


    // 이미 추가된 스킬레벨업UI가 있는지 검사한다.
    private bool CheckSameSkillUI(SkillLevelUpPresenter presenter)
    {        
        // sampleSkillList에 동일한 레벨의 스킬이 UI에 이미 있는지 검사한다.
        var sampleSkillList = Managers.Skill.sampleSkillList;
        foreach (var a in sampleSkillList)
        {
            if (a.SkillData.Name == presenter.Model.skillData.Name)
            {
                return true;
            }
        }
        return false;
    }


    //// 같은 string 값을 가진 원소가 있는지 검사하는 메서드
    //bool CheckForCommonElement(List<SkillBase> listA, List<SkillLevelUpPresenter> listB)
    //{
    //    // 비교군 A
    //    HashSet<string> setName = new HashSet<string>(listA.Select(a => a.SkillData.Name));
    //    HashSet<int> setLevel = new HashSet<int>(listA.Select(a => a.SkillData.Level));


    //    // 비교군 B에서 스킬이름과 레벨이 같은게 있는지 확인
    //    foreach (var b in listB)
    //    {
    //        if (setA.Contains(b.Model.skillData.Name))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    // 같은 string 값을 가진 원소가 있는지 검사하는 메서드
    //bool CheckForCommonElement(List<SkillBase> listA, List<SkillLevelUpPresenter> listB)
    //{
    //    // sampleSkillList에 동일한 레벨의 스킬이 UI에 이미 있는지 검사한다.
    //    bool hasMatchingElement = Managers.Skill.sampleSkillList.Any(a => presenters.Any(b => a.SkillData.Name == b.Model.skillData.Name && a.SkillData.Level == b.Model.skillData.Level));
    //    // bool hasMatchingElement = listA.Any(a => listB.Any(b => a.SkillData.Name == b.Model.skillData.Name && a.SkillData.Level == b.Model.skillData.Level));

    //    if (hasMatchingElement)
    //    {
    //        Debug.Log("같은 name과 level을 가진 원소가 존재합니다.");
    //    }
    //    else
    //    {
    //        Debug.Log("같은 name과 level을 가진 원소가 없습니다.");
    //    }
    //}

    protected override void OnHide()
    {
        // Managers.Game.IsGamePaused = false; // 삭제예정
        //Managers.UI.Joystick.gameObject.SetActive(true);
        ViewsOnOff(false);
    }

    private void Init()
    {
        // if (UIManagerNew.Instance.IsWindowCached(Define.UIWindowType.SkillLevelUpWindow) == true) { return; }
        
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

    // 240804 @홍지형
    private void SetLevelUpUI()
    {        
        for (int i = 0; i < views.Count; ++i)
        {
            // bool isSkillFound = false;
            SkillData skillData = Managers.Skill.sampleSkillList[i].SkillData;
            if (skillData.skillType == 0) continue;

            UpdateViewAndPresenter(views[i], presenters[i], skillData);

            // DEBUG::
            // Debug.Log($"SetLevelUpUI()::{i}" + skillData.Name);
        }
    }

    void UpdateViewAndPresenter(IView view, SkillLevelUpPresenter presenter, SkillData skillData)
    {        
        view.UpdateUI(Managers.Skill.allSkillDic[skillData.Name][skillData.Level - 1]);
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
