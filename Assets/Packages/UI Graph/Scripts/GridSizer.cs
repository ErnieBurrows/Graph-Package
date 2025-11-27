using System;
using UnityEngine;

[ExecuteAlways]
public class GridSizer : MonoBehaviour
{
    public static event Action OnGridSizeChanged;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnValidate()
    {
        OnGridSizeChanged?.Invoke();
    }

    void OnRectTransformDimensionsChange()
    {
        OnGridSizeChanged?.Invoke();
    }
}
