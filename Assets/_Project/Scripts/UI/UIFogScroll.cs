using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIFogScroll : MonoBehaviour

{
    public Vector2 speed = new Vector2(0.02f, 0f);

    private RawImage rawImage;
    private Vector2 offset;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        offset += speed * Time.unscaledDeltaTime;
        rawImage.uvRect = new Rect(offset, Vector2.one);
    }
}
