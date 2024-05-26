using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : SingletonMonoBehaviour<Managers>
{
    #region Contents
    private GameManager _game = new GameManager();
    private ObjectManager _object = new ObjectManager();
    private SpawnManager _spawner = new SpawnManager();

    public static GameManager Game { get { return Instance._game; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    public static SpawnManager Spawner { get { return Instance?._spawner; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    //ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SkillManager _skill = new SkillManager();
    //EscapePatternManager _escapePattern = new EscapePatternManager(); // @홍지형 추가.
    LocalizationManager _localizationManager = new LocalizationManager();

    public static DataManager Data { get { return DataManager.Instance; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return ResourceManager.Instance; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static EscapePatternManager EscapePattern { get { return EscapePatternManager.Instance; } }

    public static LocalizationManager Localization { get { return Instance._localizationManager; } }

    public static SkillManager Skill => Instance._skill;


    #endregion

    
    public bool isTest = false;
    public bool isTestScene = false;

    IEnumerator Start()
    {
        yield return null;

        if (!isTestScene)
        {
            SceneManagerNew.Instance.LoadScene(Define.EScene.TitleScene);
            //SceneManagerNew.Instance.LoadScene("TitleScene");

            StartCoroutine(Instance._skill.CoInit());


            //SceneManagerNew.Instance.LoadScene(Define.EScene.TitleScene);

            //TODO Eung 보스 출현 카운트
            // StartCoroutine(Game.BossCount());

            //EscapePattern.SpawnEscapePattern(); // @홍지형, 테스트용
        }

    }

    public bool boss;

    void Update()
    {
        Instance._skill.UpdateSkillCoolTime(Time.deltaTime);


        // if (!boss && Game.CurrentTime >= 5f)
        // {
        // 	boss = !boss;
        // 	UI.ShowPopupUI<UI_Warning>();
        // }
        // if (!boss && Game.CurrentTime >= 10f)
        // {
        // 	boss = !boss;
        // 	UI.ShowPopupUI<UI_Warning>();
        // }
    }

    protected override void Init()
    {
        //Instance._data.Init();
        //Instance._pool.Init();
        //Instance._sound.Init();
        // s_instance._spawner.Init();
    }

}
