using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("下楼，重置到第一次循环。");
            if(LoopManager.Instance.has_exception) LoopManager.Instance.ResetLoop();
            else LoopManager.Instance.StartNewLoop();
        }
    }
}