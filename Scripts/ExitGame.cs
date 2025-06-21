using UnityEngine;

public class ExitGame : MonoBehaviour
{
    void Update()
    {
        // Если нажата клавиша ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Выход из игры (работает в собранной версии)
            Application.Quit();

            // Для выхода в редакторе (чтобы тестировать)
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}