/*
Desc: 게임의 전반적인 흐름(씬 전환, 게임 상태 관리 등)을 담당합니다.
이 스크립트는 씬 전환 시 파괴되지 않고 유지됩니다. (싱글톤)
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // 간단한 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 이 GameManager 오브젝트는 파괴되지 않음
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생긴 것은 파괴 (중복 방지)
            Destroy(gameObject);
        }
    }

    public void LoadNextRoom()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // 빌드 세팅에 다음 씬이 있는지 확인
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log((nextSceneIndex) + "번 씬(방)으로 이동합니다.");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 방입니다! 게임 클리어!");
            // 여기에 게임 클리어 로직 (예: 크레딧 씬 로드, 메인 메뉴로 돌아가기 등)
            // 예시: SceneManager.LoadScene("MainMenu"); // 메인 메뉴 씬 이름이 "MainMenu"일 경우
            // 또는 첫번째 방으로 돌아가게 할 수도 있습니다.
            // SceneManager.LoadScene(0); // 빌드 인덱스 0번 씬으로 이동
        }
    }

    // (선택적) 특정 이름의 씬 로드 함수
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // (선택적) 현재 씬 다시 로드 함수 (재시작 등)
    public void ReloadCurrentRoom()
    {
        Debug.Log(SceneManager.GetActiveScene().name + " 방을 다시 시작합니다.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}