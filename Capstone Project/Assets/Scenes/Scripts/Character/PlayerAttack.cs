using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject[] attackAreas; // Array to hold all attack areas
    private bool[] attacking; // Array to track attacking state for each attack
    private float[] timeToAttack; // Array to track time to attack for each attack
    private float[] timers; // Array to track timers for each attack

    void Start()
    {
        int attackCount = 4; // Number of attack areas

        attackAreas = new GameObject[attackCount];
        attacking = new bool[attackCount];
        timeToAttack = new float[attackCount];
        timers = new float[attackCount];

        for (int i = 0; i < attackCount; i++)
        {
            attackAreas[i] = transform.Find($"AttackArea{i}").gameObject;
            attackAreas[i].SetActive(false);
            timeToAttack[i] = 0.25f; // Default time to attack for each attack
            timers[i] = 0f;
        }
    }

    void Update()
    {
        for (int i = 0; i < attackAreas.Length; i++)
        {
            // Check input for each attack
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // Alpha1 corresponds to KeyCode 1
            {
                Attack(i);
                Debug.Log(attackAreas[i].name);
            }

            // Handle attack timing
            if (attacking[i])
            {
                timers[i] += Time.deltaTime;
                if (timers[i] >= timeToAttack[i])
                {
                    timers[i] = 0f;
                    attacking[i] = false;
                    attackAreas[i].SetActive(false);
                }
            }
        }
    }

    private void Attack(int index)
    {
        attacking[index] = true;
        attackAreas[index].SetActive(true);
    }
}
