using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UILineRenderer : MaskableGraphic
{
    public UIGridRenderer gridRenderer;
    public List<Vector2> points;
    public float thickness = 10.0f;
    private float width, height;
    private float unitWidth, unitHeight;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        UIGridRenderer.OnGridValuesChanged += HandleGridChanged;
        GridSizer.OnGridSizeChanged += HandleGridChanged;
        SetVerticesDirty();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIGridRenderer.OnGridValuesChanged -= HandleGridChanged;
        GridSizer.OnGridSizeChanged -= HandleGridChanged;
    }

    private void HandleGridChanged()
    {
        SetVerticesDirty();
    }

   /// <summary>
   /// Gets the grid size and the maxValue from the gridRenderer, and normalizes the dot and line locations between 0 and gridSize.
   /// </summary>
    public void AddPoint(Vector2 pointValue)
    {
        Vector2 gridLocation = ConvertPointValueToGridLocation(pointValue);

        points.Add(gridLocation);

        SetVerticesDirty();
    }

    public void RemovePoint(Vector2 pointValue)
    {
        Vector2 gridLocation = ConvertPointValueToGridLocation(pointValue);

        if(!points.Remove(gridLocation))
        {
            Debug.Log("No point to be removed @ " + pointValue);
            return;
        }

        SetVerticesDirty();

    }

    public void RemoveAllPoints()
    {
        points.Clear();

        SetVerticesDirty();
    }
    
    protected override void OnPopulateMesh(VertexHelper vh) {

        vh.Clear();

        width = gridRenderer.width;
        height = gridRenderer.height;

        unitWidth = width / gridRenderer.gridSize.x;
        unitHeight = height / gridRenderer.gridSize.y;

        if (points.Count < 2) return;
        
        float angle = 0;
        for (int i = 0; i < points.Count - 1; i++) {

            Vector2 point = points[i];
            Vector2 point2 = points[i+1];

            if (i < points.Count - 1) {
                angle = GetAngle(points[i], points[i + 1]) + 90f;
            }

            DrawVerticesForPoint(point, point2, angle, vh);
        }

        for (int i = 0; i < points.Count - 1; i++) {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }
    }
    public float GetAngle(Vector2 me, Vector2 target)
    {
        Vector2Int aspectRatio = AspectRatioUtility.GetAspectRatio(new Vector2Int((int)gridRenderer.width, (int)gridRenderer.height));

        return (float)(Mathf.Atan2(aspectRatio.x * (target.y - me.y), aspectRatio.y * (target.x - me.x)) * (180 / Mathf.PI));
    }
    
    void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh) 
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);
    }

    public static double GetAspectRatioDecimal(int width, int height)
    {
        if (height == 0)
        {
            return 0.0; // Avoid division by zero
        }
        return (double)width / height;
    }

        private Vector2 ConvertPointValueToGridLocation(Vector2 pointValue)
    {
        Vector2 maxValue = gridRenderer.maxValues;
        Vector2 percentageLocation = pointValue / maxValue;
        Vector2 gridSizeLocation = percentageLocation * gridRenderer.gridSize;
        return gridSizeLocation;
    }
}
