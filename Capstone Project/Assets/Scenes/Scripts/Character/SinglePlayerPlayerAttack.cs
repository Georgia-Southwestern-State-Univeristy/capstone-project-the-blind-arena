using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

    [Header("Attack Audio")]
    public AudioSource attackAudioSource;
    public AudioClip[] attackAudios;

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
        if (!attackChecker)
            return;

        if (IsPointerOverUIExcluding("EnemyTracker"))
            return;

        if (attackTypes.Length > 0 && Input.GetMouseButtonDown(0))
        {
            if (attackManager.CanUseAttack(attackTypes[0]))
            {
                attackManager.TriggerAttack(attackTypes[0]);

                if (attackManager.playattacksound == true)
                {
                    PlayAttackSound(0);
                }
            }
        }

        if (attackTypes.Length > 1 && Input.GetMouseButtonDown(1))
        {
            if (attackManager.CanUseAttack(attackTypes[1]))
            {
                attackManager.TriggerAttack(attackTypes[1]);

                if (attackManager.playattacksound == true)
                {
                    PlayAttackSound(1);
                }
            }
        }

        if (attackTypes.Length > 2 && Input.GetKeyDown(KeyCode.E))
        {
            if (attackManager.CanUseAttack(attackTypes[2]))
            {
                attackManager.TriggerAttack(attackTypes[2]);

                if (attackManager.playattacksound == true)
                {
                    PlayAttackSound(2);
                }
            }
        }

        if (attackTypes.Length > 3 && Input.GetKeyDown(KeyCode.Q))
        {
            if (attackManager.CanUseAttack(attackTypes[3]))
            {
                attackManager.TriggerAttack(attackTypes[3]);

                if (attackManager.playattacksound == true)
                {
                    PlayAttackSound(3);
                }
            }
        }
    }

    void PlayAttackSound(int index)
    {
        if (attackAudioSource != null && attackAudios.Length > index && attackAudios[index] != null)
        {
            attackAudioSource.PlayOneShot(attackAudios[index], attackAudioSource.volume * 0.5f); // Lower volume by 50%
        }
    }


    private bool IsPointerOverUIExcluding(string tagToIgnore)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (!result.gameObject.CompareTag("EnemyTracker"))
            {
                return true; // Over a UI element that's NOT the enemy tracker
            }
        }

        return false; // Only over ignored element or nothing at all
    }

}