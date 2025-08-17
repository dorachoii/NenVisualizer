using UnityEngine;

public class NenHand : MonoBehaviour
{
    [Header("Aura Settings")]
    public Material auraMaterial; // Shader_aura가 적용된 머티리얼
    public GameObject auraObject; // 오오라 효과를 보여줄 오브젝트 (원본 핸드의 복사본)
    
    [Header("Aura Animation")]
    public float fadeSpeed = 5f; // 오오라 페이드 인/아웃 속도
    
    private bool isAuraActive = false;
    private Renderer auraRenderer;
    private Material auraMaterialInstance; // 머티리얼 인스턴스
    private float currentAuraIntensity = 0f; // 현재 오오라 강도 (0~1)
    
    // 셰이더 프로퍼티 이름들
    private readonly string AURA_INTENSITY_PROP = "_AuraIntensity";
    private readonly string EMISSION_INTENSITY_PROP = "_EmissionIntensity";
    private readonly string STEAM_INTENSITY_PROP = "_SteamIntensity";
    
    void Start()
    {
        // 오오라 오브젝트가 할당되지 않았다면 자동으로 찾기
        if (auraObject == null)
        {
            // 같은 이름으로 끝나는 오브젝트 찾기 (예: Hand -> Hand_Aura)
            auraObject = GameObject.Find(gameObject.name + "_Aura");
        }
        
        if (auraObject != null)
        {
            auraRenderer = auraObject.GetComponent<Renderer>();
            
            // 머티리얼 인스턴스 생성 (실시간 수정을 위해)
            if (auraMaterial != null)
            {
                auraMaterialInstance = new Material(auraMaterial);
                auraRenderer.material = auraMaterialInstance;
            }
            else
            {
                // 머티리얼이 할당되지 않았다면 현재 머티리얼의 인스턴스 생성
                auraMaterialInstance = new Material(auraRenderer.material);
                auraRenderer.material = auraMaterialInstance;
            }
            
            // 초기에는 오오라 완전히 비활성화
            SetAuraIntensity(0f);
        }
        else
        {
            Debug.LogWarning("Aura object not found! Please assign an aura object or create one with name: " + gameObject.name + "_Aura");
        }
    }

    void Update()
    {
        // Space바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetAuraActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            SetAuraActive(false);
        }
        
        // 오오라 강도 애니메이션
        UpdateAuraAnimation();
    }
    
    void UpdateAuraAnimation()
    {
        float targetIntensity = isAuraActive ? 1f : 0f;
        
        // 부드러운 페이드 인/아웃
        currentAuraIntensity = Mathf.Lerp(currentAuraIntensity, targetIntensity, Time.deltaTime * fadeSpeed);
        
        // 셰이더 프로퍼티 업데이트
        SetAuraIntensity(currentAuraIntensity);
    }
    
    void SetAuraActive(bool active)
    {
        isAuraActive = active;
        
        if (active)
        {
            Debug.Log("Aura activating...");
        }
        else
        {
            Debug.Log("Aura deactivating...");
        }
    }
    
    void SetAuraIntensity(float intensity)
    {
        if (auraMaterialInstance != null)
        {
            // 머티리얼 인스턴스의 셰이더 프로퍼티들을 실시간으로 조정
            auraMaterialInstance.SetFloat(AURA_INTENSITY_PROP, intensity);
            auraMaterialInstance.SetFloat(EMISSION_INTENSITY_PROP, intensity);
            auraMaterialInstance.SetFloat(STEAM_INTENSITY_PROP, intensity * 1.2f); // 수증기 효과는 약간만
            
            // 오오라가 완전히 꺼졌을 때만 오브젝트 비활성화 (성능 최적화)
            if (intensity <= 0.01f)
            {
                auraObject.SetActive(false);
            }
            else if (!auraObject.activeSelf)
            {
                auraObject.SetActive(true);
            }
        }
    }
    
    // 외부에서 오오라 상태 확인 가능
    public bool IsAuraActive()
    {
        return isAuraActive;
    }
    
    // 외부에서 오오라 강도 확인 가능
    public float GetAuraIntensity()
    {
        return currentAuraIntensity;
    }
    
    // 메모리 정리
    void OnDestroy()
    {
        if (auraMaterialInstance != null)
        {
            DestroyImmediate(auraMaterialInstance);
        }
    }
}
