using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Rock") || other.gameObject.tag.Contains("Electricity"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
