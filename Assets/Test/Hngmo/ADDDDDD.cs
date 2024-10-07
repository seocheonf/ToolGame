using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ADDDDDD : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField]
    PostProcessVolume asdf;

    [SerializeField]
    public TextMeshProUGUI fda;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F10))
        {
            GameManager.Instance.SceneChange("InGameStage2");
        }
    }

}