using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class test : MonoBehaviour
{
    public MeshCollider target;
    public TextMeshProUGUI textTa;
    public int asdf;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //textTa.text = $"{target.isTrigger} / {target.enabled} / {target.material} / {target.convex} / {target.sharedMesh} / {target.contactOffset}";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.AddForce(Vector3.up * asdf);
        }
    }
}
