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

        SceneType = Define.EScene.GameScene;

        Managers.UI.ShowSceneUI<UI_GameScene>();

        Managers.Object.Spawn<Hero>(new Vector3(0f, -5f, 0f), Define.HERO_ZOOKEEPER_ID);
        Camera.main.GetOrAddComponent<FollowCamera>();

        //for (int i = 0; i < 5; ++i)
        //    Managers.Object.Spawn<Monster>(new Vector3(-2f + i, -1f, 0f), Define.MONSTER_SECURITY1_ID);

        //for (int i = 0; i < 5; ++i)
        //    Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 0f, 0f), Define.MONSTER_SECURITY2_ID);

        //for (int i = 0; i < 5; ++i)
        //    Managers.Object.Spawn<Monster>(new Vector3(-2f + i, 1f, 0f), Define.MONSTER_SECURITY3_ID);

        // 길리슈터 생성 테스트용. @홍지형
        // Managers.Object.Spawn<Shooter>(new Vector3(-5f, 5f, 0f), Define.MONSTER_SHOOTER_ID);

        Managers.UI.ShowBaseUI<UI_Joystick>();

        return true;
    }

    public override void Clear()
    {
        
    }
}
