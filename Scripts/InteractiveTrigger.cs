using UnityEngine;
using Fungus;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class AdvancedInteractiveTrigger : MonoBehaviour
{
    [Header("Настройки предметов")]
    public bool shouldAddItem = false;
    public Item itemToAdd;
    public bool requireItem = false;
    public Item requiredItem;
    public KeyCode interactionKey = KeyCode.F;

    [Header("Блоки Fungus")]
    public string initialInteractionBlock = "FirstInteraction";
    public string itemMissingBlock = "ItemMissing";
    public string itemAlreadyExistsBlock = "ItemExists";
    public string afterPickupBlock = "AfterPickup";
    public string imageRevealBlock = "ImageReveal";
    public string afterImageBlock = "AfterImage";

    [Header("Настройки изображения")]
    public bool shouldRevealImage = false;
    public GameObject imageObject;
    public float imageRevealDelay = 0.5f;

    // Состояния
    private bool playerInZone;
    private bool itemWasAdded;
    private bool imageWasRevealed;
    private Flowchart flowchart;

    // Уникальный идентификатор для сохранения
    private string saveKey;

    private void Start()
    {
        flowchart = FindFirstObjectByType<Flowchart>();
        saveKey = $"{gameObject.scene.name}_{gameObject.name}_imageRevealed";

        // Загрузка сохраненного состояния
        imageWasRevealed = PlayerPrefs.GetInt(saveKey, 0) == 1;

        // Инициализация изображения
        if (imageObject != null)
        {
            imageObject.SetActive(imageWasRevealed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = false;
    }

    private void Update()
    {
        if (!playerInZone || !Input.GetKeyDown(interactionKey)) return;

        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        // 1. Проверка необходимого предмета
        if (requireItem && !InventoryManager.Instance.HasItem(requiredItem))
        {
            ExecuteFungusBlock(itemMissingBlock);
            yield break;
        }

        // 2. Проверка наличия предмета в инвентаре
        if (shouldAddItem && itemToAdd != null)
        {
            if (InventoryManager.Instance.HasItem(itemToAdd))
            {
                ExecuteFungusBlock(itemAlreadyExistsBlock);
                yield break;
            }
        }

        // 3. Основное взаимодействие
        yield return StartCoroutine(ExecuteFungusBlockWithWait(initialInteractionBlock));

        // 4. Добавление предмета
        if (shouldAddItem && itemToAdd != null && !itemWasAdded)
        {
            InventoryManager.Instance.AddItem(itemToAdd);
            itemWasAdded = true;

            // Блок после подбора предмета
            if (!string.IsNullOrEmpty(afterPickupBlock))
            {
                yield return StartCoroutine(ExecuteFungusBlockWithWait(afterPickupBlock));
            }
        }

        // 5. Показ изображения
        if (shouldRevealImage && imageObject != null && !imageWasRevealed)
        {
            // Сначала выполняем блок перед показом изображения
            if (!string.IsNullOrEmpty(imageRevealBlock))
            {
                yield return StartCoroutine(ExecuteFungusBlockWithWait(imageRevealBlock));
            }

            // Затем показываем изображение
            yield return new WaitForSeconds(imageRevealDelay);
            imageObject.SetActive(true);
            imageWasRevealed = true;

            // Сохраняем состояние
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();
        }

        // 6. Последующие взаимодействия
        if (imageWasRevealed && !string.IsNullOrEmpty(afterImageBlock))
        {
            ExecuteFungusBlock(afterImageBlock);
        }
    }

    private IEnumerator ExecuteFungusBlockWithWait(string blockName)
    {
        if (string.IsNullOrEmpty(blockName)) yield break;
        if (flowchart == null) yield break;

        flowchart.ExecuteBlock(blockName);
        yield return new WaitWhile(() => flowchart.HasExecutingBlocks());
    }

    private void ExecuteFungusBlock(string blockName)
    {
        if (string.IsNullOrEmpty(blockName)) return;
        flowchart?.ExecuteBlock(blockName);
    }

    // Метод для сброса состояния (если нужно)
    public void ResetTrigger()
    {
        itemWasAdded = false;
        imageWasRevealed = false;
        PlayerPrefs.DeleteKey(saveKey);

        if (shouldRevealImage && imageObject != null)
        {
            imageObject.SetActive(false);
        }
    }
}