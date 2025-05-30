/*
Desc: 게임의 전반적인 흐름(씬 전환 등)을 관리합니다.
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스 (필요에 따라)
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // 간단한 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextRoom()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log((nextSceneIndex) + "번 씬(방)으로 이동합니다.");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 방입니다! 게임 클리어!");
            // 여기에 게임 클리어 로직 (예: 크레딧 씬 로드, 메인 메뉴로 돌아가기 등)
            // SceneManager.LoadScene("MainMenu"); // 예시
        }
    }

    public void ReloadCurrentRoom()
    {
        Debug.Log(SceneManager.GetActiveScene().name + " 방을 다시 시작합니다.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}