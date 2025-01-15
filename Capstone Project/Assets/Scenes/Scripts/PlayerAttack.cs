using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   private GameObject attackArea = default;
   private bool attacking = false;
   private float timer= 0f;
    void Start()
    {
        attackArea = transform.GetChild(0).GameObject;
    }
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        if (attacking)
        {
            timer += timer.deltaTime;
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

