using Singletons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Linq;

public class Health : MonoBehaviour
{
    [SerializeField] private int healthStocks;
    [SerializeField] private GameObject[] healthObjects;
    [SerializeField] private int maxHealthStocks;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    public UnityEvent onDamaged;

    private void Awake()
    {
        healthStocks = healthObjects.Length;

        foreach (var hp in healthObjects)
        {
            if (hp.TryGetComponent(out HealthObject health))
            {
                health.SetHeartToFull();
            }
        }
        CheckPlayerHealth();
    }
    

    public void TakeDamage(int damage)
    {
        if (healthStocks < 0) return;
        healthStocks -= damage; 
        onDamaged.Invoke();
        PlayerDamaged(); // Subscribe to this
        if (healthStocks <= 0)
        {
            Die();
        }
    }

    public void CheckPlayerHealth()
    {

        if (healthStocks >= maxHealthStocks)
        {
            for (int i = maxHealthStocks; i < healthObjects.Length; i++) 
            {
                healthObjects[i].SetActive(false);
            }

            healthStocks = maxHealthStocks;
        }
        
        for (int i = 0; i < healthObjects.Length; i++)
        {
            if (i == healthStocks && healthObjects[i].TryGetComponent(out HealthObject healthObject))
            {
                healthObject.DamagedAnimation();
            }
        }

    }
    
    public bool IsMax() => healthStocks >= maxHealthStocks;

    public void PlayerDamaged()
    {
       
    }

public void AddHealth(int hp)
    {
        if (healthStocks >= maxHealthStocks) return;
        healthStocks += hp;
        for (int i = 0; i < healthObjects.Length; i++)
        {
            if (i == healthStocks - 1 && healthObjects[i].TryGetComponent(out HealthObject healthObject))
            {
                healthObject.HealedAnimation();
            }
        }
        CheckPlayerHealth();
    }
    
    
    private void Die()
    {
        GameManager.Instance.GameOver();
    }

    
}
