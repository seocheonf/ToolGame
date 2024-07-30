using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    private ResourceEnum.Prefab myPrefab;
    public ResourceEnum.Prefab MyPrefab => myPrefab;

    /// <summary>
    /// Ǯ������ �¾ ��ü�� Prefab Ǯ�� ������ �з��� ������ �δ� �Լ�
    /// </summary>
    /// <param name="myPrefab">�ڽ��� Prefab Ǯ�� ������</param>
    public void SetInfo(ResourceEnum.Prefab myPrefab)
    {
        this.myPrefab = myPrefab;
    }
}
