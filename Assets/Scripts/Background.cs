using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // ��ũ�� �ӵ� ����
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
