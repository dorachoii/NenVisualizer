using UnityEngine;
using System.Collections;

public class NenManager : MonoBehaviour
{
    public enum NenType
    {
        Kyouka,    // 강화계: 물이 늘어남 (Ripple Shader)
        Sousa,   // 조작계: 나뭇잎이 움직임 (LeafSpin Script)
        Henka,  // 변화계: 물의 맛이 달라짐 (Static Shader)
        Gugenka,    // 구현화계: 물에 불순물이 나타남 (Fractal Shader)
        Housyutu        // 방출계: 물의 색이 변함 (Gradient Shader)
    }
    
    [Header("Target Objects")]
    public GameObject targetPlane; // Nen 효과를 적용할 Plane
    public GameObject leafObject;  // 조작계용 Leaf 오브젝트
    
    [Header("Shaders")]
    public Material rippleShader;      // 강화계: Ripple 효과
    public Material fractalShader;     // 구현화계: Fractal 효과
    public Material gradientShader;    // 방출계: Gradient 효과
    public Material staticShader;      // 변화계: Static 효과
    
    [Header("Settings")]
    public bool showDebugInfo = true;  // 디버그 정보 표시
    
    private NenType currentNenType;
    private Material originalMaterial;
    private LeafSpin leafSpinScript;
    private LeafHeightController leafHeightController; // 강화계용 LeafHeightController
    private bool isNenActive = false;
    private Renderer planeRenderer;
    private bool isNenTypeAssigned = false; // Nen 타입이 할당되었는지 확인
    
    void Start()
    {
        // 컴포넌트 초기화
        if (targetPlane != null)
        {
            planeRenderer = targetPlane.GetComponent<Renderer>();
            if (planeRenderer != null)
            {
                // sharedMaterial 사용으로 오류 방지
                originalMaterial = planeRenderer.sharedMaterial;
            }
        }
        
        if (leafObject != null)
        {
            leafSpinScript = leafObject.GetComponent<LeafSpin>();
            leafHeightController = leafObject.GetComponent<LeafHeightController>();
        }
        
        // 초기 상태 설정
        ResetToOriginalState();
        
        // 초기에 랜덤 Nen 타입 할당 (한 번만)
        AssignRandomNenType();
    }
    
    // 초기에 한 번만 랜덤 Nen 타입 할당
    void AssignRandomNenType()
    {
        if (!isNenTypeAssigned)
        {
            currentNenType = (NenType)Random.Range(0, 5);
            isNenTypeAssigned = true;
            
            if (showDebugInfo)
            {
                Debug.Log("Assigned " + currentNenType.ToString() + " Nen type for this session!");
            }
        }
    }
    
    // NenHand에서 호출할 이벤트 함수들
    public void OnNenActivate()
    {
        if (!isNenActive)
        {
            ActivateCurrentNen();
        }
    }
    
    public void OnNenDeactivate()
    {
        if (isNenActive)
        {
            DeactivateCurrentNen();
        }
    }
    
    void ActivateCurrentNen()
    {
        if (showDebugInfo)
        {
            Debug.Log("Activating " + currentNenType.ToString() + " Nen!");
        }
        
        // 할당된 Nen 타입에 따라 효과 적용
        switch (currentNenType)
        {
            case NenType.Kyouka:
                ApplyEnhancementNen();
                break;
            case NenType.Sousa:
                ApplyManipulationNen();
                break;
            case NenType.Henka:
                ApplyTransmutationNen();
                break;
            case NenType.Gugenka:
                ApplyConjurationNen();
                break;
            case NenType.Housyutu:
                ApplyEmissionNen();
                break;
        }
        
        isNenActive = true;
    }
    
    void ApplyEnhancementNen()
    {
        // 강화계: Ripple 효과 적용
        if (planeRenderer != null && rippleShader != null)
        {
            planeRenderer.sharedMaterial = rippleShader;
        }
        
        // 강화계: LeafHeightController 활성화
        if (leafHeightController != null)
        {
            leafHeightController.enabled = true;
        }
    }
    
    void ApplyManipulationNen()
    {
        // 조작계: LeafSpin 스크립트 활성화
        if (leafSpinScript != null)
        {
            leafSpinScript.enabled = true;
            // LeafSpin에 StartSpinning 트리거
            leafSpinScript.StartSpinning();
        }
    }
    
    void ApplyTransmutationNen()
    {
        // 변화계: Static 효과 (시간이 지나도 안변함)
        if (planeRenderer != null && staticShader != null)
        {
            planeRenderer.sharedMaterial = staticShader;
        }
    }
    
    void ApplyConjurationNen()
    {
        // 구현화계: Fractal 효과 적용
        if (planeRenderer != null && fractalShader != null)
        {
            planeRenderer.sharedMaterial = fractalShader;
        }
    }
    
    void ApplyEmissionNen()
    {
        // 방출계: Gradient 효과 적용
        if (planeRenderer != null && gradientShader != null)
        {
            planeRenderer.sharedMaterial = gradientShader;
        }
    }
    
    void DeactivateCurrentNen()
    {
        if (showDebugInfo)
        {
            Debug.Log("Deactivating " + currentNenType.ToString() + " Nen!");
        }
        
        // 현재 Nen 효과 해제
        switch (currentNenType)
        {
            case NenType.Kyouka:
                // 강화계: LeafHeightController 스크립트 비활성화
                if (leafHeightController != null)
                {
                    leafHeightController.enabled = false;
                }
                // 강화계: 원본 머티리얼로 복원
                ResetToOriginalState();
                break;
            case NenType.Sousa:
                // 조작계: LeafSpin 스크립트 비활성화
                if (leafSpinScript != null)
                {
                    leafSpinScript.enabled = false;
                }
                break;
            default:
                // 다른 Nen 타입들: 원본 머티리얼로 복원
                ResetToOriginalState();
                break;
        }
        
        isNenActive = false;
    }
    
    void ResetToOriginalState()
    {
        if (planeRenderer != null && originalMaterial != null)
        {
            planeRenderer.sharedMaterial = originalMaterial;
        }
        
        if (leafSpinScript != null)
        {
            leafSpinScript.enabled = false;
        }
        if (leafHeightController != null)
        {
            leafHeightController.enabled = false;
        }
    }
    
    // 외부에서 현재 Nen 상태 확인
    public NenType GetCurrentNenType()
    {
        return currentNenType;
    }
    
    public bool IsNenActive()
    {
        return isNenActive;
    }
    
    // 외부에서 특정 Nen 타입 강제 활성화
    public void ForceActivateNen(NenType nenType)
    {
        if (isNenActive)
        {
            DeactivateCurrentNen();
        }
        
        currentNenType = nenType;
        
        switch (currentNenType)
        {
            case NenType.Kyouka:
                ApplyEnhancementNen();
                break;
            case NenType.Sousa:
                ApplyManipulationNen();
                break;
            case NenType.Henka:
                ApplyTransmutationNen();
                break;
            case NenType.Gugenka:
                ApplyConjurationNen();
                break;
            case NenType.Housyutu:
                ApplyEmissionNen();
                break;
        }
        
        isNenActive = true;
    }
    
    // 외부에서 Nen 강제 해제
    public void ForceDeactivateNen()
    {
        DeactivateCurrentNen();
    }
    
    // 메모리 정리
    void OnDestroy()
    {
        ResetToOriginalState();
    }
}