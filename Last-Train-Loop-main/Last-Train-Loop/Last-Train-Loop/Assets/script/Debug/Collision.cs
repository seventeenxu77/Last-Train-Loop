using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Åö×²¶ÔÏó: " + hit.collider.gameObject.name);
    }
}
