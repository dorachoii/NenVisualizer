using UnityEngine;

public class LeafHeightController : MonoBehaviour
{
    [Header("Height Settings")]
    public float maxHeight = 2f; // 최대 높이
    public float heightMultiplier = 1f; // 높이 배수
    public float smoothSpeed = 5f; // 부드러운 움직임 속도
    public float riseSpeed = 3f; // 올라가는 속도
    public float fallSpeed = 2f; // 내려가는 속도 (더 부드럽게)
    
    [Header("References")]
    public GameObject targetPlane; // 셰이더가 적용된 Plane
    public NenManager nenManager; // NenManager 참조
    
    private Vector3 originalPosition; // 원래 위치
    private Vector3 targetPosition; // 목표 위치
    private Renderer planeRenderer; // Plane의 Renderer
    private Material planeMaterial; // Plane의 Material
    private bool isRising = false; // 올라가는 중인지 여부
    private float currentHeight = 0f; // 현재 높이 (0~1)
    private LeafWavyMotion leafWavyMotion; // LeafWavyMotion 스크립트 참조
    
    void Start()
    {
        // 원래 위치 저장
        originalPosition = transform.position;
        targetPosition = originalPosition;
        
        // NenManager 자동 찾기
        if (nenManager == null)
        {
            nenManager = FindObjectOfType<NenManager>();
        }
        
        // Plane Renderer 찾기
        if (targetPlane != null)
        {
            planeRenderer = targetPlane.GetComponent<Renderer>();
        }
        
        // LeafWavyMotion 스크립트 찾기
        leafWavyMotion = GetComponent<LeafWavyMotion>();
    }
    
    void Update()
    {
        // 컴포넌트가 활성화되어 있고 강화계가 활성화되었는지 확인
        if (enabled && IsEnhancementActive())
        {
            // LeafWavyMotion 비활성화 (충돌 방지)
            if (leafWavyMotion != null)
            {
                leafWavyMotion.enabled = false;
            }
            
            // 입력에 따라 올라가거나 내려가기
            UpdateHeightBasedOnInput();
        }
        else
        {
            // LeafWavyMotion 활성화 (정상적인 떠다니는 움직임)
            if (leafWavyMotion != null)
            {
                leafWavyMotion.enabled = true;
            }
            
            // 비활성화되면 원래 위치로 복원
            ReturnToOriginalPosition();
        }
        
        // 부드러운 위치 전환
        SmoothMoveToTarget();
    }
    
    // 강화계가 활성화되었는지 확인
    bool IsEnhancementActive()
    {
        if (nenManager != null)
        {
            return nenManager.IsNenActive();
        }
        return false;
    }
    
    // 입력에 따라 높이 업데이트
    void UpdateHeightBasedOnInput()
    {
        // NenHand에서 입력 상태 확인
        NenHand nenHand = FindObjectOfType<NenHand>();
        if (nenHand != null && nenHand.IsAuraActive())
        {
            // 입력이 활성화되면 올라가기 (최대 높이까지)
            if (currentHeight < 1f)
            {
                RiseUp();
            }
            // 최대 높이에 도달하면 거기서 유지
        }
        else
        {
            // 입력이 비활성화되면 내려가기
            if (currentHeight > 0f)
            {
                FallDown();
            }
        }
    }
    
    // 올라가기
    void RiseUp()
    {
        isRising = true;
        currentHeight += riseSpeed * Time.deltaTime;
        currentHeight = Mathf.Min(currentHeight, 1f);
        
        UpdateTargetHeight();
    }
    
    // 내려가기
    void FallDown()
    {
        isRising = false;
        currentHeight -= fallSpeed * Time.deltaTime;
        currentHeight = Mathf.Max(currentHeight, 0f);
        
        UpdateTargetHeight();
    }
    
    // 목표 높이 업데이트
    void UpdateTargetHeight()
    {
        if (planeRenderer != null && planeRenderer.material != null)
        {
            // 셰이더에서 Wave Amplitude 값 가져오기
            float waveAmplitude = planeRenderer.material.GetFloat("_WaveAmp");
            
            // 높이 계산 (셰이더의 파동 높이에 비례하고 현재 높이 비율 적용)
            float targetHeight = waveAmplitude * heightMultiplier * maxHeight * currentHeight;
            
            // 목표 위치 설정 (Y축만 변경)
            targetPosition = originalPosition + Vector3.up * targetHeight;
        }
    }
    
    // 원래 위치로 복원
    void ReturnToOriginalPosition()
    {
        targetPosition = originalPosition;
        currentHeight = 0f;
        isRising = false;
    }
    
    // 부드러운 위치 전환
    void SmoothMoveToTarget()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
    
    // 외부에서 현재 높이 상태 확인 가능
    public bool IsRising()
    {
        return isRising;
    }
    
    public float GetCurrentHeightRatio()
    {
        return currentHeight;
    }
}
