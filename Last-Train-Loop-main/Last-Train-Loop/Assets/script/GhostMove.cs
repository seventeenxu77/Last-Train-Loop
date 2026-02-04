using System.Collections;
using UnityEngine;

public class GhostMove : MonoBehaviour
{
    [Header("位置与高度设置")]
    public Vector3 ghostBornPlace;
    public float yOffset = 1.0f;
    public float followDistance = 2.0f;
    public float moveSpeed = 3.0f;
    public float rotateSpeed = 10.0f;

    [Header("组件引用")]
    public Light checkLight;
    public CameraSet caScript;
    public Animator anim;            
    public AudioSource footstepAudio; 

    private GameObject player;
    private bool ableToMove, isCounting, showUp;
    private double nextCheckTime = 0;
    private float checkInterval = 0.1f; 
    private Coroutine cor;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ableToMove = true;
        isCounting = false;
        showUp = false;
        
        if (footstepAudio != null) footstepAudio.Stop();
    }

    void FixedUpdate()
    {
        if (Time.time >= nextCheckTime)
        {
            CheckIfInLight();
            nextCheckTime = Time.time + (double)checkInterval;
        }

        bool shouldMove = showUp && ableToMove && !isCounting;

        if (shouldMove)
        {
            Move();
            UpdateVisuals(true);
        }
        else
        {
            UpdateVisuals(false);
        }
    }

    void UpdateVisuals(bool isWalking)
    {
        if (anim != null)
        {
            anim.speed = isWalking ? 1.0f : 0.0f;
        }

        if (footstepAudio != null)
        {
            if (isWalking && !footstepAudio.isPlaying)
                footstepAudio.Play();
            else if (!isWalking && footstepAudio.isPlaying)
                footstepAudio.Pause();
        }
    }

    public void Summon()
    {
        Invoke("GhostShowUp", 1f);
    }

    void GhostShowUp()
    {
        transform.position = ghostBornPlace + Vector3.up * yOffset;
        LookAtPlayerHorizontal();
        transform.gameObject.SetActive(true);
        showUp = true;
    }

    void Move()
    {
        if (player == null) return;

        Vector3 ghostPos = transform.position;
        Vector3 playerPos = player.transform.position;
        Vector3 directionToPlayer = new Vector3(playerPos.x - ghostPos.x, 0, playerPos.z - ghostPos.z).normalized;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        float currentGroundY = ghostPos.y - yOffset;
        RaycastHit groundHit;
        // 探测地面时排除怪物自己（包括子物体）
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, Vector3.down, out groundHit, 5.0f))
        {
            if (groundHit.transform != transform && !groundHit.transform.IsChildOf(transform))
            {
                currentGroundY = groundHit.point.y;
            }
        }

        float targetY = currentGroundY + yOffset;
        float smoothedY = Mathf.Lerp(ghostPos.y, targetY, 5f * Time.deltaTime);
        Vector3 targetPosition = new Vector3(playerPos.x, smoothedY, playerPos.z) - directionToPlayer * followDistance;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void LookAtPlayerHorizontal()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = 0; 
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void CheckIfInLight()
    {
        // 1. 基础状态检查
        if (checkLight == null || caScript == null || !checkLight.enabled || caScript.isUsingMain) 
        {
            ableToMove = true;
            return;
        }

        // 2. 计算方向和角度
        // targetPoint 设在怪物中心稍微偏上的位置（胶囊体中间）
        Vector3 targetPoint = transform.position + Vector3.up * 1.2f; 
        Vector3 directionToGhost = (targetPoint - checkLight.transform.position).normalized;
        float angle = Vector3.Angle(checkLight.transform.forward, directionToGhost);

        // 调试：蓝线显示手电筒和怪物之间的逻辑连线（只要蓝线出现了，说明角度判定正在工作）
        Debug.DrawLine(checkLight.transform.position, targetPoint, Color.blue, 0.1f);

        // 3. 角度检查
        if (angle <= checkLight.spotAngle / 2f) 
        {
            RaycastHit hit;
            // 4. 射线探测（这里不忽略 Trigger，以防你的子物体 Collider 误勾了 Trigger）
            if (Physics.Raycast(checkLight.transform.position, directionToGhost, out hit, checkLight.range))
            {
                // 重点：检查射中的是否是父物体或任何子物体
                if (hit.collider.gameObject == gameObject || hit.transform.IsChildOf(transform))
                {
                    // 调试：红线代表成功射中怪物
                    Debug.DrawLine(checkLight.transform.position, hit.point, Color.red, 0.1f);

                    if (ableToMove) 
                    {
                        ableToMove = false;
                        if (cor != null) StopCoroutine(cor);
                        cor = StartCoroutine(CountDown());
                    }
                    return; 
                }
                else
                {
                    // 调试：黄线代表射线射中了别的东西（被墙遮挡了）
                    Debug.DrawLine(checkLight.transform.position, hit.point, Color.yellow, 0.1f);
                }
            }
        }
        
        ableToMove = true;
    }

    IEnumerator CountDown()
    {
        isCounting = true;
        yield return new WaitForSecondsRealtime(3.0f);
        isCounting = false;
        cor = null;
    }
}