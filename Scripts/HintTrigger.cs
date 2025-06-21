using UnityEngine;
using UnityEngine.UI;

public class HintTrigger : MonoBehaviour
{
    [SerializeField] private Sprite hintImage; // Изображение подсказки
    [SerializeField] private GameObject hintPanel; // UI-панель подсказки
    [SerializeField] private Image hintImageUI; // Компонент Image для отображения

    private bool isHintVisible = false; // Флаг видимости подсказки

    private void Start()
    {
        HideHint(); // Скрываем подсказку при старте
    }

    private void Update()
    {
        // Если подсказка видна и нажата F — скрываем её
        if (isHintVisible && Input.GetKeyDown(KeyCode.F))
        {
            HideHint();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowHint();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideHint();
        }
    }

    private void ShowHint()
    {
        if (hintPanel != null && hintImageUI != null && hintImage != null)
        {
            hintImageUI.sprite = hintImage;
            hintPanel.SetActive(true);
            isHintVisible = true;
        }
    }

    private void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
            isHintVisible = false;
        }
    }
}