using Fungus;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Animator anim;

    private bool isMovementEnabled = true;

    void Start() //хз получаем объект
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    [System.Obsolete]
    void Update() //обработка движений
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        anim.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
        anim.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));

        bool shouldBlockMovement =
            (InventoryManager.Instance != null && InventoryManager.Instance.IsInventoryOpen) ||
            IsFungusBlockPlaying();

        isMovementEnabled = !shouldBlockMovement;

        if (!isMovementEnabled)
        {
            movement = Vector2.zero;
            anim.SetFloat("MoveY", 0);
            anim.SetFloat("MoveX", 0);
            return;
        }

        if (movement.x != 0 && movement.y != 0) { anim.SetFloat("MoveY", 0); }
        

    }

    [System.Obsolete]
    private bool IsFungusBlockPlaying()
    {
        // Новый способ проверки активных блоков в Fungus 3.13.6+
        Flowchart[] flowcharts = FindObjectsOfType<Flowchart>();
        foreach (Flowchart flowchart in flowcharts)
        {
            if (flowchart.GetExecutingBlocks().Count > 0)
                return true;
        }
        return false;
    }
    void FixedUpdate() //двигаемся типо
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }



}


