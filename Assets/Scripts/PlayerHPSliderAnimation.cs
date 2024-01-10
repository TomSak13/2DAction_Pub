using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PlayerHPSliderAnimation : MonoBehaviour
{
    private const float BlinkIntervalTime = 0.5f;
    private const int BlinkLoop = 2;

    private CanvasGroup _hpSliderGroup;

    private float _blinkTimer;
    private bool _isBlink;

    private void Start()
    {
        _blinkTimer = 0;
        _isBlink = false;
        _hpSliderGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        MonitorBlinkSlider();
    }

    private void MonitorBlinkSlider()
    {
        if (_isBlink)
        {
            _blinkTimer += Time.deltaTime;
            if (_blinkTimer > BlinkIntervalTime * BlinkLoop)
            {
                ResetSlider();
            }
        }
    }

    private void ResetSlider()
    {
        _blinkTimer = 0;
        _hpSliderGroup.alpha = 1; /* 表示状態に戻す */
        _isBlink = false;
    }

    public void Blink()
    {
        _isBlink = true;
        _hpSliderGroup
            .DOFade(0.0f, BlinkIntervalTime)     // BlinkIntervalTime秒でα値が0になる
            .SetEase(Ease.Flash, 8, 0.4f)        // 高速点滅
            .SetLoops(BlinkLoop, LoopType.Yoyo); // ループ回数
    }
}
