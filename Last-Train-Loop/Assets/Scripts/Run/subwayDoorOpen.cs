using System.Collections;
using UnityEngine;

public class subwayDoorOpen : MonoBehaviour
{
    [Tooltip("设置自动关门时间")]
    public float delaySeconds = 2f;
    [Header("对应地铁门，方便拖动别修改")]
    [SerializeField] private Transform leftdoor, rightdoor;
    
    private bool isOpen = false;         
    private bool cursorOnDoor = false;
    private Vector3 deltaVector = new Vector3(0, 0, 1);
    private Vector3 lstartposition, lendposition, rstartposition, rendposition;

    private Coroutine delay;
    private void Start()
    {
        lstartposition = leftdoor.position;
        lendposition = leftdoor.position - deltaVector;
        rstartposition = rightdoor.position;
        rendposition = rightdoor.position + deltaVector;
    }
    void OnMouseEnter() {
        lstartposition = leftdoor.position;
        lendposition = leftdoor.position - deltaVector;
        rstartposition = rightdoor.position;
        rendposition = rightdoor.position + deltaVector; cursorOnDoor = true; Debug.Log("鼠标悬停");
    }
    void OnMouseExit() { cursorOnDoor = false; Debug.Log("鼠标移走"); }

    void OnMouseDown()
    {
        if (cursorOnDoor)
        {
           /* if (delay != null)
                StopCoroutine(delay);*/
            if (!isOpen)
            {
                rightdoor.position = Vector3.Lerp(rstartposition, rendposition, 1f);
                leftdoor.position = Vector3.Lerp(lstartposition,lendposition,1f);
                delay = StartCoroutine(CloseAfterDelay());
            }
            isOpen = !isOpen;
        }
    }
    private IEnumerator CloseAfterDelay()
    {
        
        yield return new WaitForSeconds(delaySeconds);
        rightdoor.position = Vector3.Lerp(rendposition, rstartposition, 1f);
        leftdoor.position = Vector3.Lerp(lendposition, lstartposition, 1f);
        isOpen = false;
        delay = null;
    }
}
 
    

