using UnityEngine;
using System.Collections.Generic;
using System.Threading;
public class PuzzleController : MonoBehaviour
{
    [Header("Settings")]
    public List<int> correctSequence = new List<int> { 1, 2, 3 };

    [Header("References")]
    public GameObject puzzlePanel;
    public List<GameObject> buttonStars;
    public Fungus.Flowchart flowchart;
    public string blockName;
    public string blockFail;
    private bool playerInZone;
    private List<int> currentSequence = new List<int>();
    private bool puzzleActive = false;
    private void Start()
    {
        puzzlePanel.SetActive(false);
        buttonStars[0].SetActive(false);
        buttonStars[1].SetActive(false);
        buttonStars[2].SetActive(false);
    }
    void Update()
    {
        //открытие/закрытие панели по нажатию F
        if (playerInZone && Input.GetKeyDown(KeyCode.F))
        {
            TogglePuzzlePanel();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("В зоне");
        playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Вышел из зоны");
        playerInZone = false;
    }
    void TogglePuzzlePanel()
    {
        puzzleActive = !puzzleActive;
        puzzlePanel.SetActive(puzzleActive);

        //сброс последовательности при закрытии
        if (!puzzleActive)
        {
            ResetPuzzle();
        }
    }

    //вызывается при нажатии на кнопку
    public void ButtonPressed(int buttonId)
    {
        //добавляем нажатую кнопку в последовательность
        currentSequence.Add(buttonId);
        
        //отображаем звездочку для текущего шага
        if (currentSequence.Count <= correctSequence.Count)
        {
            buttonStars[currentSequence.Count-1].SetActive(true);
        }
        
        //проверяем последовательность
        CheckSequence();
    }

    void CheckSequence()
    {
        //если последовательность достигла нужной длины
        if (currentSequence.Count == 4)
        {
            foreach (GameObject star in buttonStars)
            {
                star.SetActive(true);
            }
            bool correct = true;
            //проверяем каждый элемент
            for (int i = 0; i < 3; i++)
            {
                if (currentSequence[i] != correctSequence[i])
                {
                    correct = false;
                    break;
                }
            }

            if (correct)
            {
                //последовательность правильная
                PuzzleCompleted();
            }
            else
            {
                Thread.Sleep(500);
                //последовательность неправильная
                ResetPuzzle();
            }
        }
    }

    void ResetPuzzle()
    {
        currentSequence.Clear();
        //скрываем все звездочки
        foreach (GameObject star in buttonStars)
        {
            star.SetActive(false);
        }
        puzzleActive = false;
        puzzlePanel.SetActive(false);
        if (flowchart != null && !string.IsNullOrEmpty(blockName))
        {
            flowchart.ExecuteBlock(blockFail);
        }
    }

    void PuzzleCompleted()
    {
        //закрываем панель
        puzzleActive = false;
        puzzlePanel.SetActive(false);
        //вызываем блок Fungus
        if (flowchart != null && !string.IsNullOrEmpty(blockName))
        {
            flowchart.ExecuteBlock(blockName);
        }

        //сбрасываем пазл для возможного повторного использования
        ResetPuzzle();
    }
}