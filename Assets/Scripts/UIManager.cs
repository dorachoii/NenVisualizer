using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject[] nenTypeObjects = new GameObject[5]; // 5개의 Nen 타입별 게임오브젝트
    public TextMeshProUGUI nenTypeText; // Nen 타입을 표시할 텍스트
    public GameObject restartButton; // 씬 재시작 버튼
    
    [Header("Timing Settings")]
    public float uiDelayTime = 4f; // 셰이더 효과가 보인 후 UI 표시까지의 지연 시간
    
    private string[] nenTypeNames = new string[5] 
    {
        "強化系",
        "操作系", 
        "変化系",
        "具現化系",
        "放出系"
    };
    
    private NenManager nenManager;
    private bool isUIActive = false;
    private Coroutine uiCoroutine;
    
    void Start()
    {
        // NenManager 찾기
        nenManager = FindObjectOfType<NenManager>();
        
        if (nenManager == null)
        {
            Debug.LogError("NenManager not found in scene!");
        }
        
        // 초기 UI 상태 설정
        HideAllNenUI();
        
        // 재시작 버튼 초기 비활성화
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }
    }

    void Update()
    {
        // Nen이 활성화되었을 때 UI 표시 시작
        if (nenManager != null && nenManager.IsNenActive() && !isUIActive)
        {
            StartNenUI();
        }
    }
    
    void StartNenUI()
    {
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
        }
        
        uiCoroutine = StartCoroutine(ShowNenUIWithDelay());
    }
    
    IEnumerator ShowNenUIWithDelay()
    {
        isUIActive = true;
        
        // 셰이더 효과가 충분히 보일 때까지 대기
        yield return new WaitForSeconds(uiDelayTime);
        
        // 현재 Nen 타입에 따라 UI 표시
        ShowNenTypeUI(nenManager.GetCurrentNenType());
        
        // UI는 계속 남아있음 (게임오버 화면처럼)
        // 자동으로 숨기지 않음
    }
    
    void ShowNenTypeUI(NenManager.NenType nenType)
    {
        // 모든 UI 오브젝트 비활성화
        HideAllNenUI();
        
        // 해당 Nen 타입의 인덱스 계산
        int nenIndex = (int)nenType;
        
        // 해당 인덱스의 게임오브젝트 활성화
        if (nenIndex >= 0 && nenIndex < nenTypeObjects.Length && nenTypeObjects[nenIndex] != null)
        {
            nenTypeObjects[nenIndex].SetActive(true);
        }
        
        // 텍스트 업데이트
        if (nenTypeText != null && nenIndex >= 0 && nenIndex < nenTypeNames.Length)
        {
            nenTypeText.text = nenTypeNames[nenIndex];
        }
        
        // 재시작 버튼 활성화
        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }
        
        Debug.Log("Showing UI for: " + nenTypeNames[nenIndex]);
    }
    
    void HideAllNenUI()
    {
        // 모든 Nen 타입 오브젝트 비활성화
        for (int i = 0; i < nenTypeObjects.Length; i++)
        {
            if (nenTypeObjects[i] != null)
            {
                nenTypeObjects[i].SetActive(false);
            }
        }
        
        // 텍스트 초기화
        if (nenTypeText != null)
        {
            nenTypeText.text = "";
        }
        
        // 재시작 버튼 비활성화
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }
    }
    
    // 재시작 버튼 클릭 시 호출되는 함수
    public void RestartScene()
    {
        Debug.Log("Restarting scene...");
        
        // 현재 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // 외부에서 UI 강제 표시
    public void ForceShowNenUI(NenManager.NenType nenType)
    {
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
        }
        
        ShowNenTypeUI(nenType);
        isUIActive = true;
    }
    

}