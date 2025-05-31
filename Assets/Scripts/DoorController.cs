/*
Desc: 문의 열림/닫힘 상태를 관리하고 애니메이션을 제어합니다.
플레이어가 열린 문을 통과할 경우 GameManager를 통해 다음 씬(방)으로 전환하는 로직도 포함합니다.
*/
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 UnityEngine.SceneManagement 네임스페이스를 사용합니다.

public class DoorController : MonoBehaviour
{
    private Animator animator;          // 문 오브젝트의 애니메이션을 제어할 Animator 컴포넌트입니다.
    private bool isOpen = false;        // 문이 현재 열려있는 상태인지 나타내는 내부 플래그입니다.

    // GameManager에 대한 참조입니다. 씬 전환 기능을 사용하기 위해 필요합니다.
    // public으로 선언되어 인스펙터에서 할당하거나, Start()에서 자동으로 찾으려고 시도합니다.
    public GameManager gameManager;

    // 이 문이 다음 씬(방)으로 이어지는 문인지 여부를 결정합니다.
    // true이면 플레이어가 열린 문에 닿았을 때 다음 씬으로 이동을 시도합니다.
    public bool leadsToNextScene = true;

    // 스크립트 인스턴스가 처음 로드될 때 호출됩니다.
    void Start()
    {
        // 이 게임 오브젝트에 연결된 Animator 컴포넌트를 찾아 animator 변수에 할당합니다.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            // Animator가 없어도 게임이 멈추지는 않지만, 문 열림 애니메이션은 작동하지 않습니다.
            Debug.LogWarning(gameObject.name + "에 Animator 컴포넌트가 없습니다. 애니메이션 없이 작동합니다.");
        }

        // gameManager가 인스펙터에서 미리 할당되지 않았다면, 현재 씬에서 GameManager 타입의 오브젝트를 찾습니다.
        // 이는 GameManager가 씬에 하나만 존재하고, 다른 스크립트에서 쉽게 접근할 수 있도록 싱글톤 등으로 관리될 때 유용합니다.
        if (gameManager == null)
        {
            gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                // GameManager를 찾지 못하면 씬 전환 기능은 사용할 수 없지만, 문 열림 자체는 가능합니다.
                Debug.LogWarning(gameObject.name + ": GameManager를 씬에서 찾을 수 없습니다. 씬 전환 기능이 작동하지 않을 수 있습니다.");
            }
        }
    }

    // 외부에서 문을 열도록 호출하는 public 메서드입니다. (예: PuzzleTrigger에서 호출)
    public void OpenDoor()
    {
        // 문이 아직 열려있지 않다면 (중복 실행 방지)
        if (!isOpen)
        {
            // Animator 컴포넌트가 있다면 "Open"이라는 이름의 트리거 파라미터를 발동시켜 애니메이션을 재생합니다.
            // (Animator Controller 내에 "Open" 트리거와 해당 트랜지션이 설정되어 있어야 합니다.)
            if (animator != null)
            {
                animator.SetTrigger("Open");
            }
            isOpen = true; // 문의 상태를 '열림'으로 변경합니다.
            Debug.Log(gameObject.name + " 문이 열렸습니다.");

            // 여기에 문 열림 효과음 재생 등의 추가 로직을 넣을 수 있습니다.
            // 예: if (soundManager != null) soundManager.PlaySFX("doorOpen");
        }
    }

    // 현재 문이 열려있는지 상태를 외부에서 확인할 수 있는 public 메서드입니다. (선택적)
    public bool IsOpen()
    {
        return isOpen;
    }

    // 이 오브젝트의 Collider2D(IsTrigger가 true로 설정된)에 다른 Collider2D가 들어왔을 때 호출됩니다.
    // 주로 플레이어가 열린 문을 통과하는 것을 감지하기 위해 사용됩니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 문이 열려 있고(isOpen), 이 문이 다음 씬으로 연결되는 문이며(leadsToNextScene),
        // 들어온 오브젝트의 태그가 "Player"일 경우에만 다음 씬으로 이동합니다.
        if (isOpen && leadsToNextScene && collision.CompareTag("Player"))
        {
            // GameManager 참조가 유효하다면, 다음 방(씬)을 로드하도록 요청합니다.
            if (gameManager != null)
            {
                Debug.Log("플레이어가 열린 문 통과. 다음 방(" + (SceneManager.GetActiveScene().buildIndex + 1) + "번 인덱스 씬)으로 이동합니다.");
                gameManager.LoadNextRoom();
            }
            else
            {
                // GameManager가 없다면 씬 전환은 불가능합니다.
                Debug.LogError(gameObject.name + ": GameManager가 연결되지 않아 다음 씬을 로드할 수 없습니다.");
            }
        }
    }
}