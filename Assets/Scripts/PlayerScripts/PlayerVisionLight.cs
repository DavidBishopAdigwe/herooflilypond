using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVisionLight : MonoBehaviour
{
    private static readonly int PlayerLightIntensity = Shader.PropertyToID("player_light_intensity");
    [SerializeField] private LayerMask wallMask; // LayerMask for walls ONLY
    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        Shader.SetGlobalFloat(PlayerLightIntensity, _light.intensity);
    }

    private void CheckEnemyVisibility(Enemy enemy)
    {
        Vector2 direction = enemy.transform.position - transform.position;
        float distance = direction.magnitude;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, wallMask);

        Debug.DrawRay(transform.position, direction, hit.collider ? Color.red : Color.green);

        if (hit.collider == null)
        {
            enemy.ToggleLightSource(true);
        }
        else
        {
            enemy.ToggleLightSource(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy enemy))
        {
            CheckEnemyVisibility(enemy);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy enemy))
        {
            CheckEnemyVisibility(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy enemy))
        {
            enemy.ToggleLightSource(false);
        }
    }
}