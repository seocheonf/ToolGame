using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    private ResourceEnum.Prefab myPrefab;
    public ResourceEnum.Prefab MyPrefab => myPrefab;

    /// <summary>
    /// 풀링으로 태어난 객체의 Prefab 풀링 데이터 분류를 저장해 두는 함수
    /// </summary>
    /// <param name="myPrefab">자신의 Prefab 풀링 데이터</param>
    public void SetInfo(ResourceEnum.Prefab myPrefab)
    {
        this.myPrefab = myPrefab;
    }
}
