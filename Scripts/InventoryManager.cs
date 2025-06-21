using UnityEngine;
using Fungus;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public bool IsInventoryOpen { get; private set; }
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlot[] slots;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private List<Item> items = new List<Item>();
    private Flowchart fungusFlowchart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (inventoryPanel != null)
                DontDestroyOnLoad(inventoryPanel.transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
            Destroy(inventoryPanel); // Удаляем дубликат UI
        }

        // Находим Flowchart один раз при старте
        fungusFlowchart = FindAnyObjectByType<Flowchart>(FindObjectsInactive.Include);
        if (fungusFlowchart == null)
        {
            Debug.LogWarning("Flowchart не найден на сцене!");
        }
    }
  



    private void Start()
    {
        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        IsInventoryOpen = !IsInventoryOpen;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Попытка добавить null предмет!");
            return false;
        }

        if (items.Count >= slots.Length)
        {
            Debug.LogWarning("Инвентарь полон!");
            return false;
        }

        items.Add(item);
        UpdateUI();

        // Вызываем Fungus блок если он указан
        if (!string.IsNullOrEmpty(item.fungusBlock))
        {
            ExecuteFungusBlock(item.fungusBlock);
        }

        return true;
    }

    private void ExecuteFungusBlock(string blockName)
    {
        if (fungusFlowchart != null)
        {
            fungusFlowchart.ExecuteBlock(blockName);
        }
        else
        {
            Debug.LogWarning($"Не удалось выполнить блок Fungus '{blockName}' - Flowchart не найден!");
        }
    }
    public bool HasItem(Item item)
    {
        if (item == null) return false;
        return items.Contains(item);
    }
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
            items.Remove(item);
        UpdateUI();
    }
    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].AddItem(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}