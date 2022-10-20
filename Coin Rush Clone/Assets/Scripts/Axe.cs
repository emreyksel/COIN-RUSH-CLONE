using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Axe : MonoBehaviour
{
    public Ease ease;
    private float time = 2;

    private void Start()
    {
        transform.DORotate(new Vector3(0, 90, 0), time).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
    }
}
