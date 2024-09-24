using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void StartFunction();
public delegate void UpdateFunction(float deltaTime);
public delegate void FixedUpdateFunction(float fixedDeltaTime);
public delegate void DestroyFunction();

public class GameManager : MonoBehaviour
{
    #region 변수선언

    private static GameManager instance;
    public static GameManager Instance => instance;

    private ControllerManager controller;
    public ControllerManager Controller => controller;

    private WorldManager currentWorld;
    public WorldManager CurrentWorld => currentWorld;

    private OptionManager option;
    public OptionManager Option => option;

    private ResourceManager resource;
    public ResourceManager Resource => resource;

    private SaveManager save;
    public SaveManager Save => save;

    private SoundManager sound;
    public SoundManager Sound => sound;

    private UIManager ui;
    public UIManager UI => ui;

    //실행의 순서를 지켜야 하는 묶음단위가 있으므로 각각의 델리게이트로 분리하여 관리한다.

    //처음 나타날 때 한번 해야할 일들의 묶음
    public static StartFunction ManagersStart
    {
        get
        {

            while (ManagersStart_A != null || ManagersStart_B != null)
            {
                //양쪽 중 하나라도 비어있지 않다면

                if (managersStart_Running)
                {
                    ManagersStart_A?.Invoke();
                    ManagersStart_A = null;
                }
                else
                {
                    ManagersStart_B?.Invoke();
                    ManagersStart_B = null;
                }

                managersStart_Running = !managersStart_Running;

            }
            //만약 양쪽 다 비었다면 그만 실행.
            return null;

        }
        set
        {
            if(value == null)
            {
                ManagersStart_A = null;
                ManagersStart_B = null;
            }

            if (!managersStart_Running)
                ManagersStart_A = value;
            else
                ManagersStart_B = value;
        }
    }
    private static bool managersStart_Running = true; // get 했을 시 현재 실행해야 할 트리거 [true : A, false : B]. set은 그 반대
    private static StartFunction ManagersStart_A;
    private static StartFunction ManagersStart_B;

    public static StartFunction ObjectsStart
    {
        get;
        set;
    }
    private static StartFunction ObjectsStart_A;
    private static StartFunction ObjectsStart_B;

    //매 프레임마다 지속적으로 해야할 일들의 묶음
    public static UpdateFunction ManagersUpdate;
    public static UpdateFunction ObjectsUpdate;

    //매 FixedUpdate마다 지속적으로 해야할 일들의 묶음
    public static FixedUpdateFunction ManagersFixedUpdate;
    public static FixedUpdateFunction CharactersFixedUpdate;
    public static FixedUpdateFunction ObjectsFixedUpdate;

    //매 LateUpdate마다 지속적으로 해야할 일들의 묶음
    //일반적으로 렌더링 직전 해야할 일들을 정의함.
    public static UpdateFunction ManagersLateUpdate;

    //사라질 때 한번 해야할 일들의 묶음
    public static DestroyFunction ManagersDestroy;
    public static DestroyFunction ObjectsDestroy;

    [SerializeField]
    private BasicLoadingCanvas basicLoadingCanvas;

    private bool isScriptEntireUpdateStop = true;
    public bool IsScriptEntireUpdateStop
    {
        get
        {
            return isScriptEntireUpdateStop;
        }
        set
        {
            isScriptEntireUpdateStop = value;
        }
    }

    private bool isScriptManagersUpdateStop = false;
    public bool IsScriptManagersUpdateStop
    {
        get
        {
            return isScriptManagersUpdateStop;
        }
        set
        {
            isScriptManagersUpdateStop = value;
        }
    }

    private bool isScriptObjectsUpdateStop = false;
    public bool IsScriptObjectsUpdateStop
    {
        get
        {
            return isScriptObjectsUpdateStop;
        }
        set
        {
            isScriptObjectsUpdateStop = value;
        }
    }

    #endregion

    /// <summary>
    /// GameManager가 생성될 때 수행할 함수. 코루틴으로 시작한다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
 
        #region MakeSingleton
        
