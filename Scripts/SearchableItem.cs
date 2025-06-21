using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SearchableItem : MonoBehaviour
{
    [SerializeField] private string itemTag;
    private ItemSearchGame gameController;

    private void Start()
    {
        gameController = FindAnyObjectByType<ItemSearchGame>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnMouseDown()
    {
        gameController?.OnItemClicked(itemTag);
    }
}