using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

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
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SkillManager _skill = new SkillManager();
    EscapePatternManager _escapePattern = new EscapePatternManager(); // @홍지형 추가.

    public static DataManager Data { get { return Instance._data; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static EscapePatternManager EscapePattern { get { return Instance._escapePattern; } }

    public static SkillManager Skill => s_instance._skill;

    #endregion

    void Start()
    {
        Init();
        StartCoroutine(s_instance._skill.CoInit());
        //TODO Eung 보스 출현 카운트
        // StartCoroutine(Game.BossCount());

        s_instance._escapePattern.SpawnEscapePattern(); // @홍지형, 테스트용
    }

    public bool boss;

    void Update()
    {
        s_instance._skill.UpdateSkillCoolTime(Time.deltaTime);


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

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
            s_instance._escapePattern.Init();
            // s_instance._spawner.Init();
        }
    }
}
