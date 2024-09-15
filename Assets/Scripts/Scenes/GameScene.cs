using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectManager.Instance.SetHero();
        SceneType = Define.EScene.GameScene;

        Managers.UI.SetJoyStick(Managers.UI.ShowBaseUI<UI_Joystick>().gameObject);
        // Managers.UI.ShowSceneUI<UI_GameScene>(); // 테스트용. 지울것
        //Managers.UI.ShowWindowUI<InGameWindow>(Define.UIWindowType.Game).Show();

        //Managers.Object.Spawn<Hero>(new Vector3(0f, -15f, 0f), Define.HERO_ZOOKEEPER_ID);
        SoundManager.Instance.Play(Define.ESoundMainType.Bgm, Define.ESoundType.Ingame);
        Camera.main.GetOrAddComponent<FollowCamera>();

        // 게임씬을만들때 스포너 생성/스폰시작
        Managers.Spawner.Init();
        // StartCoroutine(Managers.Spawner.Spawn()); // 홍지형 임시.

        Managers.EscapePattern.SpawnEscapePattern();

        Managers.Stage.LoadStage(Managers.Game.currentStageID);

        // UIManagerNew.Instance.ShowWindow<WindowBase>(Define.UIWindowType.InGameWindow); // 캐싱        
        UIManagerNew.Instance.ShowWindow<InGameWindow>(false); // 논캐싱


        //for (int i = 0; i < 5; ++i)
        //    Managers.Object.Spawn<Monster>(new Vector3(-2f + i, -1f, 0f), Define.MONSTER_SECURITY1_ID);

        //for (int i = 0; i < 5; ++i)
        //    Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 0f, 0f), Define.MONSTER_SECURITY2_ID);

        //for (int i = 0; i < 5; ++i)
        //    Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 1f, 0f), Define.MONSTER_SECURITY3_ID);

        // 길리슈터 생성 테스트용. @홍지형
        // Managers.Object.Spawn<Shooter>(new Vector3(-5f, 5f, 0f), Define.MONSTER_SHOOTER_ID);

        
        //TODO Eung 보스 출현 카운트
        // StartCoroutine(Managers.Game.BossCount()); // 테스트용

        return true;
    }

    private void Update()
    {
        // DEBUG::
        if (Managers.Game.isStartGame && Managers.Object.Hero != null)
            Managers.Skill.UpdateSkillCoolTime(Time.deltaTime);
    }

    public override void Clear()
    {
        
    }
}
