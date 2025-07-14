using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachinePositionComposer playerComposer;
    [SerializeField] private LensSettings _lens;

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
        playerComposer.Composition.ScreenPosition = new Vector2(0, 0);
        playerCam.Lens = _lens;
        gameObject.transform.position = spawnPoint.position;
    }
}