using UnityEngine;

public class DarkLoopTextTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")||!LoopManager.Instance.isDarkLoop) return;
        bool passBy = GameObject.Find("stairbox").GetComponent<StairsTrigger>().toOrigin;
        if (!passBy) return; 
        passBy = false;
        Debug.Log("µ÷ÓÃDarkLoopTextTriggerµÄstartNewLoop");
        LoopManager.Instance.StartNewLoop();
    }
}
