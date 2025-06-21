using UnityEngine;
using Fungus;
using System.Collections.Generic;

public class ItemSearchGame : MonoBehaviour
{
    [System.Serializable]
    public class SearchStage
    {
        public string requiredItemTag;
        public string preSearchBlock;
        public string postSearchBlock;
        [HideInInspector] public bool itemFound = false;
    }

    [SerializeField] private List<SearchStage> stages = new List<SearchStage>();
    private int currentStage = 0;
    private Flowchart flowchart;
    private bool isExecutingBlock = false;

    // Свойство для проверки, выполняется ли сейчас блок Fungus
    public static bool IsFungusBlockExecuting { get; private set; }

    private void Start()
    {
        flowchart = FindAnyObjectByType<Flowchart>();
        StartCurrentStage();
    }

    private void StartCurrentStage()
    {
        if (currentStage >= stages.Count)
        {
            Debug.Log("Все предметы найдены!");
            return;
        }

        ExecuteFungusBlock(stages[currentStage].preSearchBlock, () =>
        {
            // Коллбэк после выполнения preSearchBlock
            isExecutingBlock = false;
            IsFungusBlockExecuting = false;
        });
    }

    public void OnItemClicked(string itemTag)
    {
        if (IsFungusBlockExecuting || isExecutingBlock || currentStage >= stages.Count) return;

        if (itemTag == stages[currentStage].requiredItemTag && !stages[currentStage].itemFound)
        {
            stages[currentStage].itemFound = true;
            isExecutingBlock = true;
            IsFungusBlockExecuting = true;

            ExecuteFungusBlock(stages[currentStage].postSearchBlock, () =>
            {
                // Коллбэк после выполнения postSearchBlock
                isExecutingBlock = false;
                IsFungusBlockExecuting = false;
                currentStage++;
                StartCurrentStage();
            });
        }
    }

    private void ExecuteFungusBlock(string blockName, System.Action onComplete = null)
    {
        if (string.IsNullOrEmpty(blockName))
        {
            onComplete?.Invoke();
            return;
        }

        // Получаем блок и выполняем его
        Block block = flowchart.FindBlock(blockName);
        if (block == null)
        {
            Debug.LogError($"Блок Fungus '{blockName}' не найден!");
            onComplete?.Invoke();
            return;
        }

        // Запускаем блок
        flowchart.ExecuteBlock(block);
        IsFungusBlockExecuting = true;

        // Для отслеживания завершения блока используем корутину
        StartCoroutine(WaitForBlockCompletion(block, onComplete));
    }

    private System.Collections.IEnumerator WaitForBlockCompletion(Block block, System.Action onComplete)
    {
        // Ждем пока блок выполняется
        while (block.IsExecuting())
        {
            yield return null;
        }

        // Когда блок завершился
        onComplete?.Invoke();
    }
}