        //싱글톤. 만약 원본이 없다면 저장, 있다면 본인 파괴하고 즉각 나가기.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            yield break;
        }
        
        DontDestroyOnLoad(gameObject);

        #endregion

        TurnOnBasicLoadingCavnas("");

        /* 
         * 
         * 이전 단계의 매니저의 정보를 필요로 하거나 아닐 수도 있다.
         * 
         * 1. Resource를 준비한다. (기본 바탕 준비)
         * 2. Sound 관리부(처리 / 송출 등을 담당)를 준비한다. (기본 바탕 준비)
         * 3. 입력 처리를 준비한다. (기본 바탕 준비)
         * 4. UI 시스템을 준비한다.
         * 5. 저장되어 있던 정보를 불러온다.
         * 6. 저장 정보를 바탕으로 option을 설정한다. 
         * 
         */

        resource = new ResourceManager();
        yield return resource.Initiate();

        sound = new SoundManager();
        yield return sound.Initiate();

        controller = new ControllerManager();
        yield return controller.Initiate();

        ui = new UIManager();
        yield return ui.Initiate();

        save = new SaveManager();
        yield return save.Initiate();

        option = new OptionManager();
        yield return option.Initiate();

        ManagersUpdate += controller.ManagerUpdate;
        ManagersUpdate += ui.ManagerUpdate;
        ManagersUpdate += sound.ManagerUpdate;

        TurnOffBasicLoadingCanvas();

    }


    private void Update()
    {

        if (isScriptEntireUpdateStop) return;

        if(ManagersStart != null)
        {
            ManagersStart.Invoke();
            ManagersStart = null;
        }
        else
        {
            ObjectsStart?.Invoke();
            ObjectsStart = null;

            if (!isScriptManagersUpdateStop) ManagersUpdate?.Invoke(Time.deltaTime);
            if (!isScriptObjectsUpdateStop) ObjectsUpdate?.Invoke(Time.deltaTime);
        }

        ObjectsDestroy?.Invoke();
        ObjectsDestroy = null;
        ManagersDestroy?.Invoke();
        ManagersDestroy = null;
    }

    private void FixedUpdate()
    {
        if (isScriptEntireUpdateStop) return;

        if (ManagersStart != null)
        {
            ManagersStart.Invoke();
            ManagersStart = null;
        }
        else
        {
            
            ObjectsStart?.Invoke();
            ObjectsStart = null;

            if (!isScriptObjectsUpdateStop)
            {
                CharactersFixedUpdate?.Invoke(Time.fixedDeltaTime);
                ObjectsFixedUpdate?.Invoke(Time.fixedDeltaTime);
            }
            if (!isScriptManagersUpdateStop) ManagersFixedUpdate?.Invoke(Time.fixedDeltaTime);
        }

        ObjectsDestroy?.Invoke();
        ObjectsDestroy = null;
        ManagersDestroy?.Invoke();
        ManagersDestroy = null;
    }

    private void LateUpdate()
    {
        if (isScriptEntireUpdateStop) return;

        if (!isScriptManagersUpdateStop) ManagersLateUpdate?.Invoke(Time.deltaTime);        
    }

    /// <summary>
    /// 기본 로딩 캔버스를 문구와 함께 나타낸다. GameManager 스크립트상의 Update를 정지시킨다.
    /// </summary>
    /// <param name="info"> 기본 로딩 캔버스 문구 </param>
    public static void TurnOnBasicLoadingCavnas(string info)
    {
        instance.basicLoadingCanvas.gameObject.SetActive(true);
        instance.basicLoadingCanvas.SetInfo(info);
        instance.isScriptEntireUpdateStop = true;
    }
    
    /// <summary>
    /// 기본 로딩 캔버스를 닫는다. GameManager 스크립트상의 Update를 진행시킨다.
    /// </summary>
    public static void TurnOffBasicLoadingCanvas()
    {
        instance.basicLoadingCanvas.gameObject.SetActive(false);
        instance.isScriptEntireUpdateStop = false;
    }

    /// <summary>
    /// 씬 전환 함수
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    public void SceneChange(string sceneName)
    {
        WorldDelete();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    /// <summary>
    /// 씬 전환 함수
    /// </summary>
    /// <param name="sceneBuildIndex">씬의 빌드상 Index</param>
    public void SceneChange(int sceneBuildIndex)
    {
        WorldDelete();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex);
    }
    /// <summary>
    /// 새로운 World를 적용하는 함수
    /// </summary>
    /// <param name="newWorld">새로운 World 정보</param>
    public void SetCurrentWorld(WorldManager newWorld)
    {
        if(currentWorld != null)
        {
            WorldDelete();
        }
        currentWorld = newWorld;
    }
    /// <summary>
    /// currentWorld를 지우며 관련 처리를 함께 하는 함수
    /// </summary>
    private void WorldDelete()
    {
        if (currentWorld != null)
        {
            currentWorld.WorldManagerDestroy();
            Destroy(currentWorld.gameObject);
        }
        currentWorld = null;
    }
}
