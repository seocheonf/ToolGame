using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    //�� ���尡 ������ ī�޶� ����
    private CameraManager worldCamera;
    public CameraManager WorldCamera => worldCamera;

    //�� ���尡 ������ Ǯ�� ����
    private PoolManager pool;
    public PoolManager Pool => pool;

    //�ν�����â���� �̸� ������ Ǯ�� ������ ������
    [SerializeField]
    private List<PrefabPool> prefabPoolList;

    //�� ���尡 Update�󿡼� �ؾ��� �ϵ��� ����
    //���߿� �߰��� ���带 �������� ��, �Ѳ����� �ִ��� �� �� �� �ְ� ���ʿ� �̸� ���ؼ� �ֵ��� ��.
    public UpdateFunction WorldUpdates;
    public FixedUpdateFunction WorldFixedUpdates;


    /// <summary>
    /// �� ������ ������Ʈ�� �ѹ��� ���� ���ؼ�, �� ������ ������Ʈ�� �����ϴ� �Լ��� �ѹ� ���� ��, �̸� GameManager�� ������Ʈ�� ����.
    /// </summary>
    /// <param name="deltaTime">1������ �ð�</param>
    private void InnerWorldUpdates(float deltaTime)
    {
        WorldUpdates?.Invoke(deltaTime);
    }
    /// <summary>
    /// �� ������ Fixed ������Ʈ�� �ѹ��� ���� ���ؼ�, �� ������ Fixed ������Ʈ�� �����ϴ� �Լ��� �ѹ� ���� ��, �̸� GameManager�� Fixed ������Ʈ�� ����.
    /// </summary>
    /// <param name="fixedDeltaTime">1Fixed������ �ð�</param>
    private void InnerWorldFixedUpdates(float fixedDeltaTime)
    {
        WorldFixedUpdates?.Invoke(fixedDeltaTime);
    }

    /// <summary>
    /// WorldManager�� ó�� ������ ��, ���� �Ŵ����� ����ϰ� �ڽ��� ������ �� �� ���� ���� �Ŵ��� Update�� �����Լ� ��������Ʈ�� ����ϴ� �Լ�. ���� �̷� ���� �۾� Ÿ�̹��� �����ϱ� ����(������ ���� ������ ����)
    /// </summary>
    /// <returns>�ڷ�ƾ��</returns>
    private IEnumerator Start()
    {
#if UNITY_EDITOR
        //������ �󿡼� ���� ���Ǹ� ����, ���� �Ŵ����� ������ ���� ������ �̵��ϴ� ���
        //���� �󿡼� �� ��Ȳ ��ü�� �����̱⿡ (���� ���� ���� �� ������, ���� �� ��ġ�⵵ ����, �����ϰ� ���� ���� ����)
        if (GameManager.Instance == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            yield break;
        }
#endif

        //GameManager�� WorldManager�� �ϳ��� ���� ��� ������ ���� �ʿ���(���ǹ���) ���
        //���� ó��
        yield return new WaitUntil(() => { return GameManager.Instance != null; });

        //������ �¾�� �� �ؾ��� �ϵ��� ���� �Ŵ����� �����Ͽ� �̷� ����
        GameManager.ManagersStart += WorldManagerStart;

    }
    
    /// <summary>
    /// ���� �Ŵ����� �¾�� �� �� ��
    /// </summary>
    private void WorldManagerStart()
    {
        StartCoroutine(Initiate());
    }

    /// <summary>
    /// ���� �Ŵ����� �ʱ�ȭ�ϸ鼭 �ؾ��� ��
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Initiate()
    {
        GameManager.TurnOnBasicLoadingCavnas("World Loading...");

        //�켱 �ֹε�� �Ű�����.
        GameManager.Instance.SetCurrentWorld(this);

        pool = new PoolManager();
        yield return pool.Initiate();
        yield return pool.SetPool(prefabPoolList);

        worldCamera = new CameraManager();
        yield return worldCamera.Initiate();

        //������Ʈ ���
        GameManager.ManagersUpdate -= InnerWorldUpdates;
        GameManager.ManagersUpdate += InnerWorldUpdates;
        GameManager.ManagersFixedUpdate -= InnerWorldFixedUpdates;
        GameManager.ManagersFixedUpdate += InnerWorldFixedUpdates;

        //�� ���� �Ŵ������� �Ϸ� �� �� �ϵ��� ����.
        GameManager.TurnOffBasicLoadingCanvas();
    }

    /// <summary>
    /// ���� �Ŵ����� ���� �� �ؾ��� ��
    /// </summary>
    public virtual void WorldManagerDestroy()
    {
        //������Ʈ ��ü
        GameManager.ManagersUpdate -= InnerWorldUpdates;
        GameManager.ManagersFixedUpdate -= InnerWorldFixedUpdates;
    }

}