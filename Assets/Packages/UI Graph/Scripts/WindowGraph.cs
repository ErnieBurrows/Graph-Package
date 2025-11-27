using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private GameObject HorizontalLine, xAxisText;
    [SerializeField] private float maxX, maxY;
    [SerializeField] private int ySteps, xSteps;

    private Vector3[] localCorners;
    private RectTransform graphContainer;

    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

        float yStep = maxY / ySteps;

        localCorners = new Vector3[4];

        graphContainer.GetLocalCorners(localCorners);

        SetUpYSide();

        SetUpXSide();

        for (int i = 1; i < 12; i++)
        {
            PlotSingleDot(i, i * 25 );
        }
    }



    private void SetUpYSide()
    {
        float step = Mathf.Abs(localCorners[0].y) + Mathf.Abs(localCorners[1].y);

        float z = step / ySteps;
        float num = maxY / ySteps;

        for (int i = 0; i < ySteps; i++)
        {
            if (i == 0) continue;

            float yPos = localCorners[0].y + (z * i);
            GameObject go = Instantiate(HorizontalLine);
            go.transform.SetParent(graphContainer, false);
            go.transform.localPosition = new Vector3(localCorners[0].x, yPos);
            go.GetComponentInChildren<TextMeshProUGUI>().text = (num * i).ToString();
        }

    }

    private void SetUpXSide()
    {
        float step = Mathf.Abs(localCorners[0].x) + Mathf.Abs(localCorners[3].x);

        float z = step / xSteps;
        float num = maxX / xSteps;

        for (int i = 0; i < xSteps; i++)
        {
            if (i == 0) continue;

            float xPos = localCorners[0].x + (z * i);
            GameObject go = Instantiate(xAxisText);
            go.transform.SetParent(graphContainer, false);
            go.transform.localPosition = new Vector3(xPos, localCorners[3].y);
            go.GetComponent<TextMeshProUGUI>().text = (num * i).ToString();
        }
    }

    private void PlotSingleDot(float xVal, float yVal)
    {
        float lengthX = Mathf.Abs(localCorners[0].x) + Mathf.Abs(localCorners[3].x);
        float lengthY = Mathf.Abs(localCorners[0].y) + Mathf.Abs(localCorners[1].y);

        // gives percentage as decimal
        float xPos = (xVal / maxX) * lengthX;
        float yPos = (yVal / maxY) * lengthY;

        xPos += localCorners[0].x;
        yPos += localCorners[0].y;

        GameObject go = new GameObject("Circle", typeof(Image));
        go.transform.SetParent(graphContainer, false);
        go.GetComponent<Image>().sprite = circleSprite;
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
        go.transform.localPosition = new Vector2(xPos,yPos);
    }
    
    
    // todo: should I maybe actually put x and y into Key value pairs or something?


}

    // private void DisplayLocalCorners()
    // {
    //     Vector3[] v = new Vector3[4];

    //     graphContainer.GetLocalCorners(v);

    //     for (int i = 0; i < 4; i++)
    //     {
    //         Debug.Log(v[i]);
    //         GameObject go = new GameObject("Circle", typeof(Image));
    //         go.transform.SetParent(graphContainer, false);
    //         go.GetComponent<Image>().sprite = circleSprite;
    //         go.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
    //         go.transform.localPosition = v[i];
    //     }
    // }