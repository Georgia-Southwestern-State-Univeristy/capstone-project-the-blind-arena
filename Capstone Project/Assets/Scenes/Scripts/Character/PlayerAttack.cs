using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   [SerializeField] private GameObject attackArea = default;
   private bool attacking = false;
   private float timeToAttack = 0.25f;
   private float timer= 0f;
    void Start()
    {
        attackArea = transform.Find("AttackArea").gameObject;
        attackArea.SetActive(false); 
    }
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
            Debug.Log(attackArea.name);
        }
        if (attacking)
        {
            timer += Time.deltaTime;
            if(timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
            }
        }
    }
    private void Attack(){
        attacking = true;
        attackArea.SetActive(attacking);
    }

}

