using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pinwheel.Poseidon.FX.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

public class DDDDDDD : MonoBehaviour
{
    public TextMeshProUGUI asdf;
    public PUnderwater asdfasdf;
    public PostProcessVolume qwer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        asdfasdf = qwer.profile.settings[0] as PUnderwater;
        asdf.text = asdfasdf.distortionNormalMap.value.name;
    }
}
