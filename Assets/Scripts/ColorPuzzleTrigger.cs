/*
Desc: 특정 색상의 빛이 자신의 영역에 들어왔을 때 연결된 문을 열도록 하는 퍼즐 트리거입니다.
*/
using UnityEngine;

public class ColorPuzzleTrigger : MonoBehaviour
{
    [Header("퍼즐 설정")]
    public DoorController connectedDoor;    // 이 퍼즐 트리거와 연결되어 열리게 될 문 오브젝트의 DoorController 컴포넌트입니다.
    public Color requiredColor = Color.red; // 퍼즐을 해결(문을 열기)하는 데 필요한 빛의 정확한 색상입니다. 인스펙터에서 설정합니다.
    public string triggerTag = "Light";     // 이 트리거가 감지할 빛 오브젝트에 설정된 태그입니다. (기본값: "Light")
    public bool disableAfterTrigger = true; // 퍼즐이 한 번 성공적으로 작동된 후, 이 트리거를 비활성화할지 여부입니다.

    [Header("디버그용 (현재 빛 색상 일치 여부)")]
    [SerializeField] // private 변수지만 인스펙터에서 값을 확인하고 싶을 때 사용합니다. (수정은 불가)
    private bool isCorrectColorDetected = false; // 현재 트리거 영역 내 빛의 색상이 requiredColor와 일치하는지 여부입니다.

    private bool isAlreadyTriggered = false;    // 이 퍼즐이 이미 한 번 성공적으로 발동되었는지 여부를 저장하는 플래그입니다.

    // 스크립트가 처음 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // connectedDoor가 인스펙터에서 할당되지 않았으면 개발자에게 경고를 보냅니다.
        if (connectedDoor == null)
        {
            Debug.LogError(gameObject.name + "에 DoorController가 연결되지 않았습니다! 문 오브젝트를 연결해주세요.");
        }

