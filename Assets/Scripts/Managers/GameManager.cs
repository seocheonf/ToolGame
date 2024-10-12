using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public delegate void StartFunction();
public delegate void UpdateFunction(float deltaTime);
public delegate void FixedUpdateFunction(float fixedDeltaTime);
public delegate void DestroyFunction();

public class GameManager : MonoBehaviour
{
    #region ��������

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

    //������ ������ ���Ѿ� �ϴ� ���������� �����Ƿ� ������ ��������Ʈ�� �и��Ͽ� �����Ѵ�.

    //ó�� ��Ÿ�� �� �ѹ� �ؾ��� �ϵ��� ����
    public static StartFunction ManagersStart;
    public static StartFunction ObjectsStart;

    //�� �����Ӹ��� ���������� �ؾ��� �ϵ��� ����
    public static UpdateFunction ManagersUpdate;
    public static UpdateFunction ObjectsUpdate;

    //�� FixedUpdate���� ���������� �ؾ��� �ϵ��� ����
    public static FixedUpdateFunction ManagersFixedUpdate;
    public static FixedUpdateFunction CharactersFixedUpdate;
    public static FixedUpdateFunction ObjectsFixedUpdate;

    //�� LateUpdate���� ���������� �ؾ��� �ϵ��� ����
    //�Ϲ������� ������ ���� �ؾ��� �ϵ��� ������.
    public static UpdateFunction ManagersLateUpdate;

    //����� �� �ѹ� �ؾ��� �ϵ��� ����
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
    /// GameManager�� ������ �� ������ �Լ�. �ڷ�ƾ���� �����Ѵ�.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
 
        #region MakeSingleton
        
        //�̱���. ���� ������ ���ٸ� ����, �ִٸ� ���� �ı��ϰ� �ﰢ ������.
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

        IsScriptEntireUpdateStop = true;

        TurnOnBasicLoadingCavnas("");

        /* 
         * 
         * ���� �ܰ��� �Ŵ����� ������ �ʿ�� �ϰų� �ƴ� ���� �ִ�.
         * 
         * 1. Resource�� �غ��Ѵ�. (�⺻ ���� �غ�)
         * 2. Sound ������(ó�� / ���� ���� ���)�� �غ��Ѵ�. (�⺻ ���� �غ�)
         * 3. �Է� ó���� �غ��Ѵ�. (�⺻ ���� �غ�)
         * 4. UI �ý����� �غ��Ѵ�.
         * 5. ����Ǿ� �ִ� ������ �ҷ��´�.
         * 6. ���� ������ �������� option�� �����Ѵ�. 
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

        DoCompletelyStartFunction(ref ManagersStart);

        TurnOffBasicLoadingCanvas();

        IsScriptEntireUpdateStop = false;

    }


    private void DoCompletelyStartFunction(ref StartFunction target)
    {
        StartFunction runTarget = target;
        target = null;
        runTarget?.Invoke();
    }

    private void Update()
    {
        
        if (isScriptEntireUpdateStop) return;

        if(ManagersStart != null)
        {
            DoCompletelyStartFunction(ref ManagersStart);
        }
        else
        {
            if (currentWorld == null || !currentWorld.WorldAlive)
                return;

            DoCompletelyStartFunction(ref ObjectsStart);

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
            DoCompletelyStartFunction(ref ManagersStart);
        }
        else
        {
            if (currentWorld == null || !currentWorld.WorldAlive)
                return;

            DoCompletelyStartFunction(ref ObjectsStart);

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

        if (currentWorld == null)
            return;

        if (!isScriptManagersUpdateStop) ManagersLateUpdate?.Invoke(Time.deltaTime);        
    }

    private static int basicLoadingCanvasCount = 0;

    /// <summary>
    /// �⺻ �ε� ĵ������ ������ �Բ� ��Ÿ����. GameManager ��ũ��Ʈ���� Update�� ������Ų��.
    /// </summary>
    /// <param name="info"> �⺻ �ε� ĵ���� ���� </param>
    public static void TurnOnBasicLoadingCavnas(string info)
    {
        basicLoadingCanvasCount += 1;
        instance.basicLoadingCanvas.gameObject.SetActive(true);
        instance.basicLoadingCanvas.SetInfo(info);
        //instance.isScriptEntireUpdateStop = true;
        instance.isScriptManagersUpdateStop = true;
        instance.isScriptObjectsUpdateStop = true;
    }
    
    /// <summary>
    /// �⺻ �ε� ĵ������ �ݴ´�. GameManager ��ũ��Ʈ���� Update�� �����Ų��.
    /// </summary>
    public static void TurnOffBasicLoadingCanvas()
    {
        basicLoadingCanvasCount -= 1;
        if(basicLoadingCanvasCount == 0)
        {
            basicLoadingCanvasCount = 0;
            instance.basicLoadingCanvas.gameObject.SetActive(false);
            //instance.isScriptEntireUpdateStop = false;
            instance.isScriptManagersUpdateStop = false;
            instance.isScriptObjectsUpdateStop = false;
        }
        else if(basicLoadingCanvasCount < 0)
        {
            Debug.LogError("������ �ε� ĵ������ ���� ī��Ʈ�� ��������!");
        }
        
    }

    /// <summary>
    /// �� ��ȯ �Լ�
    /// </summary>
    /// <param name="sceneName">�� �̸�</param>
    public void SceneChange(string sceneName)
    {
        WorldDelete();
        StartCoroutine(AsyncSceneChange(sceneName));
        //TurnOnBasicLoadingCavnas("Scene Change");
        //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        //TurnOffBasicLoadingCanvas();
    }
    private IEnumerator AsyncSceneChange(string sceneName)
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName);
        TurnOnBasicLoadingCavnas("Scene Change...");
        while(!sceneLoading.isDone)
        {
            yield return null;
        }
        yield return new WaitUntil(() => currentWorld != null);
        TurnOffBasicLoadingCanvas();
    }

    /// <summary>
    /// �� ��ȯ �Լ�
    /// </summary>
    /// <param name="sceneBuildIndex">���� ����� Index</param>
    public void SceneChange(int sceneBuildIndex)
    {
        WorldDelete();
        StartCoroutine(AsyncSceneChange(sceneBuildIndex));
        //TurnOnBasicLoadingCavnas("Scene Change");
        //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex);
        //TurnOffBasicLoadingCanvas();
    }
    private IEnumerator AsyncSceneChange(int sceneBuildIndex)
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneBuildIndex);

        TurnOnBasicLoadingCavnas("Scene Change...");
        while (!sceneLoading.isDone)
        {
            yield return null;
        }
        yield return new WaitUntil(() => currentWorld != null);
        TurnOffBasicLoadingCanvas();
    }

    /// <summary>
    /// ���ο� World�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="newWorld">���ο� World ����</param>
    public void SetCurrentWorld(WorldManager newWorld)
    {
        if(currentWorld != null)
        {
            WorldDelete();
        }
        currentWorld = newWorld;
    }
    /// <summary>
    /// currentWorld�� ����� ���� ó���� �Բ� �ϴ� �Լ�
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
