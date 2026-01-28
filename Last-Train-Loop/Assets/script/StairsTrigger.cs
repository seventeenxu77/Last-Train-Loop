using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if (!LoopManager.Instance.isDarkLoop)
            { 
                Debug.Log("下楼，触发下一循环。");
                if (LoopManager.Instance.has_exception) LoopManager.Instance.StartNewLoop();
                else LoopManager.Instance.ResetLoop();
            }
            else
            {
                Debug.Log("黑夜关卡，需要回到起点");
            }
            
        }

    }
}