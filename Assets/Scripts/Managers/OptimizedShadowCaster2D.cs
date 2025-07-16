using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(CompositeCollider2D))]
public class OptimizedShadowCaster2D : MonoBehaviour
{
    [Header("Shadow Settings")]
    [SerializeField] private bool selfShadows = true;
    [SerializeField] private float shadowDistance = 100f;
    [SerializeField] [Range(0.01f, 0.5f)] private float vertexDistance = 0.1f;
    
    [Header("Optimization")]
    [SerializeField] private bool useDistanceCulling = true;
    [SerializeField] private float maxShadowDistance = 50f;
    [SerializeField] private bool disableWhenOffscreen = true;

    private CompositeCollider2D compositeCollider;
    private List<ShadowCaster2D> shadowCasters = new List<ShadowCaster2D>();
    private Camera mainCamera;
    private Plane[] cameraPlanes;

    void Start()
    {
        compositeCollider = GetComponent<CompositeCollider2D>();
        mainCamera = Camera.main;
        
        // Configure composite collider for optimal performance
        compositeCollider.vertexDistance = vertexDistance;
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compositeCollider.GenerateGeometry();
        
        GenerateShadowCasters();
    }

    void GenerateShadowCasters()
    {
        // Clear existing shadow casters
        ClearShadowCasters();

        if (compositeCollider == null || compositeCollider.pathCount == 0) 
            return;

        // Create a root object for all shadow casters
        GameObject shadowRoot = new GameObject("ShadowCasters");
        shadowRoot.transform.SetParent(transform);
        shadowRoot.transform.localPosition = Vector3.zero;
        shadowRoot.transform.localRotation = Quaternion.identity;
        shadowRoot.transform.localScale = Vector3.one;
        
        // Create shadow casters for each collider path
        List<Vector2> pathPoints = new List<Vector2>();
        for (int pathIndex = 0; pathIndex < compositeCollider.pathCount; pathIndex++)
        {
            pathPoints.Clear();
            compositeCollider.GetPath(pathIndex, pathPoints);
            
            if (pathPoints.Count < 3) continue;
            
            // Create shadow caster object
            GameObject shadowObject = new GameObject($"ShadowPath_{pathIndex}");
            shadowObject.transform.SetParent(shadowRoot.transform);
            shadowObject.transform.localPosition = Vector3.zero;
            shadowObject.transform.localRotation = Quaternion.identity;
            shadowObject.transform.localScale = Vector3.one;
            
            // Add required components
            ShadowCaster2D shadowCaster = shadowObject.AddComponent<ShadowCaster2D>();
            shadowCaster.selfShadows = selfShadows;
            shadowCaster.useRendererSilhouette = false;
            
            // Create polygon collider to generate shadow geometry
            PolygonCollider2D polyCollider = shadowObject.AddComponent<PolygonCollider2D>();
            polyCollider.SetPath(0, pathPoints);
            polyCollider.isTrigger = true;
            
            shadowCasters.Add(shadowCaster);
        }
    }

    void ClearShadowCasters()
    {
        foreach (ShadowCaster2D caster in shadowCasters)
        {
            if (caster != null && caster.gameObject != null)
            {
                Destroy(caster.gameObject);
            }
        }
        shadowCasters.Clear();
    }

    void Update()
    {
        if (mainCamera == null) return;
        
        // Update camera planes for culling
        cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        
        foreach (ShadowCaster2D caster in shadowCasters)
        {
            if (caster == null) continue;
            
            bool shouldBeEnabled = true;
            
            // Distance-based culling
            if (useDistanceCulling)
            {
                float distance = Vector3.Distance(caster.transform.position, mainCamera.transform.position);
                shouldBeEnabled = shouldBeEnabled && (distance <= maxShadowDistance);
            }
            
            // Frustum culling
            if (disableWhenOffscreen)
            {
                Bounds bounds = new Bounds(transform.position, compositeCollider.bounds.size);
                shouldBeEnabled = shouldBeEnabled && GeometryUtility.TestPlanesAABB(cameraPlanes, bounds);
            }
            
            caster.enabled = shouldBeEnabled;
        }
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && compositeCollider == null)
        {
            compositeCollider = GetComponent<CompositeCollider2D>();
        }
        
        if (compositeCollider != null)
        {
            compositeCollider.vertexDistance = vertexDistance;
            compositeCollider.GenerateGeometry();
        }
    }
    #endif
}