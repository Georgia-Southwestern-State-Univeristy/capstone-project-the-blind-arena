using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] private int health = 100;
    [SerializeField] private int stamina = 100;
    private int MAX_HEALTH = 100;
    private int MAX_STAMINA = 100;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Damage(10);
        }
                if (Input.GetKeyDown(KeyCode.X))
        {
            Heal(10);
        }
        
    }
    public void Damage(int amount)
    {
        if(amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");
        }
        this.health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("I have died");
        Destroy(gameObject);
    }
    public void Heal(int amount)
    {
        if(amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative Healing");
        }

        if(health + amount > MAX_HEALTH)
        {
            this.health = MAX_HEALTH;
        } else 
        {
            this.health += amount;
        }

    }

}
