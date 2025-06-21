using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnController : MonoBehaviour
{
    public static string PreviousScene { get; private set; }

    [Header("Spawn Points")]
    public Transform defaultSpawnPoint;
    public Transform customSpawnPoint;

    [Header("Player Sprites")]
    public Sprite defaultSprite;
    public Sprite sideSprite;

    private GameObject player;
    private SpriteRenderer playerSpriteRenderer;
    private bool DifferentSpawn;
    private Animator playerAnimator; // Добавляем проверку на аниматор

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure your player has 'Player' tag.");
            return;
        }

        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("Player doesn't have a SpriteRenderer!");
            return;
        }


        playerAnimator = player.GetComponent<Animator>();
        bool wasAnimatorEnabled = playerAnimator != null && playerAnimator.enabled;
        if (playerAnimator != null)
            playerAnimator.enabled = false;

        // Загрузка сохраненного состояния
        DifferentSpawn = PlayerPrefs.GetInt("FromCodepanelWalk", 0) == 1;


        // Устанавливаем позицию и спрайт
        if (DifferentSpawn)
        {
            player.transform.position = customSpawnPoint.position;
            playerSpriteRenderer.sprite = sideSprite;
        }
        else 
        {
            player.transform.position = defaultSpawnPoint.position;
            playerSpriteRenderer.sprite = defaultSprite;
        }

        if (playerAnimator != null && wasAnimatorEnabled)
            playerAnimator.enabled = true;
    }
}