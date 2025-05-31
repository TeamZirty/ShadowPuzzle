/*
Desc: 플레이어의 입력을 받아 횃불 빛의 색상을 변경하고, 마우스 위치를 따라 빛 오브젝트가 움직이도록 제어합니다.
*/
using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    [Header("빛 설정")]
    public Transform lightTransform;        // 빛으로 사용할 게임 오브젝트의 Transform 컴포넌트입니다. 이 Transform의 위치를 조절하여 빛을 움직입니다.
    public SpriteRenderer lightSpriteRenderer; // 빛 오브젝트의 SpriteRenderer 컴포넌트입니다. 빛의 색상, 모양(Sprite) 등을 제어합니다.
    public float followSpeed = 15f;         // 빛이 마우스를 따라가는 속도입니다. 값이 클수록 빠르게 따라갑니다.

    [Header("색상 설정")] // 인스펙터에서 색상 관련 설정을 그룹화하여 보여줍니다.
    public Color defaultColor = Color.white; // 기본 상태의 빛 색상입니다. (예: 숫자 0키)
    public Color redColor = Color.red;     // 빨간색 빛으로 변경될 때의 색상입니다. (예: 숫자 1키)
    public Color greenColor = Color.green; // 초록색 빛으로 변경될 때의 색상입니다. (예: 숫자 2키)
    public Color blueColor = Color.blue;   // 파란색 빛으로 변경될 때의 색상입니다. (예: 숫자 3키)

    private Camera mainCamera;              // 화면 좌표(마우스 위치)를 월드 좌표로 변환하기 위해 메인 카메라 정보가 필요합니다.

    // 스크립트 인스턴스가 로드될 때 처음 한 번 호출됩니다. (Start보다 먼저 실행될 수 있음, 주로 참조 설정에 사용)
    void Awake() // Start 대신 Awake를 사용해도 무방하며, 때로는 다른 스크립트의 Start보다 먼저 초기화해야 할 때 유용합니다.
    {
        // 메인 카메라를 찾아서 변수에 할당합니다. Camera.main은 "MainCamera" 태그가 붙은 카메라를 반환합니다.
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("TorchLightController: 메인 카메라를 찾을 수 없습니다! 'MainCamera' 태그가 설정되어 있는지 확인해주세요.");
        }

        // lightSpriteRenderer가 인스펙터에서 직접 할당되지 않았지만, lightTransform은 할당되었다면,
        // lightTransform 오브젝트에서 SpriteRenderer 컴포넌트를 찾아 할당하려고 시도합니다.
        if (lightSpriteRenderer == null && lightTransform != null)
        {
            lightSpriteRenderer = lightTransform.GetComponent<SpriteRenderer>();
        }
    }

    // 첫 번째 프레임 업데이트 전에 한 번 호출됩니다. (오브젝트 활성화 시)
    void Start()
    {
        // lightSpriteRenderer가 정상적으로 할당되었다면, 빛의 초기 색상을 defaultColor로 설정합니다.
        if (lightSpriteRenderer != null)
        {
            lightSpriteRenderer.color = defaultColor;
        }
        // lightSpriteRenderer 할당에 실패한 경우, 어떤 부분이 문제인지에 따라 다른 경고 메시지를 출력합니다.
        else if (lightTransform != null) // lightTransform은 있는데 SpriteRenderer가 없는 경우
        {
            Debug.LogError("TorchLightController: Light Sprite Renderer가 " + lightTransform.name + " 오브젝트에 연결되지 않았거나 존재하지 않습니다! SpriteRenderer 컴포넌트를 추가하고 할당해주세요.");
        }
        else // lightTransform 자체도 할당되지 않은 경우
        {
            Debug.LogError("TorchLightController: Light Transform이 연결되지 않았습니다! 빛 역할을 할 오브젝트를 만들고 Transform을 할당해주세요.");
        }
    }

    // 매 프레임마다 호출됩니다.
    void Update()
    {
        // 1. 빛 오브젝트가 마우스 위치를 따라다니도록 처리
        // lightTransform과 mainCamera가 모두 정상적으로 할당되었는지 먼저 확인합니다. (Null 참조 오류 방지)
        if (lightTransform != null && mainCamera != null)
        {
            // Input.mousePosition은 현재 마우스 커서의 화면 좌표(픽셀 단위)를 반환합니다. (좌하단이 0,0)
            Vector3 mouseScreenPosition = Input.mousePosition;

            // mainCamera.ScreenToWorldPoint()는 화면 좌표를 월드 좌표로 변환해줍니다.
            // Z값은 카메라로부터의 거리를 의미하는데, 2D 게임에서는 보통 카메라의 nearClipPlane보다 조금 더 먼 값으로 설정하거나,
            // 빛 오브젝트가 다른 오브젝트와 상호작용하는 Z 깊이를 고려하여 설정합니다.
            // 여기서는 mainCamera.nearClipPlane + 10f 로 설정하여 카메라 시야에 확실히 들어오도록 합니다.
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.transform.position.z + 10f)); // 혹은 고정된 Z값 사용
            mouseWorldPosition.z = 0; // 2D 게임이므로 Z축 값은 0으로 고정하여 깊이 문제를 방지합니다.

            // Vector3.Lerp(현재위치, 목표위치, 속도)는 현재 위치에서 목표 위치로 부드럽게 이동하는 효과를 줍니다.
            // Time.deltaTime을 곱해주는 것은 프레임 속도에 관계없이 일정한 속도로 움직이게 하기 위함입니다.
            lightTransform.position = Vector3.Lerp(lightTransform.position, mouseWorldPosition, followSpeed * Time.deltaTime);
        }

        // 2. 빛 색상 변경 입력 처리
        // lightSpriteRenderer가 할당되었는지 확인합니다.
        if (lightSpriteRenderer != null)
        {
            // 각 숫자 키 입력(GetKeyDown: 키가 눌리는 순간 true)에 따라 lightSpriteRenderer의 color 속성을 변경합니다.
            if (Input.GetKeyDown(KeyCode.Alpha0)) // 숫자 0키
            {
                lightSpriteRenderer.color = defaultColor;
                Debug.Log("빛 색상: 기본"); // 개발 중 확인을 위한 로그
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1)) // 숫자 1키
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
        }
    }

    // 외부 스크립트(예: ColorPuzzleTrigger)에서 현재 빛의 색상을 가져갈 수 있도록 public 메서드로 제공합니다.
    public Color GetCurrentLightColor()
    {
        if (lightSpriteRenderer != null)
        {
            return lightSpriteRenderer.color; // 현재 SpriteRenderer의 색상 반환
        }
        // lightSpriteRenderer가 없는 비정상적인 경우, 기본 색상이라도 반환하도록 처리합니다.
        Debug.LogWarning("TorchLightController: lightSpriteRenderer가 없어 GetCurrentLightColor 호출 시 기본 색상을 반환합니다.");
        return defaultColor;
    }
}