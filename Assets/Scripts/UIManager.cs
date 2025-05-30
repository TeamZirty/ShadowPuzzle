using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI timerText;

    private float timer;


    private void Update()
    {
        timer += Time.deltaTime;
        timerText.text = $"경과 시삭: (timer:F1)초";
    }

    // Update is called once per frame
    public void ShowHint(string hint)
    {
        hintText.text = hint;
    }
}
