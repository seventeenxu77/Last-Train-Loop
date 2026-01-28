using UnityEngine;
public class CameraSet : MonoBehaviour
{
    [SerializeField]public Camera[] cams;   
    int idx = 0;
    public bool isUsingMain;
    private void Start()
    {
        cams[1].enabled = false;
        Camera.main.enabled = true;
        isUsingMain = true;
    }
    void Update()
    {
        if (LoopManager.Instance.isDarkLoop)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                cams[idx].enabled = false;
                idx = (idx + 1) % cams.Length;
                cams[idx].enabled = true;
                isUsingMain = !isUsingMain;
            }
        }
    }
}
