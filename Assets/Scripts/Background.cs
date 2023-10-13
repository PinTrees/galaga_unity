using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // 스크롤 속도 조절
    private Renderer backgroundRenderer;
    private Vector2 offset;

    void Start()
    {
        backgroundRenderer = GetComponent<Renderer>();
        offset = Vector2.zero;
    }

    void Update()
    {
        offset.y = Time.time * scrollSpeed;
        backgroundRenderer.material.mainTextureOffset = offset;
    }
}
