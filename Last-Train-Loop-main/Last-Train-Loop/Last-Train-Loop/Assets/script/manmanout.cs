using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class manymanout : MonoBehaviour
{
    [Header("角色管理")] // <-- 【新增】角色列表
    public List<GameObject> allCharacters; // 儲存所有需要統一控制的角色
    void OnTriggerEnter(Collider other)
    {
            SetAllCharactersActive(false);
    }
    public void SetAllCharactersActive(bool shouldActivate)
    {
        if (allCharacters == null || allCharacters.Count == 0)
        {
            Debug.LogWarning("角色列表 (allCharacters) 為空，沒有角色可控制。");
            return;
        }

        foreach (GameObject character in allCharacters)
        {
            if (character != null)
            {
                character.SetActive(shouldActivate);
            }
        }

        Debug.Log($"所有角色已設置為 {(shouldActivate ? "顯示" : "隱藏")}");
    }
}
