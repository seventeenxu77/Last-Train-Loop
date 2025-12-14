using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("进入列车，触发下一循环。");

            if (LoopManager.Instance.has_exception) LoopManager.Instance.StartNewLoop();
            else LoopManager.Instance.ResetLoop();
        }
    }
}