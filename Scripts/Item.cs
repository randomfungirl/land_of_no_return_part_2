using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public string fungusBlock; // Имя блока Fungus, который нужно вызвать при подборе
}