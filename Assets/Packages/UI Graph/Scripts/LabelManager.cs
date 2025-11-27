#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
[ExecuteAlways]
public class LabelManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private RectTransform gridRect;
    [SerializeField] private UIGridRenderer gridRenderer;
    private Vector2Int gridSize;
    private Vector2 maxValue;

    [Header("Label References")]
    [SerializeField] private GameObject xLabelPrefab;
    [SerializeField] private GameObject yLabelPrefab;       
    [SerializeField] private Transform xLabelParent, yLabelParent;    

    [Header("Object Cleanup")]
    private List<TMP_Text> xLabels = new List<TMP_Text>();
    private List<TMP_Text> yLabels = new List<TMP_Text>();
    private bool _needsUpdate;

    private void OnEnable()
    {
        UIGridRenderer.OnGridValuesChanged += ScheduleUpdate;
        GridSizer.OnGridSizeChanged += ScheduleUpdate;
    }

    private void OnDisable()
    {
        UIGridRenderer.OnGridValuesChanged -= ScheduleUpdate;
        GridSizer.OnGridSizeChanged -= ScheduleUpdate;
    }

    private void OnDestroy()
    {
    #if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
    #endif
    }

    private void ScheduleUpdate()
    {
#if UNITY_EDITOR
        _needsUpdate = true;
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;
#else
        UpdateLabels();
#endif
    }

    
#if UNITY_EDITOR
    private void EditorUpdate()
    {
        if (this == null || transform == null)
        {
            EditorApplication.update -= EditorUpdate;
            return;
        }

        if (!_needsUpdate || !transform.parent || !transform.parent.gameObject.activeSelf) return;

        _needsUpdate = false;

        HandleGridChanged();

        EditorApplication.update -= EditorUpdate;
    }
#endif

    private void Awake()
    {
        gridSize = gridRenderer.gridSize;
        maxValue = gridRenderer.maxValues;
    }

    private void HandleGridChanged()
    {
        UIGridRenderer grid = gridRenderer.GetComponent<UIGridRenderer>();

        if (grid == null) return;

        gridSize = gridRenderer.gridSize;
        maxValue = gridRenderer.maxValues;

        UpdateXLabels();
        UpdateYLabels();
    }

    public void UpdateYLabels()
    {
        if (gridRect == null || yLabelPrefab == null || yLabelParent == null)
            return; 

        UpdateLabels(gridSize.y, yLabels, yLabelParent, yLabelPrefab, "YLabel_", maxValue.y, true);
    }

    public void UpdateXLabels()
    {
        if (gridRect == null || xLabelPrefab == null || xLabelParent == null)
            return;

        UpdateLabels(gridSize.x, xLabels, xLabelParent, xLabelPrefab, "XLabel_", maxValue.x, false);
    }

    public void UpdateLabels(int gridSize, List<TMP_Text> labels, Transform parent, GameObject prefab, String LabelName, float maxValue, bool isVerticalText)
    {
        int targetCount = gridSize + 1;

        // STEP 1 — Rebuild list from actual children (this prevents desync)
        labels.Clear();
        for (int i = 0; i < parent.childCount; i++)
        {
            TMP_Text txt = parent.GetChild(i).GetComponent<TMP_Text>();
            if (txt != null) labels.Add(txt);
        }

        // STEP 2 — SPAWN missing labels
        while (labels.Count < targetCount)
        {
#if UNITY_EDITOR
            GameObject go = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
#else
            GameObject go = Instantiate(labelPrefab.gameObject, parent);
#endif
            go.name = LabelName + labels.Count;
            labels.Add(go.GetComponent<TMP_Text>());
        }

        // STEP 3 — DELETE extra labels
        while (labels.Count > targetCount)
        {
            TMP_Text last = labels[labels.Count - 1];

#if UNITY_EDITOR
        var obj = last.gameObject;
        EditorApplication.delayCall += () =>
        {
            if (obj != null)
                Undo.DestroyObjectImmediate(obj);
        };
#else
            Destroy(last.gameObject);
#endif

            labels.RemoveAt(labels.Count - 1);
        }

        // STEP 4 — Position + text
        float step;
        if(isVerticalText)
            step = gridRect.rect.height / gridSize;
        else
            step = gridRect.rect.width / gridSize;

        for (int i = 0; i < labels.Count; i++)
        {
            TMP_Text label = labels[i];
            float value = i * (maxValue / gridSize);

            label.text = value.ToString("0");

            RectTransform rt = label.rectTransform;

            if(isVerticalText)
                rt.anchoredPosition = new Vector2(0, i * step - gridRect.rect.height / 2);
            else
                rt.anchoredPosition = new Vector2(i * step - gridRect.rect.width / 2f, 0);
        }
    }
}
