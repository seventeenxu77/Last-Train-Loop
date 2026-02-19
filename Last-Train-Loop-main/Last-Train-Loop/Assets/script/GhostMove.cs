using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // [新增] 用于场景跳转

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

    [Header("杀戮设置")] // [新增]
    public float killDistance = 0.5f; // 触发死亡的距离
    public string deathSceneName = "dolldeath"; // 对应你截图里的死亡场景名

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

        // 判断是否应该移动
        bool shouldMove = showUp && ableToMove && !isCounting;

        if (shouldMove)
        {
            Move();
            UpdateVisuals(true);
            CheckKillPlayer(); // [新增] 移动时检查是否撞到玩家
        }
        else
        {
            UpdateVisuals(false);
        }
    }

    // [新增] 死亡检查逻辑
    void CheckKillPlayer()
    {
        if (player == null || !showUp) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        // 运行阶段在控制台实时打印距离，看看你贴着它时，这个数字是多少
        Debug.Log("当前距离怪物: " + dist);

        if (dist < killDistance)
        {
            ExecuteGameOver();
        }
    }

    // [新增] 执行游戏结束
    void ExecuteGameOver()
    {
        showUp = false; // 停止怪物逻辑

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("<color=red>幽灵抓住了玩家！跳转场景...</color>");
        if (LoopManager.Instance != null)
        {
            Destroy(LoopManager.Instance.gameObject);
        }
        SceneManager.LoadScene(deathSceneName);
    }

void UpdateVisuals(bool isWalking)
{
    if (anim != null)
    {
        if (isWalking)
        {
            anim.speed = 3.0f; // 没被看到时，动画快放
        }
        else
        {
            // 当刚被发现而停下时
            if (anim.speed > 0.1f) 
            {
                // 核心：瞬间切换到一个随机姿势定格
                anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.value);
            }
            // 微动感：给一个极小的速度，让它看起来像是在极慢地呼吸或颤抖
            anim.speed = 0.02f; 
        }
    }

    if (footstepAudio != null)
    {
        // 既然速度变快了，声音的音调(Pitch)也要调高才有急促感
        footstepAudio.pitch = isWalking ? 1.5f : 1.0f; 
        
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
        if (checkLight == null || caScript == null || !checkLight.enabled || caScript.isUsingMain)
        {
            ableToMove = true;
            return;
        }

        Vector3 targetPoint = transform.position + Vector3.up * 1.2f;
        Vector3 directionToGhost = (targetPoint - checkLight.transform.position).normalized;
        float angle = Vector3.Angle(checkLight.transform.forward, directionToGhost);

        Debug.DrawLine(checkLight.transform.position, targetPoint, Color.blue, 0.1f);

        if (angle <= checkLight.spotAngle / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(checkLight.transform.position, directionToGhost, out hit, checkLight.range))
            {
                if (hit.collider.gameObject == gameObject || hit.transform.IsChildOf(transform))
                {
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