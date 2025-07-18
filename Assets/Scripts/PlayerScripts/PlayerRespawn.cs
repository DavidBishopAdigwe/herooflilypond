using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachinePositionComposer playerComposer;
    
    private LensSettings _lens;

    private void Awake()
    {
        _lens = playerCam.Lens;
    }

    private void Start()
    {
        Respawn();
    }

    public void Respawn()
    {
        _lens.OrthographicSize = 1.5f;
        playerComposer.Composition.ScreenPosition = Vector2.zero;
        playerCam.Lens = _lens;
        gameObject.transform.position = spawnPoint.position;
    }
}