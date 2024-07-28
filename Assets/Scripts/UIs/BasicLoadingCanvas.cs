using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasicLoadingCanvas : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI loadingTextInfo;
    public void SetInfo(string loadingTextInfo)
    {
        this.loadingTextInfo.text = loadingTextInfo;
    }

}
