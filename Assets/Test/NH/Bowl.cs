using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour
{
    public List<Food> values;
    public int foodValues;
    public Transform raccoonDirection;

    private void Update()
    {
        foodValues = values.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Food>())
        {
            if (!values.Find((target) => target == other.GetComponent<Food>())) values.Add(other.GetComponent<Food>());

        }
        
        if (other.GetComponent<Raccoon>())
        {
            Raccoon raccoon = other.GetComponent<Raccoon>();
            values[0].gameObject.transform.rotation = Quaternion.identity;
            raccoon.PickUpTool(values[0]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Food>())
        {
            values.Remove(other.GetComponent<Food>());
        }
    }
}

