using Managers;
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
        CheckPlayerHealth();
    }
    private void Start()
    {

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

        if (healthStocks >= maxHealthStocks)
        {
            for (int i = maxHealthStocks; i < healthObjects.Length; i++) // 
            {
                healthObjects[i].SetActive(false);
            }
            healthStocks = maxHealthStocks;
            
        }
        for (int i = 0; i < healthObjects.Length; i++)
        {
            var healthObjectImage = healthObjects[i].GetComponent<Image>();
            healthObjectImage.sprite = i < healthStocks ? fullHeart : emptyHeart;
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
