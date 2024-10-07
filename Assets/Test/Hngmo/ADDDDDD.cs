using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;


public class ADDDDDD : MonoBehaviour
{

    [SerializeField]
    PostProcessVolume asdf;

    [SerializeField]
    public TextMeshProUGUI fda;

    bool tri = false;

    void Update()
    {
        if(Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.R) && tri == false)
        {
            GameManager.Instance.SceneChange(SceneManager.GetActiveScene().name);
            tri = true;
            StartCoroutine(checkTime());
        }
    }


    IEnumerator checkTime()
    {
        yield return new WaitForSeconds(5f);
        tri = false;
    }
}