using UnityEngine;

public class StarterMessage : MonoBehaviour
{
    
    // Prototype only, telling player controls and goal
    public GameObject textMesh;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textMesh.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textMesh.SetActive(false);
        }
    }
}
