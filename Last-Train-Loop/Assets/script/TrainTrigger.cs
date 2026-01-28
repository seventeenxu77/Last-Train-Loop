using UnityEngine;

public class TrainTrigger : MonoBehaviour
{
    public void TrainJudge()
    {
        GameObject.Find("stairbox").GetComponent<StairsTrigger>().toOrigin = false;
        Debug.Log("进入列车，触发下一循环");
        if (!LoopManager.Instance.has_exception) { Debug.Log("调用TrainTrigger的startNew"); LoopManager.Instance.StartNewLoop(); }
        else LoopManager.Instance.ResetLoop();

    }
}