using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Data;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
//using static UnityEditor.Progress;
using System.Linq;
using System.Collections;
using System;
using UnityEngine.AI;
using static Define;

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
        // 스킬선택 0번~2번에 있는 스킬에 따라서 돌파스킬에 필요한 스킬을 찾아낸다.
        int count = 0;
        foreach (var item in Managers.Skill.sampleSkillList)
        {
            // Debug.Log($"[샘플스킬리스트 {count}]{item.SkillData.Name}");
            int currentSkillID = item.SkillData.SkillID;
            int lv1ID_current = BreakthroughHelper.Instance.GetFirstLvSkillID(currentSkillID); // Lv1스킬의 ID
            Define.SkillType currentSkillType = item.SkillData.skillType;
            int combinationSkillID = -1; // 돌파에 필요한 스킬ID
            string combinationSkillName; // 돌파에 필요한 스킬이름
            bool isContiansSkill = false;
            
            // 현재 선택 스킬이 돌파스킬인경우
            if(currentSkillType == SkillType.Breakthrough)
            {
                count++; /*Debug.Log($"[돌파스킬감지 {count}]");*/ continue;
            }
            // 현재 선택 스킬이 패시브인경우
            if (currentSkillType == Define.SkillType.Passive)
            {
                // 이 패시브 스킬과 관련된 돌파스킬조건 만족 스킬을 찾는다.
                foreach (var btSkillData in Managers.Data.BreakthroughDic)
                {
                    int lv1ID_BT = BreakthroughHelper.Instance.GetFirstLvSkillID(btSkillData.Value.G_Skill_ID2); // G_Skill_ID2(패시브)
                    if (lv1ID_BT == lv1ID_current)  
                    {
                        // 돌파에 필요한 액티브 스킬을 출력
                        combinationSkillID = btSkillData.Value.G_Skill_ID1;
                        break;
                    }
                }
                List<SkillBase> usingSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Active];
                // 현재 스킬을 가지고 있는지 검사한다.
                foreach (var usingSkill in usingSkillList)
                {
                    int lv1ID_currentUsing = BreakthroughHelper.Instance.GetFirstLvSkillID(usingSkill.SkillData.SkillID); // Lv1스킬의 ID
                    int lv1ID_combinationSkillID = BreakthroughHelper.Instance.GetFirstLvSkillID(combinationSkillID);     // Lv1스킬의 ID
                    isContiansSkill = (lv1ID_currentUsing == lv1ID_combinationSkillID);
                    combinationSkillName = Managers.Data.SkillDic[lv1ID_currentUsing].Name;
                    if(isContiansSkill)
                    {
                        UpdateCombinationImage(views[count], combinationSkillName, true);
                        // Debug.Log($"[출력(1){count}]{combinationSkillName}");
                        count++;
                        break;
                    }
                }
                if (isContiansSkill == false) // 미보유 표시
                {
                    if(combinationSkillID < 0) { count++; /*Debug.Log($"[미보유2 {count}]");*/ continue; } // 돌파조합이 없는 스킬인 경우 예외처리
                    combinationSkillName = Managers.Data.SkillDic[combinationSkillID].Name;
                    UpdateCombinationImage(views[count], combinationSkillName, false);
                    // Debug.Log($"[출력(2){count}]{combinationSkillName}");
                    count++;
                    continue;
                }
            }

            // 현재 선택 스킬이 액티브인경우
            if (currentSkillType == Define.SkillType.Active)
            {
                // 이 패시브 스킬과 관련된 돌파스킬조건 만족 스킬을 찾는다.
                foreach (var btSkillData in Managers.Data.BreakthroughDic)
                {
                    int lv1ID_BT = BreakthroughHelper.Instance.GetFirstLvSkillID(btSkillData.Value.G_Skill_ID1); // G_Skill_ID1(액티브)
                    if (lv1ID_BT == lv1ID_current)
                    {
                        // 돌파에 필요한 패시브 스킬을 출력
                        combinationSkillID = btSkillData.Value.G_Skill_ID2;
                        break;
                    }
                }
                List<SkillBase> usingSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Passive];
                // 현재 스킬을 가지고 있는지 검사한다.
                foreach (var usingSkill in usingSkillList)
                {
                    int lv1ID_currentUsing = BreakthroughHelper.Instance.GetFirstLvSkillID(usingSkill.SkillData.SkillID); // Lv1스킬의 ID
                    int lv1ID_combinationSkillID = BreakthroughHelper.Instance.GetFirstLvSkillID(combinationSkillID);     // Lv1스킬의 ID
                    isContiansSkill = (lv1ID_currentUsing == lv1ID_combinationSkillID);
                    combinationSkillName = Managers.Data.SkillDic[lv1ID_currentUsing].Name;
                    if (isContiansSkill)
                    {
                        UpdateCombinationImage(views[count], combinationSkillName, true);
                        // Debug.Log($"[출력(3){count}]{combinationSkillName}");
                        count++;
                        break;
                    }
                }
                if (isContiansSkill == false) // 미보유 표시
                {
                    if (combinationSkillID < 0) { count++; /*Debug.Log($"[미보유4 {count}]");*/ continue; } // 돌파조합이 없는 스킬인 경우 예외처리
                    combinationSkillName = Managers.Data.SkillDic[combinationSkillID].Name;
                    UpdateCombinationImage(views[count], combinationSkillName, false);
                    // Debug.Log($"[출력(4){count}]{combinationSkillName}");
                    count++;
                    continue;
                }
            }
        }
    }

    private void PreviewImage()
    {
        List<SkillBase> activeSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Active];
        List<SkillBase> passiveSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Passive];
        List<SkillBase> btSkillList = Managers.Skill.usingSkillDic[Define.SkillType.Breakthrough];
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

        foreach (var item in btSkillList)
        {
            var obj = Managers.Pool.Pop(ActiveAndPassiveImage.gameObject);
            obj.transform.SetParent(activeGrid);
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

    // 조합 추천 스킬 이미지만 업데이트
    public void UpdateCombinationImage(IView view, string name, bool flag) // name: 스킬이름, flag: 조합스킬 보유여부
    {
        view.UpdateUI(new { SkillName = name, IsContiansSkill = flag });
    }

    private void ViewsOnOff(bool enable)
    {
        foreach (var item in views)
        {
            item.gameObject.SetActive(enable);
        }
    }
}
