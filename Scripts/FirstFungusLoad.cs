using UnityEngine;
using Fungus;

public class FungusTriggerOnFirstLoad : MonoBehaviour
{
    [SerializeField] private Flowchart flowchart; // Ссылка на Flowchart Fungus
    [SerializeField] private string blockName; // Имя блока, который нужно выполнить

    private static bool hasBeenLoadedBefore = false;

    private void Start()
    {
        if (!hasBeenLoadedBefore && flowchart != null && !string.IsNullOrEmpty(blockName))
        {
            // Находим блок по имени и выполняем его
            var block = flowchart.FindBlock(blockName);
            if (block != null)
            {
                flowchart.ExecuteBlock(blockName);
            }

            hasBeenLoadedBefore = true;
        }
    }
}