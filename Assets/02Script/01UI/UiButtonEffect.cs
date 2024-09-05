using UnityEngine;

public class UiButtonEffect : MonoBehaviour
{
    public Vector2 initalSize;
    private RectTransform buttonRectTransform;
    public RectTransform ButtonRectTransform
    {
        get => buttonRectTransform;
        set
        {
            buttonRectTransform = value;
            if (buttonRectTransform != null)
            {
                isButtonSet = true;
                transform.SetParent(buttonRectTransform);
                transform.localPosition = Vector3.zero;
                ResizeParticleSystem();
            }
            else
            {
                isButtonSet = false;
                gameObject.SetActive(false);
            }
        }
    }
    private bool isButtonSet = false;

    private Vector2 previousSize;
    public Vector2 intialScale;
    

    private void Update()
    {
        if (!isButtonSet)
            return;

        if (GetWorldSize(ButtonRectTransform) != previousSize)
        {
            ResizeParticleSystem();
        }
    }

    private void ResizeParticleSystem()
    {
        Vector2 size = GetWorldSize(ButtonRectTransform);

        transform.localScale = new Vector3(intialScale.x * size.x / initalSize.x, intialScale.y * size.y / initalSize.y, 1f);
        previousSize = size;
    }

    private Vector2 GetWorldSize(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float width = Vector3.Distance(corners[0], corners[3]);
        float height = Vector3.Distance(corners[0], corners[1]);

        return new Vector2(width, height);
    }
}
