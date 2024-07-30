using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    //�� ���尡 ������ Ǯ�� ����
    private PoolManager pool;
    public PoolManager Pool => pool;

    //�ν�����â���� �̸� ������ Ǯ�� ������ ������
    [SerializeField]
    private List<PrefabPool> prefabPoolList = new List<PrefabPool>();

    //�� ���尡 Update�󿡼� �ؾ��� �ϵ��� ����
    //���߿� �߰��� ���带 �������� ��, �Ѳ����� �ִ��� �� �� �� �ְ� ���ʿ� �̸� ���ؼ� �ֵ��� ��.
    private UpdateFunction WorldUpdates;


    protected virtual IEnumerator Start()
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
    protected virtual void WorldManagerStart()
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

        pool = new PoolManager();
        yield return pool.Initiate();
        yield return pool.SetPool(prefabPoolList);

        //�غ� �Ϸ�Ǿ����� �ֹε�� �Ű�����.
        GameManager.Instance.SetCurrentWorld(this);

        GameManager.TurnOffBasicLoadingCanvas();
    }

    /// <summary>
    /// ���� �Ŵ����� ���� �� �ؾ��� ��
    /// </summary>
    public virtual void WorldManagerDestroy()
    {
        GameManager.ManagersUpdate -= WorldUpdates;
    }

}