using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SinglePlayerAttack : MonoBehaviour
{
    public Animator animator;
    [Header("Attack Types")]
    public string[] attackTypes;

    [Header("Player Attack Manager")]
    public PlayerAttackManager attackManager;

    [SerializeField] public bool attackChecker;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2) //prevents player from using attacks in scene 2
        {
            attackChecker = false;
        }
        else
        {
            attackChecker = true; //keep this even if you want the player to attack in scene 2
        }

        if (attackManager == null)
            attackManager = GetComponent<PlayerAttackManager>();

        if (attackTypes.Length == 0)
            Debug.LogWarning("No attack types assigned. Please assign them in the Inspector.");
    }

    void Update()
    {
        if (!attackChecker || EventSystem.current.IsPointerOverGameObject())
            return;

        if (attackChecker == true)
        {

            if (attackTypes.Length > 0 && Input.GetMouseButtonDown(0)) // Left Click
                attackManager.TriggerAttack(attackTypes[0]);

            if (attackTypes.Length > 1 && Input.GetMouseButtonDown(1)) // Right Click
                attackManager.TriggerAttack(attackTypes[1]);

            if (attackTypes.Length > 2 && Input.GetKeyDown(KeyCode.E)) // E key
                attackManager.TriggerAttack(attackTypes[2]);

            if (attackTypes.Length > 3 && Input.GetKeyDown(KeyCode.Q)) // Q key
                attackManager.TriggerAttack(attackTypes[3]);
        }
    }
}