        // (선택적 기능) 시작 시, 이 트리거 오브젝트에 SpriteRenderer가 있다면,
        // 그 Sprite의 색상을 requiredColor와 유사한 반투명 색으로 변경하여
        // 에디터나 게임 내에서 시각적인 힌트를 제공할 수 있습니다.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(requiredColor.r, requiredColor.g, requiredColor.b, 0.5f); // 50% 투명도
        }
    }

    // 다른 Collider2D가 이 오브젝트의 트리거 영역(IsTrigger가 체크된 Collider2D) 안에 "머무는 동안" 매 프레임 호출됩니다.
    // OnTriggerEnter2D 대신 Stay를 사용한 이유: 빛이 이미 영역 안에 들어와 있는 상태에서 플레이어가 빛의 색을 바꿨을 때,
    // 그 변경을 즉시 감지하고 반응하기 위함입니다. Enter는 들어오는 순간만 감지합니다.
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 이미 퍼즐이 발동되었고, 한 번 발동 후 비활성화하는 옵션이 켜져 있다면 더 이상 로직을 진행하지 않습니다.
        if (isAlreadyTriggered && disableAfterTrigger) return;

        // 충돌한 오브젝트의 태그가 우리가 지정한 triggerTag("Light")와 일치하는지 확인합니다.
        if (collision.CompareTag(triggerTag))
        {
            // 빛 오브젝트에서 TorchLightController 컴포넌트를 찾아 현재 빛의 색상 정보를 가져옵니다.
            // GetComponentInParent를 사용한 이유: TorchLightController 스크립트가 빛 오브젝트(collision.gameObject)의
            // 부모(예: Player 오브젝트)에 붙어있을 수 있기 때문입니다. 구조에 따라 GetComponent로 변경 가능합니다.
            TorchLightController torchController = collision.GetComponentInParent<TorchLightController>();

            if (torchController != null)
            {
                // TorchLightController로부터 현재 빛의 실제 색상을 가져옵니다.
                Color currentLightColor = torchController.GetCurrentLightColor();

                // IsColorMatch 메서드를 사용해 필요한 색상(requiredColor)과 현재 빛의 색상(currentLightColor)이
                // 거의 일치하는지 확인합니다. (부동 소수점 오차를 고려한 비교)
                if (IsColorMatch(currentLightColor, requiredColor))
                {
                    isCorrectColorDetected = true; // 디버그용 플래그 업데이트
                    // 연결된 문이 있고, 아직 이 퍼즐이 발동된 적이 없다면 문을 엽니다.
                    if (connectedDoor != null && !isAlreadyTriggered)
                    {
                        Debug.Log(gameObject.name + ": 올바른 색상('" + requiredColor + "')의 빛 감지! 문(" + connectedDoor.gameObject.name + ")을 엽니다.");
                        connectedDoor.OpenDoor(); // 연결된 문의 OpenDoor() 메서드를 호출합니다.
                        isAlreadyTriggered = true; // 퍼즐이 발동되었음을 표시합니다.

                        // 한 번 발동 후 비활성화 옵션이 켜져 있다면, 이 퍼즐 트리거 게임 오브젝트를 비활성화합니다.
                        if (disableAfterTrigger)
                        {
                            // gameObject는 이 스크립트가 붙어있는 게임 오브젝트 자신을 의미합니다.
                            gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    isCorrectColorDetected = false; // 색상이 일치하지 않으면 플래그를 false로 설정합니다.
                    // (선택적) 잘못된 색상의 빛이 비춰졌을 때 플레이어에게 피드백을 줄 수 있습니다.
                    // 예: 소리 재생, 트리거 색상 잠시 변경 등
                }
            }
            else
            {
                // 빛 오브젝트는 맞지만 TorchLightController를 찾지 못한 경우 (구조 문제 또는 스크립트 누락)
                Debug.LogWarning("'" + triggerTag + "' 태그를 가진 오브젝트(" + collision.gameObject.name + ")에서 TorchLightController를 찾을 수 없습니다. 구조를 확인해주세요.");
                isCorrectColorDetected = false;
            }
        }
    }

    // 다른 Collider2D가 이 오브젝트의 트리거 영역을 "벗어나는 순간" 한 번 호출됩니다.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 빛 오브젝트가 트리거 영역을 벗어나면, 색상 감지 상태를 false로 초기화합니다.
        if (collision.CompareTag(triggerTag))
        {
            isCorrectColorDetected = false;
        }
    }

    // 두 색상이 지정된 허용 오차(tolerance) 내에서 거의 같은지 비교하는 private 도우미 메서드입니다.
    // Color 구조체의 각 채널(R, G, B, A) 값은 float(부동 소수점)이므로, '==' 연산자로 직접 비교하면
    // 미세한 오차 때문에 정확히 일치하지 않는 경우가 많아 문제가 생길 수 있습니다.
    private bool IsColorMatch(Color c1, Color c2, float tolerance = 0.05f)
    {
        // 각 색상 채널(R, G, B, A) 값의 차이가 tolerance보다 작은지 확인합니다.
        // Mathf.Abs()는 절대값을 반환합니다.
        bool match = Mathf.Abs(c1.r - c2.r) < tolerance &&
                     Mathf.Abs(c1.g - c2.g) < tolerance &&
                     Mathf.Abs(c1.b - c2.b) < tolerance &&
                     Mathf.Abs(c1.a - c2.a) < tolerance; // 알파(투명도) 값도 비교에 포함합니다.
        return match;
    }

    // (선택적) Unity 에디터에서 이 오브젝트가 선택되었을 때 Gizmo를 그려줍니다.
    // 퍼즐의 감지 영역과 필요한 색상을 시각적으로 쉽게 파악할 수 있도록 돕습니다.
    void OnDrawGizmos() // OnDrawGizmosSelected를 사용하면 오브젝트 선택 시에만 보입니다.
    {
        Collider2D col = GetComponent<Collider2D>(); // 이 오브젝트의 Collider2D 컴포넌트를 가져옵니다.
        if (col != null)
        {
            // Gizmo의 색상을 requiredColor와 유사하게 설정하되, 약간의 투명도를 줍니다.
            Gizmos.color = new Color(requiredColor.r, requiredColor.g, requiredColor.b, 0.3f);

            // Collider의 종류에 따라 다른 모양의 Gizmo를 그립니다.
            if (col is BoxCollider2D boxCol) // BoxCollider2D인 경우
            {
                // BoxCollider2D의 실제 위치(오프셋 고려)와 크기에 맞춰 큐브를 그립니다.
                Gizmos.DrawCube(transform.position + (Vector3)boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D circleCol) // CircleCollider2D인 경우
            {
                // CircleCollider2D의 실제 위치(오프셋 고려)와 반지름에 맞춰 구를 그립니다.
                Gizmos.DrawSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
            }
        }
    }
}