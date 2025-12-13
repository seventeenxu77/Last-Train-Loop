using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToleft : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("Êó±êµ¥»÷");
        SubwayRun s = GetComponentInParent<SubwayRun>();
        LoopManager.Instance.enter_train = true;
        if (s != null) s.left();
    }

}
