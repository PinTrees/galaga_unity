using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interrupt : MonoBehaviour
{
    public float startHeight = 1.0f;
    public float endHeight = 5.9f;
    public float duration = 1.0f;
    public float deleteDelay = 4.0f;

    private float startTime;
    private float initialYScale;

    public bool IsInterrupted = false;
    Enemy owner;

    public void Enter(Enemy owner)
    {
        IsInterrupted = false;
        this.owner = owner;
        startTime = Time.time;
        initialYScale = 1;

        Vector3 newScale = transform.localScale;
        newScale.y = initialYScale;
        transform.localScale = newScale;

        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;

        if (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float newHeight = Mathf.Lerp(startHeight, endHeight, t);

            Vector3 newScale = transform.localScale;
            newScale.y = newHeight;
            transform.localScale = newScale;
        }

        if(elapsedTime > deleteDelay && !IsInterrupted)
        {
            Exit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            IsInterrupted = true;
            var player = other.GetComponent<Player>();
            player.OnInterruct(owner.transform);
            owner.OnInterrup();

            StartCoroutine(DelayDesroy(2f));
        }
    }

    IEnumerator DelayDesroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Exit();
    }
}
