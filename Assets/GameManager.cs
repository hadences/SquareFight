using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private InputAction startAction;

    [Header("References")]
    [SerializeField] private HUD hud;
    [SerializeField] private GameObject box1;
    [SerializeField] private GameObject box2;

    [Header("Game Settings")]
    [SerializeField] private Color box1Color;
    [SerializeField] private Color box2Color;
    [SerializeField] private float speedBuff = 6;

    private BoxComponent box1Comp;
    private BoxComponent box2Comp;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startAction = InputSystem.actions.FindAction("Jump");
        startAction.performed += ctx => startGame();

        // change the color fo the sprites
        box1Comp = box1.GetComponent<BoxComponent>();
        box2Comp = box2.GetComponent<BoxComponent>();

        box1Comp.setColor(box1Color);
        box2Comp.setColor(box2Color);

        hud.box1 = box1; hud.box2 = box2;

        hud.box1Color = box1Color;
        hud.box2Color = box2Color;

        hud.updateBox1Health();
        hud.updateBox2Health();

        box1.SetActive(false);
        box2.SetActive(false);
    }

    private void startGame() {
        hud.showStartTitle(false);

        box1.SetActive(true);
        box2.SetActive(true);
    }

    public void onBoxDamage() {
        hud.updateBox1Health();
        hud.updateBox2Health();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (box1Comp.health > box2Comp.health) {
            box1Comp.currentSpeed = speedBuff;
            box2Comp.currentSpeed = box2Comp.speed;
        }
        else if (box1Comp.health <= box2Comp.health) {
            box2Comp.currentSpeed = speedBuff;
            box1Comp.currentSpeed = box1Comp.speed;
        }
    }
}
