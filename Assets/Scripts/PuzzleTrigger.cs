/*
Desc: 특정 태그를 가진 오브젝트(예: 빛)가 자신의 영역에 들어오면 연결된 문을 엽니다.
*/
using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("설정")]
    public DoorController connectedDoor; // 연결할 문 오브젝트의 DoorController
    public string triggerTag = "Light"; // 감지할 오브젝트의 태그
    public bool disableAfterTrigger = true; // 한 번 작동 후 비활성화 여부

    private bool isTriggered = false;

    void Start()
    {
        if (connectedDoor == null)
        {
            Debug.LogError(gameObject.name + "에 DoorController가 연결되지 않았습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered && disableAfterTrigger) return; // 이미 작동했고, 비활성화 설정이면 더 이상 실행 안 함

        if (collision.CompareTag(triggerTag))
        {
            if (connectedDoor != null)
            {
                Debug.Log(gameObject.name + ": '" + triggerTag + "' 태그 감지! 문(" + connectedDoor.gameObject.name + ")을 엽니다.");
                connectedDoor.OpenDoor();
                isTriggered = true;

                if (disableAfterTrigger)
                {
                    gameObject.SetActive(false); // 퍼즐 트리거 비활성화 (선택적)
                }
            }
            else
            {
                Debug.LogError(gameObject.name + "에 연결된 문이 없습니다!");
            }
        }
    }

    // 에디터에서 영역을 보기 쉽게 Gizmo로 표시 (선택적)
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // 초록색 반투명
            if (col is BoxCollider2D boxCol)
            {
                Gizmos.DrawCube(transform.position + (Vector3)boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D circleCol)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
            }
        }
    }
}