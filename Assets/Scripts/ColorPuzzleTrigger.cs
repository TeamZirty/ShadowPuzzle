/*
Desc: 특정 색상의 빛이 닿았을 때 연결된 문을 열도록 하는 퍼즐 트리거입니다.
*/
using UnityEngine;

public class ColorPuzzleTrigger : MonoBehaviour
{
    [Header("퍼즐 설정")]
    public DoorController connectedDoor; // 연결된 문 컨트롤러
    public Color requiredColor = Color.red; // 퍼즐 해결에 필요한 빛의 색상

    [Header("디버그용")]
    [SerializeField] // 인스펙터에서 private 필드 보기 (값 변경은 코드에서)
    private bool isTriggeredByCorrectColor = false;

    void Start()
    {
        if (connectedDoor == null)
        {
            Debug.LogError(gameObject.name + "에 DoorController가 연결되지 않았습니다!");
        }
        // 트리거 영역이 보이도록 Gizmo 색상 설정 (에디터에서만 보임)
        GetComponent<SpriteRenderer>().color = new Color(requiredColor.r, requiredColor.g, requiredColor.b, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 빛 태그를 가진 오브젝트와 충돌했는지 확인
        if (collision.CompareTag("Light")) // 빛 오브젝트에는 "Light" 태그를 설정해야 합니다.
        {
            TorchLightController torch = collision.GetComponentInParent<TorchLightController>(); // 빛 오브젝트의 부모에서 TorchLightController 찾기 (구조에 따라 변경 가능)
            // 또는 빛 오브젝트 자체에 SpriteRenderer가 있고 색상 정보를 직접 가지고 있다면 아래처럼 사용
            // SpriteRenderer lightSprite = collision.GetComponent<SpriteRenderer>();

            if (torch != null)
            {
                Color currentLightColor = torch.GetCurrentLightColor();
                // if (lightSprite != null) { Color currentLightColor = lightSprite.color; } // 대체 방식

                // 필요한 색상과 현재 빛의 색상이 일치하는지 확인 (색상 비교 시 근사치 비교가 더 안전할 수 있음)
                if (IsColorMatch(currentLightColor, requiredColor))
                {
                    if (connectedDoor != null)
                    {
                        Debug.Log(gameObject.name + " : 올바른 색상의 빛 감지! 문을 엽니다.");
                        connectedDoor.OpenDoor();
                        isTriggeredByCorrectColor = true;
                        // (선택적) 퍼즐 성공 후 트리거 비활성화 또는 다른 시각적 피드백
                        // gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log(gameObject.name + " : 빛 감지, 하지만 색상 불일치 (필요: " + requiredColor + ", 현재: " + currentLightColor + ")");
                    isTriggeredByCorrectColor = false;
                }
            }
            else
            {
                Debug.LogWarning("충돌한 Light 오브젝트에서 TorchLightController 또는 SpriteRenderer를 찾을 수 없습니다.");
            }
        }
    }

    // 색상 비교 함수 (부동 소수점 오차 감안)
    private bool IsColorMatch(Color c1, Color c2, float tolerance = 0.05f)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance; // 알파값도 비교 (필요에 따라)
    }

    // (선택적) 빛이 트리거 영역에서 나갔을 때 처리
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Light"))
        {
            isTriggeredByCorrectColor = false;
            // 문을 다시 닫아야 한다면 여기에 로직 추가
            // if (connectedDoor != null && connectedDoor.IsOpen()) { connectedDoor.CloseDoor(); }
        }
    }

    // 에디터에서 퍼즐 영역과 필요 색상을 시각적으로 보여주기 위한 Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(requiredColor.r, requiredColor.g, requiredColor.b, 0.5f);
        // Collider2D의 형태에 따라 Gizmo를 다르게 그릴 수 있습니다. BoxCollider2D를 가정합니다.
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else // 기본적으로는 오브젝트 위치에 큐브를 그림
        {
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}