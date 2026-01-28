using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    public bool toOrigin = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!LoopManager.Instance.isDarkLoop)
            { 
                toOrigin = false;
                Debug.Log("下楼，触发下一循环。");
                if (LoopManager.Instance.has_exception) { LoopManager.Instance.StartNewLoop(); Debug.Log("调用StairsTrigger的startNew"); }
                else LoopManager.Instance.ResetLoop();
            }
            else
            {
                toOrigin = true;
                Debug.Log("黑夜关卡，需要回到起点");
            }
            
        }

    }
}