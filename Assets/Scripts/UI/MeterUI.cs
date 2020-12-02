using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterUI : MonoBehaviour
{
    public RectTransform centerZoneMeter;
    public RectTransform positiveZoneMeter;
    public RectTransform negativeZoneMeter;
    public RectTransform sliderArea;
    private float _lastCenterZoneSize;
    private float _currentCenterZoneSize;
    private float _totalMeterSize;
    // Start is called before the first frame update
    void Awake()
    {
        _totalMeterSize = centerZoneMeter.rect.width + positiveZoneMeter.rect.width + negativeZoneMeter.rect.width;
        _lastCenterZoneSize = centerZoneMeter.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMeterUI(float currentSpread)
    {
        _currentCenterZoneSize = (_totalMeterSize * currentSpread * 2) / 2;
        float centerSizeDifference = _currentCenterZoneSize - _lastCenterZoneSize;
        centerZoneMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _currentCenterZoneSize);
        positiveZoneMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, positiveZoneMeter.rect.width - (centerSizeDifference/2));
        negativeZoneMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, negativeZoneMeter.rect.width - (centerSizeDifference/2));
        sliderArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _totalMeterSize);
        centerZoneMeter.ForceUpdateRectTransforms();
        positiveZoneMeter.ForceUpdateRectTransforms();
        negativeZoneMeter.ForceUpdateRectTransforms();
        sliderArea.ForceUpdateRectTransforms();
        _lastCenterZoneSize = _currentCenterZoneSize;
    }
}
