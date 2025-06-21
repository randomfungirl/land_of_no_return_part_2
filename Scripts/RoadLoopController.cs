using UnityEngine;
using Fungus;
using UnityEngine.SceneManagement;

public class RoadTeleportSystem : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private Transform upSpawnPoint;
    [SerializeField] private Transform downSpawnPoint;

    [Header("Fungus Settings")]
    [SerializeField] private string startGameBlock = "Start_game";
    [SerializeField] private int requiredPasses = 10;

    [Header("Scene Settings")]
    [SerializeField] private string firstSceneToLoad = "FirstScene";
    [SerializeField] private string secondSceneToLoad = "SecondScene";

    private static bool triggersDisabled = false;
    private static bool isFirstCompletion = true; // Заменили alreadyCompleted
    private static int totalPasses = 0;
    private Flowchart flowchartCache;
    private Collider2D[] allTriggers;
    private Collider2D upTrigger;

    private void Start()
    {
        flowchartCache = FindAnyObjectByType<Flowchart>();
        allTriggers = FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        allTriggers = System.Array.FindAll(allTriggers, col =>
            col.isTrigger && col.CompareTag("RoadTrigger"));

        upTrigger = System.Array.Find(allTriggers, col =>
            col.name.ToLower().Contains("uptrigger"));

        if (triggersDisabled)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = upSpawnPoint.position;
            }

            foreach (var trigger in allTriggers)
            {
                if (trigger != upTrigger)
                {
                    trigger.enabled = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggersDisabled && gameObject.name.ToLower() == "uptrigger")
        {
            DisableFungus();
            string sceneToLoad = isFirstCompletion ? firstSceneToLoad : secondSceneToLoad;
            Debug.Log($"Loading {(isFirstCompletion ? "FIRST" : "SECOND")} scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);

            // Сбрасываем только после успешной загрузки первой сцены
            if (isFirstCompletion)
            {
                isFirstCompletion = false;
            }
            return;
        }

        if (triggersDisabled) return;

        switch (gameObject.name.ToLower())
        {
            case "lefttrigger":
                TeleportPlayer(other.transform, rightSpawnPoint);
                break;
            case "righttrigger":
                TeleportPlayer(other.transform, leftSpawnPoint);
                break;
            case "uptrigger":
                TeleportPlayer(other.transform, downSpawnPoint);
                break;
            case "downtrigger":
                TeleportPlayer(other.transform, upSpawnPoint);
                break;
        }
    }

    private void TeleportPlayer(Transform player, Transform targetSpawnPoint)
    {
        player.position = targetSpawnPoint.position;
        totalPasses++;
        Debug.Log($"Total passes: {totalPasses}/{requiredPasses}");

        if (totalPasses >= requiredPasses && !triggersDisabled)
        {
            triggersDisabled = true;
            DisableAllTriggersExceptUp();

            if (isFirstCompletion && flowchartCache != null && !string.IsNullOrEmpty(startGameBlock))
            {
                Debug.Log("Executing Fungus block for first completion");
                flowchartCache.ExecuteBlock(startGameBlock);
            }
        }
    }

    private void DisableAllTriggersExceptUp()
    {
        foreach (var trigger in allTriggers)
        {
            if (trigger != null && trigger != upTrigger)
            {
                trigger.enabled = false;
            }
        }
    }

    private void DisableFungus()
    {
        if (flowchartCache != null)
        {
            flowchartCache.StopAllBlocks();
        }

        var dialogInput = FindAnyObjectByType<DialogInput>();
        if (dialogInput != null)
        {
            dialogInput.enabled = false;
        }
    }

    public static void ResetSystem()
    {
        triggersDisabled = false;
        totalPasses = 0;
        isFirstCompletion = true;
        Debug.Log("System reset - all flags cleared");
    }
}