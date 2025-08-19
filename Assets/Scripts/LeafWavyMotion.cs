using UnityEngine;

public class LeafWavyMotion : MonoBehaviour
{
    [Header("Floating Motion Settings")]
    public float floatSpeed = 2f; // 떠다니는 속도
    public float floatAmplitude = 0.2f; // 떠다니는 높이
    public float horizontalSpeed = 1.5f; // 좌우 움직임 속도
    public float horizontalAmplitude = 0.1f; // 좌우 움직임 범위
    
    private Vector3 originalPosition; // 원래 위치
    
    void Start()
    {
        // 원래 위치 저장
        originalPosition = transform.position;
    }

    void Update()
    {
        // 떠다니는 효과 적용
        ApplyFloatingMotion();
    }
    
    void ApplyFloatingMotion()
    {
        float time = Time.time;
        
        // 위아래 움직임 (Y축)
        float verticalWave = Mathf.Sin(time * floatSpeed) * floatAmplitude;
        
        // 좌우 움직임 (X축) - 선택적
        float horizontalWave = Mathf.Sin(time * horizontalSpeed) * horizontalAmplitude;
        
        // 새로운 위치 계산
        Vector3 newPosition = originalPosition + new Vector3(horizontalWave, verticalWave, 0f);
        
        // 위치 적용
        transform.position = newPosition;
    }
    
 
}
