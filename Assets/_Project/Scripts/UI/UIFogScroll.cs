using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIFogScroll : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.02f, 0.01f);
    public bool unscaledTime = true;

    private RawImage _img;
    private Rect _uv;

    void Awake()
    {
        _img = GetComponent<RawImage>();
        _uv = _img.uvRect;
    }

    void Update()
    {
        float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _uv.position += speed * dt;
        _img.uvRect = _uv;
    }
}