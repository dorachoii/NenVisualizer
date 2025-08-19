using UnityEngine;

public class LeafSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    public float maxRotationSpeed = 720f; // 최대 회전 속도 (도/초) - 2회전/초
    public float accelerationSpeed = 360f; // 가속 속도 (도/초²)
    public float decelerationSpeed = 720f; // 감속 속도 (도/초²)
    public Vector3 rotationAxis = Vector3.up; // 회전 축 (기본값: Y축)
    
    private float currentRotationSpeed = 0f; // 현재 회전 속도
    private bool isSpinning = false; // 회전 중인지 여부
    
    void Start()
    {
        // 초기에는 정지 상태
        currentRotationSpeed = 0f;
        isSpinning = false;
    }

    void Update()
    {
        // Space바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpinning();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopSpinning();
        }
        
        // 회전 속도 업데이트
        UpdateRotationSpeed();
        
        // 실제 회전 적용
        ApplyRotation();
    }
    
    public void StartSpinning()
    {
        isSpinning = true;
        Debug.Log("Leaf spinning started!");
    }
    
    public void StopSpinning()
    {
        isSpinning = false;
        Debug.Log("Leaf spinning stopped!");
    }
    
    void UpdateRotationSpeed()
    {
        if (isSpinning)
        {
            // 가속 (최대 속도까지)
            currentRotationSpeed += accelerationSpeed * Time.deltaTime;
            currentRotationSpeed = Mathf.Min(currentRotationSpeed, maxRotationSpeed);
        }
        else
        {
            // 감속 (0까지)
            currentRotationSpeed -= decelerationSpeed * Time.deltaTime;
            currentRotationSpeed = Mathf.Max(currentRotationSpeed, 0f);
        }
    }
    
    void ApplyRotation()
    {
        // 현재 속도로 회전
        float rotationAmount = currentRotationSpeed * Time.deltaTime;
        transform.Rotate(rotationAxis, rotationAmount);
    }
    
    // 외부에서 회전 상태 확인 가능
    public bool IsSpinning()
    {
        return isSpinning;
    }
    
    // 외부에서 현재 회전 속도 확인 가능
    public float GetCurrentRotationSpeed()
    {
        return currentRotationSpeed;
    }
    
    // 외부에서 회전 축 변경 가능
    public void SetRotationAxis(Vector3 axis)
    {
        rotationAxis = axis.normalized;
    }
}
