using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableUI : MonoBehaviour
{
    [SerializeField] private GameObject questionMark = null;
    [SerializeField] private Image highlight = null;
    [SerializeField] private Image collectable = null;
    [SerializeField] private GameObject collectedTick = null;
    [SerializeField] private Transform group = null;

    [SerializeField] private Color blankColor = Color.white;
    [SerializeField] private Color highlightColor = Color.white;

    private void Start()
    {
        questionMark.gameObject.SetActive(true);
        highlight.color = blankColor;
        collectable.gameObject.SetActive(false);
        collectedTick.gameObject.SetActive(false);
    }

    public void UnlockCollectable()
    {
        questionMark.gameObject.SetActive(false);
        highlight.color = highlightColor;
        collectable.gameObject.SetActive(true);
        StartCoroutine(RevealCoroutine());
    }

    public void Collected()
    {
        collectedTick.gameObject.SetActive(true);
    }

    private IEnumerator RevealCoroutine()
    {
        float time = 0f;
        const float duration = 0.6f;
        const float multiplier = 0.2f;
        Vector3 baseScale = Vector3.one;

        while (time <= duration)
        {
            float T = time / duration;
            group.localScale = baseScale + (Mathf.Sin(Mathf.PI * 1f * T) * multiplier * Vector3.one);

            time += Time.deltaTime;
            yield return null;
        }

        group.localScale = baseScale;
    }
}
