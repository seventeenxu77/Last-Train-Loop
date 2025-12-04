using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraSet : MonoBehaviour
{
    public Camera[] cams;   
    int idx = 0;
    private void Start()
    {
        foreach(Camera c in cams) c.enabled = false;
        cams[1].enabled = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cams[idx].enabled = false;          // 关掉旧的
            idx = (idx + 1) % cams.Length;
            cams[idx].enabled = true;           // 打开新的
        }
    }
}
