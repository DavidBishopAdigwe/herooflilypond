using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int healthStocks; 
    [SerializeField] private Image[] healthObjects;
    [SerializeField] private int maxHealthStocks;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    
    public UnityEvent onDamaged;

    private void Awake()
    {
        healthStocks = maxHealthStocks;
    }
    private void Start()
    {
        healthStocks = maxHealthStocks;
        CheckPlayerHealth();
    }

    public void TakeDamage(int damage)
    {
        if (healthStocks < 0) return;
        healthStocks -= damage;
        if (healthStocks <= 0)
        {
            Die();
            return;
        } 
        onDamaged.Invoke();
    }
    public void CheckPlayerHealth()
    {
        if (healthStocks >= maxHealthStocks) healthStocks = maxHealthStocks;
        for (int i = 0; i < healthObjects.Length; i++)
        {
            if (i < healthStocks)
            {
                healthObjects[i].sprite = fullHeart;
            }
            else
            {
                healthObjects[i].sprite = emptyHeart;
            }
        }
    }
    public void AddHealth(int hp)
    {
        if (healthStocks >= maxHealthStocks) return;
        healthStocks += hp;
        CheckPlayerHealth();
    }
    

   

    private void Die()
    {
        GameManager.Instance.GameOver();
    }

    
}
