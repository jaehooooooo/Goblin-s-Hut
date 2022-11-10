using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponColider : MonoBehaviour
{
    public bool isSharp;
    public float velocity;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<BreakableObject>())
        {
            BreakableObject breakableObject = other.gameObject.GetComponent<BreakableObject>();
            breakableObject.Broken(isSharp,velocity);
        }
    }
}
