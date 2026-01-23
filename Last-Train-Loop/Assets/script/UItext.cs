using System.Collections;
using UnityEngine;
using TMPro;

public class UItext : MonoBehaviour
{
    // 在Inspector中拖拽UI文本组件
    [SerializeField] private GameObject textUI;
    // 显示时长（秒）
    [SerializeField] private float displayDuration = 3f;

    private void Awake()
    {
        textUI.SetActive(false);   
    }
    void OnTriggerEnter(Collider other)
    {
        int index = LoopManager.Instance.currentLoopIndex;
        if (other.CompareTag("Player"))
        {
            // 激活文本
            TextMeshProUGUI txt = textUI.GetComponent<TextMeshProUGUI>();
            txt.text = $"11:{(index + 1) * 10}";
            textUI.SetActive(true);

            // 如果是暂时显示
            if (displayDuration > 0)
            {
                StartCoroutine(HideTextAfterDelay());
            }
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        textUI.SetActive(false);
    }
}
