using UnityEngine;

public class NenHand : MonoBehaviour
{
    [Header("Aura Settings")]
    public Material auraMaterial; // Shader_aura가 적용된 머티리얼
    public GameObject auraObject; // 오오라 효과를 보여줄 오브젝트 (원본 핸드의 복사본)
    
    [Header("Aura Animation")]
    public float fadeSpeed = 5f; // 오오라 페이드 인/아웃 속도
    
    [Header("Nen Manager Reference")]
    public NenManager nenManager; // NenManager 참조
    
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
            // Renderer 컴포넌트 체크
            auraRenderer = auraObject.GetComponent<Renderer>();
            
            if (auraRenderer != null)
            {
                // 머티리얼 인스턴스 생성 (실시간 수정을 위해)
                if (auraMaterial != null)
                {
                    auraMaterialInstance = new Material(auraMaterial);
                    auraRenderer.material = auraMaterialInstance;
                }
                else
                {
                    // 머티리얼이 할당되지 않았다면 현재 머티리얼의 인스턴스 생성
                    if (auraRenderer.material != null)
                    {
                        auraMaterialInstance = new Material(auraRenderer.material);
                        auraRenderer.material = auraMaterialInstance;
                    }
                    else
                    {
                        Debug.LogError("No material found on aura object: " + auraObject.name);
                        return;
                    }
                }
                
                // 초기에는 오오라 완전히 비활성화
                SetAuraIntensity(0f);
            }
            else
            {
                Debug.LogError("No Renderer component found on aura object: " + auraObject.name + 
                             "\nPlease add a MeshRenderer or SkinnedMeshRenderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Aura object not found! Please assign an aura object or create one with name: " + gameObject.name + "_Aura");
        }
        
        // NenManager 자동 찾기
        if (nenManager == null)
        {
            nenManager = FindObjectOfType<NenManager>();
        }
    }

    void Update()
    {
        // 입력 감지 (스페이스바 또는 터치)
        bool inputDetected = false;
        
        // 스페이스바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnInputPressed();
            inputDetected = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            OnInputReleased();
            inputDetected = true;
        }
        
        // 터치 입력 감지 (스페이스바 입력이 없을 때만)
        if (!inputDetected)
        {
            HandleTouchInput();
        }
        
        // 오오라 강도 애니메이션
        UpdateAuraAnimation();
    }
    
    // 터치 입력 처리
    void HandleTouchInput()
    {
        // 터치 입력 감지 (화면 상단 30% 제외)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치만 처리
            
            // 화면 상단 30% 제외하고 터치 감지
            if (touch.position.y < Screen.height * 0.7f)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // 터치 시작
                        OnInputPressed();
                        break;
                        
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        // 터치 종료
                        OnInputReleased();
                        break;
                }
            }
        }
        
        // 마우스 입력도 터치로 처리 (PC에서 테스트용, 화면 상단 30% 제외)
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.y < Screen.height * 0.7f)
            {
                OnInputPressed();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Input.mousePosition.y < Screen.height * 0.7f)
            {
                OnInputReleased();
            }
        }
    }
    
    void OnInputPressed()
    {
        // 오오라 활성화
        SetAuraActive(true);
        
        // NenManager에 이벤트 전달
        if (nenManager != null)
        {
            nenManager.OnNenActivate();
        }
    }
    
    // 입력 종료
    void OnInputReleased()
    {
        if (isAuraActive)
        {
            isAuraActive = false;
            
            // 오오라 비활성화
            SetAuraActive(false);
            
            // NenManager에 이벤트 전달
            if (nenManager != null)
            {
                nenManager.OnNenDeactivate();
            }
            
            // LeafSpin이 활성화되어 있다면 StopSpinning 호출
            if (nenManager != null && nenManager.GetCurrentNenType() == NenManager.NenType.Sousa)
            {
                LeafSpin leafSpin = nenManager.leafObject?.GetComponent<LeafSpin>();
                if (leafSpin != null && leafSpin.enabled)
                {
                    leafSpin.StopSpinning();
                }
            }
            
            Debug.Log("Input deactivated - Aura and Nen effects stopped!");
        }
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
        if (auraMaterialInstance != null && auraObject != null)
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
    
    // 외부에서 회전 축 변경 가능
    public void SetRotationAxis(Vector3 axis)
    {
        // LeafSpin 스크립트가 있다면 회전 축 변경
        LeafSpin leafSpin = GetComponent<LeafSpin>();
        if (leafSpin != null)
        {
            leafSpin.SetRotationAxis(axis);
        }
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
