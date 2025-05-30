/*
Desc: 문의 열림/닫힘 상태를 관리하고 애니메이션을 제어합니다.
씬 전환 로직도 포함될 수 있습니다.
*/
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class DoorController : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    // GameManager 참조 (씬 로딩용)
    public GameManager gameManager; // 인스펙터에서 연결 필요

    // 문이 열렸을 때 다음 씬으로 넘어갈지 여부
    public bool leadsToNextScene = true;
    // 다음 씬의 이름 또는 빌드 인덱스 (선택적, GameManager가 주로 담당)
    // public string nextSceneName;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning(gameObject.name + "에 Animator 컴포넌트가 없습니다.");
        }

        // GameManager 자동 찾기 (씬에 하나만 있다는 가정 하에)
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager를 씬에서 찾을 수 없습니다!");
            }
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            if (animator != null)
            {
                animator.SetTrigger("Open"); // "Open" 트리거를 가진 애니메이션 상태로 전환
            }
            isOpen = true;
            Debug.Log(gameObject.name + " 문이 열렸습니다.");

            // (선택적) 문이 열린 후 바로 다음 씬으로 이동하지 않고, 플레이어가 문에 접촉해야 이동하는 경우
            // 이 로직은 아래 OnTriggerEnter2D로 이동할 수 있습니다.
            // if (leadsToNextScene && gameManager != null)
            // {
            //     gameManager.LoadNextRoom();
            // }
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    // (선택적) 플레이어가 열린 문에 접촉했을 때 다음 씬 로드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen && leadsToNextScene && collision.CompareTag("Player")) // 플레이어 태그 확인
        {
            if (gameManager != null)
            {
                Debug.Log("플레이어가 열린 문 통과. 다음 방으로 이동합니다.");
                gameManager.LoadNextRoom();
            }
            else
            {
                Debug.LogError("GameManager가 연결되지 않아 다음 씬을 로드할 수 없습니다.");
            }
        }
    }

    // (선택적) 문 닫기 기능
    public void CloseDoor()
    {
        if (isOpen)
        {
            if (animator != null)
            {
                animator.SetTrigger("Close"); // "Close" 트리거 (애니메이터에 해당 상태 및 트랜지션 필요)
            }
            isOpen = false;
            Debug.Log(gameObject.name + " 문이 닫혔습니다.");
        }
    }
}