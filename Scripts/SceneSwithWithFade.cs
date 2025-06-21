using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchWithFade : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneName;
    public bool useTrigger = true;
    public bool destroyAfterSwitch = true;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;

    private bool isTransitioning = false;
    private float alpha = 0f;
    private Texture2D fadeTexture;
    private static bool isInitialized = false;
    private int fadeDir = 0; // 1 = fade out, -1 = fade in
    private bool shouldDestroyAfterLoad = false; // Флаг для отложенного удаления
    public static string PreviousScene { get; private set; }
    void Awake()
    {
        if (isInitialized && destroyAfterSwitch)
        {
            Destroy(gameObject);
            return;
        }

        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, Color.white);
        fadeTexture.Apply();

        isInitialized = true;

        if (!destroyAfterSwitch)
        {
            if (transform.parent != null)
                transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        
        if (sceneName == "Tapestries")
        {
            if (SceneManager.GetActiveScene().name == "Codepanel")
            {
                PlayerPrefs.SetInt("FromCodepanelWalk", 1);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetInt("FromCodepanelWalk", 0);
                PlayerPrefs.Save();
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldDestroyAfterLoad)
        {
            Destroy(gameObject);
            return;
        }

        if (!destroyAfterSwitch)
        {
            fadeDir = -1; // Start fade in
            alpha = 1f; // Start from fully faded
        }
    }

    void Update()
    {
        if (fadeDir != 0)
        {
            alpha += fadeDir * Time.unscaledDeltaTime / fadeDuration;
            alpha = Mathf.Clamp01(alpha);

            if (fadeDir == 1 && alpha >= 1f)
            {
                // Fade out complete - load scene
                if (!string.IsNullOrEmpty(sceneName))
                {
                    if (destroyAfterSwitch)
                    {
                        shouldDestroyAfterLoad = true; // Помечаем для удаления после загрузки
                    }
                    SceneManager.LoadScene(sceneName);
                }
                fadeDir = 0;
            }
            else if (fadeDir == -1 && alpha <= 0f)
            {
                // Fade in complete
                fadeDir = 0;
                isTransitioning = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (!useTrigger && !isTransitioning)
            StartFadeOut();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (useTrigger && !isTransitioning && other.CompareTag("Player"))
            StartFadeOut();
    }

    void StartFadeOut()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        fadeDir = 1; // Start fade out
        alpha = 0f;
    }

    void OnGUI()
    {
        if (alpha > 0f)
        {
            GUI.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            GUI.depth = -9999;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }

    private void OnApplicationQuit()
    {
        isInitialized = false;
    }
}