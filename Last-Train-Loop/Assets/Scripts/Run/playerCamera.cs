using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCamera : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        transform.position = target.position;
    }
    void Update()
    {
        transform .position = target.position;
        transform.LookAt(target.position + target.forward);
    }

}
