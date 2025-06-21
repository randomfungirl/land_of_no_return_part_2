// InventorySlot.cs (исправленная версия)
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon; // Добавляем сериализуемое поле

    private void Awake()
    {
        // Автоматически находим иконку, если не назначена
        if (icon == null)
            icon = GetComponentInChildren<Image>();
    }

    public void AddItem(Item newItem)
    {
        // Добавляем проверки на null
        if (newItem == null)
        {
            Debug.LogWarning("Попытка добавить null предмет в слот!");
            return;
        }

        if (icon == null)
        {
            Debug.LogError("Иконка слота не назначена!");
            return;
        }

        icon.sprite = newItem.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}