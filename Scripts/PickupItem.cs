using Fungus;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;
    private bool isPlayerInTrigger = false;
    public Flowchart flowchart;
    public string afterPickupBlock = "AfterPickup";
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок в зоне");
            isPlayerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вышел из зоны");
            isPlayerInTrigger = false;
        }
    }

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Собираем предмет...");
            bool wasPickedUp = InventoryManager.Instance.AddItem(item);
            if (wasPickedUp)
            {
                flowchart?.ExecuteBlock(afterPickupBlock);
                Destroy(gameObject);
            }
        }
    }
}