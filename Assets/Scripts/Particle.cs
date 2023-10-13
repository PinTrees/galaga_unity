using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Vector3 startScale = Vector3.one;
    public float scaleFactor = 2f; // ũ�� ���� ����
    public float duration = 1f;    // ũ�� ���� �Ⱓ

    private float startTime;

    public void Enter()
    {
        startTime = Time.time;
        gameObject.SetActive(true);
    }

    public void Dispose()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < duration)
        {
            // ũ�⸦ �������� ����
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, startScale * scaleFactor, t);
        }
        else
        {
            Dispose();
        }
    }
}
