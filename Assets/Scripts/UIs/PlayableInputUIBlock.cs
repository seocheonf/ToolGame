using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayableInputUIBlock : FixedUIComponent
{

    [SerializeField]
    TextMeshProUGUI key;
    [SerializeField]
    TextMeshProUGUI description;

    public void SetInfo(string key, string description)
    {
        this.key.text = key;
        this.description.text = description;
    }

}
