using UnityEngine;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    [Header("Blinking Settings")]
    public float blinkSpeed = 1f; // 깜빡임 속도 (초)
    public float fadeInDuration = 0.3f; // 페이드 인 지속 시간
    public float fadeOutDuration = 0.3f; // 페이드 아웃 지속 시간
    public float holdTime = 0.5f; // 0에서 유지하는 시간
    
    [Header("Text Component")]
    public CanvasGroup canvasGroup; // CanvasGroup 컴포넌트 (자동 할당)
    
    private bool hasInputOccurred = false; // 입력이 한 번이라도 발생했는지
    private Coroutine blinkCoroutine;
    private float currentAlpha = 0f;
    private float targetAlpha = 0f;
    
    void Start()
    {
        // CanvasGroup 컴포넌트 자동 찾기
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        // CanvasGroup이 없으면 추가
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 초기 알파값 설정 (0에서 시작)
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            currentAlpha = 0f;
            targetAlpha = 0f;
        }
        
        // 초기화 시 바로 깜빡임 시작
        StartBlinking();
    }
    
    void Update()
    {
        // 입력이 이미 발생했다면 더 이상 처리하지 않음
        if (hasInputOccurred) return;
        
        // 입력 감지 (스페이스바 또는 터치/마우스)
        bool inputDetected = false;
        
        // 스페이스바 입력 감지
        if (Input.GetKey(KeyCode.Space))
        {
            inputDetected = true;
        }
        
        // 터치 입력 감지 (화면 상단 30% 제외)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // 화면 상단 30% 제외하고 터치 감지
            if (touch.position.y < Screen.height * 0.7f)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    inputDetected = true;
                }
            }
        }
        
        // 마우스 입력 감지 (PC에서 테스트용, 화면 상단 30% 제외)
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.y < Screen.height * 0.7f)
            {
                inputDetected = true;
            }
        }
        
        // 입력이 감지되면 영구적으로 숨기기
        if (inputDetected && !hasInputOccurred)
        {
            hasInputOccurred = true;
            StopBlinking();
        }
        
        // 알파값 업데이트 (부드러운 전환)
        if (canvasGroup != null)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 8f); // 8f는 전환 속도
            canvasGroup.alpha = currentAlpha;
        }
    }
    
    // 깜빡임 시작
    void StartBlinking()
    {
        Debug.Log("StartBlinking");
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkText());
    }
    
    // 깜빡임 정지
    void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        // 0으로 즉시 설정
        targetAlpha = 0f;
        currentAlpha = 0f;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
    
    // 텍스트 깜빡임 코루틴
    IEnumerator BlinkText()
    {
        while (!hasInputOccurred)
        {
            // 0에서 시작해서 서서히 나타남
            targetAlpha = 1f;
            yield return new WaitForSeconds(fadeInDuration);
            
            // 1에서 유지
            yield return new WaitForSeconds(holdTime);
            
            // 서서히 사라짐
            targetAlpha = 0f;
            yield return new WaitForSeconds(fadeOutDuration);
            
            // 0에서 유지
            yield return new WaitForSeconds(holdTime);
        }
    }
    
    // 컴포넌트 비활성화 시 정리
    void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
    }
}
