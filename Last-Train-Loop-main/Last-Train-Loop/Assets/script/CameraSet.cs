using UnityEngine;
public class CameraSet : MonoBehaviour
{
    [SerializeField]public Camera[] cams;   
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
            if (Input.GetKey(KeyCode.F))
            {
                cams[0].enabled = false;
                isUsingMain = false;
                cams[1].enabled = true;
                
            }
            else
            {
                cams[0].enabled = true;
                isUsingMain = true;
                cams[1].enabled = false;
            }
        }
        else
        {
            cams[0].enabled = true;
            isUsingMain = true;
            cams[1].enabled = false;
        }
    }
}
