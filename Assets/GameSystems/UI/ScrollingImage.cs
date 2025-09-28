using UnityEngine;
using UnityEngine.UI;

public class ScrollingImage : MonoBehaviour
{
    public RawImage rawImage;
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;


    private void Start()
    {
        if(rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
    }
    private void Update()
    {
        if (rawImage != null)
        {
            Rect rect = rawImage.uvRect;
            rect.x += scrollSpeedX * Time.deltaTime;
            rect.y += scrollSpeedY * Time.deltaTime;
            rawImage.uvRect = rect;
        }
    }
}
