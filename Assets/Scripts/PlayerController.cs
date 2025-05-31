/*
Desc: 플레이어 캐릭터의 이동(좌우, 점프) 및 지면 착지 여부를 관리합니다.
*/
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")] // 인스펙터에서 섹션 제목으로 표시됩니다.
    public float moveSpeed = 5f;    // 플레이어의 좌우 이동 속도입니다.
    public float jumpForce = 10f;   // 플레이어의 점프 힘입니다.

    private Rigidbody2D rb;         // 플레이어의 물리적 움직임을 제어할 Rigidbody2D 컴포넌트입니다.
    private bool isGrounded;        // 플레이어가 현재 지면에 닿아있는지 여부를 나타내는 플래그입니다.

    [Header("Ground 체크")]
    public Transform groundCheck;   // 지면 감지를 위한 기준점 오브젝트의 Transform입니다. (보통 플레이어 발밑)
    public float checkRadius = 0.1f; // groundCheck 위치에서 지면을 감지할 원형 영역의 반지름입니다.
    public LayerMask groundLayer;   // 지면으로 인식할 오브젝트들의 레이어를 지정합니다.

    // 게임이 시작될 때 한 번 호출됩니다. (오브젝트가 활성화될 때)
    void Start()
    {
        // GetComponent<Rigidbody2D>()를 통해 이 스크립트가 붙어있는 게임 오브젝트에서 Rigidbody2D 컴포넌트를 찾아 rb 변수에 할당합니다.
        // 이렇게 찾은 컴포넌트를 통해 물리 효과를 제어할 수 있습니다.
        rb = GetComponent<Rigidbody2D>();

        // groundCheck 오브젝트가 인스펙터에서 할당되지 않았을 경우 경고를 출력합니다.
        if (groundCheck == null)
        {
            Debug.LogError("PlayerController: Ground Check Transform이 할당되지 않았습니다! 플레이어 발밑에 빈 오브젝트를 만들고 할당해주세요.");
        }
    }

    // 매 프레임마다 호출됩니다. 주로 입력 처리나 비물리적인 로직 업데이트에 사용됩니다.
    void Update()
    {
        // 1. 좌우 이동 처리
        // Input.GetAxis("Horizontal")는 수평 입력 축의 값을 반환합니다. (A/D 키, 좌/우 화살표 키, 게임패드 스틱 등)
        // 값의 범위는 -1 (왼쪽) 부터 +1 (오른쪽) 까지입니다. 입력이 없으면 0입니다.
        float moveInput = Input.GetAxis("Horizontal"); // GetAxisRaw를 사용하면 부드러운 가감속 없이 즉각적인 -1, 0, 1 값을 받습니다.

        // Rigidbody2D의 속도(velocity)를 직접 제어하여 플레이어를 이동시킵니다.
        // X축 속도: moveInput * moveSpeed (입력 방향 * 이동 속도)
        // Y축 속도: rb.velocity.y (현재 Y축 속도를 유지하여 중력 등의 효과가 계속 적용되도록 함)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 2. 점프 처리
        // Input.GetButtonDown("Jump")는 "Jump"라는 이름의 가상 버튼이 "눌리는 순간" true를 반환합니다. (기본: 스페이스 바)
        // isGrounded가 true일 때만 (즉, 땅에 있을 때만) 점프가 가능하도록 합니다.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Y축 속도를 순간적으로 0으로 만들어 연속 점프 시 이전 점프의 힘이 누적되는 것을 방지할 수 있습니다. (선택적)
           // rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Rigidbody2D에 위쪽 방향(Vector2.up)으로 jumpForce만큼의 힘을 "순간적으로" 가합니다 (ForceMode2D.Impulse).
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // 고정된 시간 간격으로 호출됩니다. (기본값: 0.02초) 주로 물리 관련 로직 업데이트에 사용됩니다.
    void FixedUpdate()
    {
        // 1. 지면 감지 처리
        // Physics2D.OverlapCircle()은 특정 위치(groundCheck.position)에서 특정 반지름(checkRadius)의 원형 영역 안에
        // 지정된 레이어(groundLayer)에 속하는 Collider가 있는지 검사합니다.
        // 감지되면 isGrounded는 true가 되고, 그렇지 않으면 false가 됩니다.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    // (선택적) 플레이어 발밑의 GroundCheck 영역을 에디터에서 시각적으로 보여주기 위한 Gizmo입니다.
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow; // Gizmo 색상 설정
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius); // groundCheck 위치에 checkRadius 크기의 원을 그림
    }
}