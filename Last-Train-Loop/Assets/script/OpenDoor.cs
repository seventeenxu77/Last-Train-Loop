using System.Collections;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool isOpen = false;          // µ±Ç°×´Ì¬
    private Quaternion closedRot;
    private Quaternion openRot;

    void Awake()
    {
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0, 0, -90); 
    }
    public void openDoor()
    {
        StartCoroutine("doorRotate");  
        isOpen = !isOpen;
    }
    private IEnumerator doorRotate()
    {
        Quaternion start =  isOpen ? closedRot : openRot,target = isOpen ? closedRot : openRot;
        float elapsedTime = 0f;
        while (elapsedTime < 3f)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(start, target, elapsedTime /3f);
            yield return null;
        }
    }
}


