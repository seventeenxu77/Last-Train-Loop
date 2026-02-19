using System.Collections;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GhostMove : MonoBehaviour
{
    public Vector3 ghostBornPlace;
    public Light checkLight;
    public CameraSet caScript;//拖入脚本

    GameObject player;
    bool ableToMove, isCounting, showUp;
    int followDistance = 2;
    double nextCheckTime = 0,checkInterval = 0.2f;

    private Coroutine cor;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ableToMove = true;
        cor = null;
        isCounting = false;
        showUp = false;
        transform.position = ghostBornPlace;
    }
    void FixedUpdate()
    {
        if (Time.time >= nextCheckTime)
        {
            CheckIfInLight();
            nextCheckTime = Time.time + checkInterval;
        }
      
        if (showUp && ableToMove && !isCounting)
        {
            Move();
        }
       
    }
    public void Summon()
    {
        Invoke("GhostShowUp", 10f);
    }
    void GhostShowUp()
    {
        transform.position = ghostBornPlace;
        transform.LookAt(player.transform);
        transform.gameObject.SetActive(true);
        showUp = true;
    }
    void Move()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 targetPosition = player.transform.position - directionToPlayer * followDistance;

        // 向目标位置移动
        transform.position = Vector3.MoveTowards(transform.position,targetPosition, 3f * Time.deltaTime);
    }
    void CheckIfInLight()
    { 
        if (checkLight == null)
        {
            Debug.LogWarning("未找到Spotlight！");
            return;
        }
        if (caScript.isUsingMain)
        {
            ableToMove = true;
            if (cor == null) isCounting = false;
            return;
        }
        // 检查ghost是否在spotlight的锥形范围内
        Vector3 directionToGhost = (transform.position - checkLight.transform.position).normalized;
        float angle = Vector3.Angle(checkLight.transform.forward, directionToGhost);

        // 如果在锥形角度内且距离足够近
        if (angle <= checkLight.spotAngle)
        {
            Debug.Log("Ghost被照到！");
            RaycastHit hit;
            if (Physics.Raycast(checkLight.transform.position, directionToGhost * Vector3.Distance(checkLight.transform.position, transform.position), out hit, checkLight.range))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ableToMove = false;
                    if (cor != null) StopCoroutine(cor);
                    cor = StartCoroutine(CountDown());
                    Debug.Log("Ghost被射线射到！");
                }
            }
        }
        else
        {
            ableToMove = true;
            if(cor == null) isCounting = false;
        }
    }
    IEnumerator CountDown()
    {
        isCounting = true;
        yield return new WaitForSecondsRealtime(3.0f);
        isCounting = false;
        cor = null;
    }
}
