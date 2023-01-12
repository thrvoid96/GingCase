using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BalloonSpawner : MonoBehaviour
{
    [SerializeField] private int maxBallons;
    private Tween spawnTween;
    private void Start()
    {
        spawnTween = DOVirtual.DelayedCall(1f, SpawnBalloon).SetLoops(-1, LoopType.Restart);
    }

    private void SpawnBalloon()
    {
        if (BalloonHolder.Instance.currentBalloons.Count >= maxBallons)
        {
            spawnTween.Kill();
            return;
        }
        ObjectPool.Instance.SpawnFromPool(PoolEnums.Balloon, 
            BalloonHolder.Instance.transform.position + 
            new Vector3(Random.Range(-0.1f,0.1f),0f,Random.Range(-0.1f,0.1f)),
            Quaternion.Euler(new Vector3(-90f,0f,0f)), BalloonHolder.Instance.transform);
    }
}
