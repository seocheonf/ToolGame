using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class test : MonoBehaviour
{
    public MeshCollider target;
    public TextMeshProUGUI textTa;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //textTa.text = $"{target.isTrigger} / {target.enabled} / {target.material} / {target.convex} / {target.sharedMesh} / {target.contactOffset}";
    }
}
