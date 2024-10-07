using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorld : WorldManager
{
    public TestChangeChange testChangeChange;
    protected override IEnumerator RemainInitiate()
    {
        yield return base.RemainInitiate();

        WorldUpdates += testChangeChange.VUpdate;

        Vector3 a = new Vector3(1, 0, 0);
        for (int i = 0; i < 10; i++)
        {
            testChangeChange.testList.Add(PoolManager.TakeStock(ResourceEnum.Prefab.CapCap, a).GetComponent<TestCapCap>());
            a.x *= -1;
            a.y += 3;
        }
    }

    //protected override IEnumerator Initiate()
    //{
    //    yield return base.Initiate();

    //    WorldUpdates += testChangeChange.VUpdate;

    //    Vector3 a = new Vector3(1, 0, 0);
    //    for (int i = 0; i < 10; i++)
    //    {
    //        testChangeChange.testList.Add(PoolManager.TakeStock(ResourceEnum.Prefab.CapCap, a).GetComponent<TestCapCap>());
    //        a.x *= -1;
    //        a.y += 3;
    //    }

    //    GameManager.TurnOffBasicLoadingCanvas();
    //}
}
