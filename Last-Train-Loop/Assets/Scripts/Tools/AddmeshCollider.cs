using UnityEngine;
using UnityEditor;

public class AddMeshCollidersEditor : EditorWindow
{
    [MenuItem("Tools/为选中物体及后代批量添加 MeshCollider")]
    static void OpenWin() => GetWindow<AddMeshCollidersEditor>("批量加 MeshCollider");

    bool onlyIfMissing = true;   // 仅当没有 Collider 时才加
    bool convex = false;    // 是否需要 Convex（按需勾选）

    void OnGUI()
    {
        GUILayout.Label("给所有带 MeshFilter 的子/孙物体加 MeshCollider");
        onlyIfMissing = GUILayout.Toggle(onlyIfMissing, "仅缺少 Collider 的才添加");
        convex = GUILayout.Toggle(convex, "Convex（需与 RigidBody 碰撞时勾选）");

        if (GUILayout.Button("执行"))
        {
            foreach (GameObject go in Selection.gameObjects)
                DoIt(go);
        }
    }

    void DoIt(GameObject root)
    {
        // 递归遍历所有后代
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            GameObject go = t.gameObject;
            if (go.TryGetComponent(out MeshFilter mf) && mf.sharedMesh != null)
            {
                if (onlyIfMissing && go.TryGetComponent(out Collider _))
                    continue;          // 已存在任意 Collider 就跳过

                var mc = go.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                mc.convex = convex;
            }
        }
        Debug.Log("完成！");
    }
}
