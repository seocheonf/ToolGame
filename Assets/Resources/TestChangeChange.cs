using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeChange : MonoBehaviour
{
    public List<TestCapCap> testList;
    public int index = 0;

    // Update is called once per frame
    public void VUpdate(float deltaTime)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (testList.Count <= 0)
            {
                return;
            }
            if (index >= testList.Count)
            {
                index = 0;
            }
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(testList[index], CameraViewType.FirstView);
            index++;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(testList.Count <= 0)
            {
                return;
            }
            if(index >= testList.Count)
            {
                index = 0;
            }
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(testList[index], CameraViewType.ThirdView);
            index++;
        }
    }
}
