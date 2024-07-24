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

    //실행의 순서를 지켜야 하는 묶음단위가 있으므로 각각의 델리게이트로 분리하여 관리한다.

    //처음 나타날 때 한번 해야할 일들의 묶음
    public static StartFunction ManagersStart;
    public static StartFunction ControllersStart;
    public static StartFunction ObjectsStart;

    //매 프레임마다 지속적으로 해야할 일들의 묶음
    public static UpdateFunction ManagersUpdate;
    public static UpdateFunction ControllersUpdate;
    public static UpdateFunction ObjectsUpdate;

    //매 FixedUpdate마다 지속적으로 해야할 일들의 묶음
    public static FixedUpdateFunction ManagersFixedUpdate;
    public static FixedUpdateFunction ControllersFixedUpdate;
    public static FixedUpdateFunction ObjectsFixedUpdate;

    //사라질 때 한번 해야할 일들의 묶음
    public static DestroyFunction ManagersDestroy;
    public static DestroyFunction ControllersDestroy;
    public static DestroyFunction ObjectsDestroy;

    

    /// <summary>
    /// GameManager가 생성될 때 수행할 함수. 코루틴으로 시작한다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {

        //싱글톤. 만약 원본이 없다면 저장, 있다면 본인 파괴하고 즉각 나가기.
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
