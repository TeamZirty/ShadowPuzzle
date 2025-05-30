/*
Desc: 플레이어의 입력을 받아 횃불 빛의 색상을 변경하고, 마우스 위치를 따라 빛 오브젝트가 움직이도록 제어합니다.
*/
using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    [Header("빛 설정")]
    public Transform lightTransform; // 빛으로 사용할 오브젝트의 Transform
    public SpriteRenderer lightSpriteRenderer; // 빛 오브젝트의 SpriteRenderer (색 변경용)
    public float followSpeed = 15f; // 마우스 따라오는 속도

    [Header("색상 설정")]
    public Color defaultColor = Color.white;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    public Color blueColor = Color.blue;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (lightSpriteRenderer == null && lightTransform != null)
        {
            lightSpriteRenderer = lightTransform.GetComponent<SpriteRenderer>();
        }

        if (lightSpriteRenderer != null)
        {
            lightSpriteRenderer.color = defaultColor; // 기본 색상으로 시작
        }
        else
        {
            Debug.LogError("Light Sprite Renderer가 연결되지 않았습니다!");
        }

        if (lightTransform == null)
        {
            Debug.LogError("Light Transform이 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        // 빛 오브젝트가 마우스 위치를 따라다니도록 처리
        if (lightTransform != null)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane + 10f)); // Z값을 카메라에 맞게 조정
            mouseWorldPosition.z = 0; // 2D 환경이므로 z는 0으로 고정
            lightTransform.position = Vector3.Lerp(lightTransform.position, mouseWorldPosition, followSpeed * Time.deltaTime);
        }

        // 빛 색상 변경 입력 처리
        if (lightSpriteRenderer != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // 숫자 1키
            {
                lightSpriteRenderer.color = redColor;
                Debug.Log("빛 색상: 빨강");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // 숫자 2키
            {
                lightSpriteRenderer.color = greenColor;
                Debug.Log("빛 색상: 초록");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) // 숫자 3키
            {
                lightSpriteRenderer.color = blueColor;
                Debug.Log("빛 색상: 파랑");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0)) // 숫자 0키 (기본색)
            {
                lightSpriteRenderer.color = defaultColor;
                Debug.Log("빛 색상: 기본");
            }
        }
    }

    // 외부에서 현재 빛의 색상을 가져갈 수 있는 함수 (선택적)
    public Color GetCurrentLightColor()
    {
        if (lightSpriteRenderer != null)
        {
            return lightSpriteRenderer.color;
        }
        return defaultColor; // 기본값 반환
    }
}