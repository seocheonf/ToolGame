using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void StartFunction();
public delegate void UpdateFunction(float deltaTime);
public delegate void FixedUpdateFunction(float fixedDeltaTime);
public delegate void DestroyFunction();

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance => Instance;

    ControllerManager controller;
    public ControllerManager Controller => controller;

    PoolManager pool;
    public PoolManager Pool => pool;

    WorldManager currentWorld;
    public WorldManager CurrentWorld => currentWorld;

    CameraManager camera;
    public CameraManager Camera => camera;

    OptionManager option;
    public OptionManager Option => option;

    ResourceManager resource;
    public ResourceManager Resource => resource;

    SaveManager save;
    public SaveManager Save => save;

    SoundManager sound;
    public SoundManager Sound => sound;

    UIManager ui;
    public UIManager UI => ui;

    //������ ������ ���Ѿ� �ϴ� ���������� �����Ƿ� ������ ��������Ʈ�� �и��Ͽ� �����Ѵ�.

    //ó�� ��Ÿ�� �� �ѹ� �ؾ��� �ϵ��� ����
    public static StartFunction ManagersStart;
    public static StartFunction ControllersStart;
    public static StartFunction ObjectsStart;

    //�� �����Ӹ��� ���������� �ؾ��� �ϵ��� ����
    public static UpdateFunction ManagersUpdate;
    public static UpdateFunction ControllersUpdate;
    public static UpdateFunction ObjectsUpdate;

    //�� FixedUpdate���� ���������� �ؾ��� �ϵ��� ����
    public static FixedUpdateFunction ManagersFixedUpdate;
    public static FixedUpdateFunction ControllersFixedUpdate;
    public static FixedUpdateFunction ObjectsFixedUpdate;

    //����� �� �ѹ� �ؾ��� �ϵ��� ����
    public static DestroyFunction ManagersDestroy;
    public static DestroyFunction ControllersDestroy;
    public static DestroyFunction ObjectsDestroy;

    

    /// <summary>
    /// GameManager�� ������ �� ������ �Լ�. �ڷ�ƾ���� �����Ѵ�.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {

        //�̱���. ���� ������ ���ٸ� ����, �ִٸ� ���� �ı��ϰ� �ﰢ ������.
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            yield break;
        }

        controller = new ControllerManager();
        yield return controller.Initiate();


        ManagersUpdate += controller.ManagerUpdate;


        yield return null;
    }

    private void Update()
    {
        
    }

}
