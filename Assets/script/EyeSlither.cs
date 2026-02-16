using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeSlitherPro : MonoBehaviour
{
    [Header("眼球运动")]
    public float moveRange = 0.15f;    // 蠕动半径
    public float slitherSpeed = 0.6f;  // 蠕动速度
    
    [Header("脐带样式")]
    public float baseWidth = 0.03f;    // 基础粗细
    public float sagAmount = 0.3f;     // 弯曲程度（调大此值，吊着的效果更明显）
    public Color cordColor = new Color(0.8f, 0.1f, 0.1f);

    [Header("呼吸效果")]
    public float breatheSpeed = 2.0f;  
    public float pulseIntensity = 0.01f; 
    public bool useEmissionPulse = true; 

    private Vector3 initialLocalPos;
    private Transform mouthAnchor; 
    private LineRenderer line;
    
    private float randOffset; 
    private float randSeed;   
    private Vector3 individualHangDir; // 每个眼球独特的下垂/上吊方向

    void Start()
    {
        randOffset = Random.Range(0f, 100f);
        randSeed = Random.Range(0f, 100f);
        
        initialLocalPos = transform.localPosition;

        // 核心改动 1：随机化方向
        // 随机一个球体内的方向，这样有的向上，有的向下，有的向左斜
        // 如果你想让它主要偏向上下，可以加大 Y 轴权重
        individualHangDir = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f)).normalized;

        GameObject root = new GameObject(name + "_RootAnchor");
        mouthAnchor = root.transform;
        mouthAnchor.SetParent(transform.parent);
        mouthAnchor.localPosition = initialLocalPos;

        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 16; // 增加到16，让弯曲更平滑

        // 材质处理：解决紫色问题
        // 如果你用的是 URP，请务必手动在面板拖入一个材质
        if(line.sharedMaterial == null) {
            line.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        slitherSpeed *= Random.Range(0.8f, 1.2f);
        breatheSpeed *= Random.Range(0.9f, 1.1f);
    }

    void Update()
    {
        // --- A. 随机蠕动逻辑 ---
        float timeX = Time.time * slitherSpeed + randOffset;
        float timeY = Time.time * slitherSpeed + randSeed;
        
        float x = (Mathf.PerlinNoise(timeX, 0) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(0, timeY) - 0.5f) * 2f;
        
        transform.localPosition = initialLocalPos + new Vector3(x * moveRange, y * moveRange, 0);

        // --- B. 呼吸效果逻辑 ---
        float pulse = Mathf.Sin(Time.time * breatheSpeed + randOffset); 
        float currentWidth = baseWidth + (pulse * pulseIntensity);
        
        line.startWidth = currentWidth;
        line.endWidth = currentWidth * 0.5f;

        if (useEmissionPulse)
        {
            float colorPulse = 0.5f + (pulse + 1f) * 0.25f; 
            line.startColor = cordColor * colorPulse;
            line.endColor = new Color(cordColor.r * 0.3f, 0, 0, 1f);
        }

        // --- C. 绘制连线 ---
        DrawBreathingCord(pulse);
    }

    void DrawBreathingCord(float pulse)
    {
        Vector3 start = transform.position;
        Vector3 end = mouthAnchor.position;
        
        // 1. 计算中点
        Vector3 midPointBase = (start + end) * 0.5f;

        // 2. 核心修正：使用每个眼球特有的方向 individualHangDir
        // 这样有的眼球会往上吊，有的往下垂
        Vector3 currentHangDir = individualHangDir; 

        // 3. 计算最终控制点
        // 呼吸脉冲也会微弱影响弯曲度，看起来像在跳动
        Vector3 controlPoint = midPointBase + currentHangDir * (sagAmount + pulse * 0.02f);

        // 4. 渲染贝塞尔曲线
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);
            // 二次贝塞尔公式实现圆滑弧度
            Vector3 m1 = Vector3.Lerp(start, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, end, t);
            line.SetPosition(i, Vector3.Lerp(m1, m2, t));
        }
    }
}