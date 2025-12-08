using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private Rigidbody body;
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        body = GetComponent<Rigidbody>();
        Debug.Log("·¢ÉúÅö×²");
    }
    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        Debug.Log("Åö×²½áÊø");
    }
}
