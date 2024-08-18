using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance = null;

    [Space, Header("Collecting")]
    [SerializeField] private List<CollectableUI> collectableUI = null;
    [SerializeField] private RectTransform collectableGroup = null;

    [Space, Header("Eating")]
    [SerializeField] private List<Sprite> eatingSprites = null;
    [SerializeField] private Image eatingFadedImage = null;
    [SerializeField] private Image eatingFillImage = null;
    [SerializeField] private GameObject eatingGroup = null;
    [SerializeField] private RectTransform baseEatingGroup = null;
    [SerializeField] private AnimationCurve eatingAnimationCurve = null;

    [Space, Header("Animation")]
    [SerializeField] private Animator eatingUIAnimator = null;

    private Coroutine revealCollectableCoroutine = null;
    private Coroutine revealEatingCoroutine = null;
    private Coroutine eatingAnimationTimeout = null;
    private float eatingCount = 0f;
    private float maxEatingCount = 3f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetEatingFood(SceneController.Instance.CollectableCount);
        StartTimeout();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void RevealUI()
    {
        gameObject.SetActive(true);
        if (revealCollectableCoroutine != null) StopCoroutine(revealCollectableCoroutine);
        revealCollectableCoroutine = StartCoroutine(RevealCollectableCoroutine());
        if (revealEatingCoroutine != null) StopCoroutine(revealEatingCoroutine);
        revealEatingCoroutine = StartCoroutine(RevealEatingCoroutine());
    }

    public void HideUI()
    {
        if (eatingAnimationTimeout != null) StopCoroutine(eatingAnimationTimeout);
        gameObject.SetActive(false);
    }

    [ContextMenu("Test Eating")]
    public void TestEating()
    {
        EatFood(SceneController.EatingFoods.Crumb);
    }

    public void EatFood(SceneController.EatingFoods foodType)
    {
        eatingCount++;
        if (foodType == SceneController.Instance.ActiveEatableFood)
            eatingFillImage.fillAmount = eatingCount / maxEatingCount;

        StartTimeout();
        EatingUIAnimation();

        if (eatingCount >= maxEatingCount)
        {
            int collectableCount = SceneController.Instance.CollectableCount;
            StartCoroutine(WobbleCoroutine(eatingGroup.transform, () => 
            {
                collectableUI[collectableCount].UnlockCollectable();
                SceneController.Instance.CollectableCount++;
                SetEatingFood(SceneController.Instance.CollectableCount);
            }));
        }
        else
        {
            StartCoroutine(WobbleCoroutine(eatingGroup.transform));
        }
    }

    private void SetEatingFood(int collectableCount)
    {
        eatingCount = 0f;
        eatingFillImage.fillAmount = eatingCount / maxEatingCount;
        eatingFadedImage.sprite = eatingSprites[collectableCount];
        eatingFillImage.sprite = eatingSprites[collectableCount];
    }

    private void StartTimeout()
    {
        if (eatingAnimationTimeout != null) StopCoroutine(eatingAnimationTimeout);
        eatingAnimationTimeout = StartCoroutine(EatingUITimeoutCoroutine());
    }

    private WaitForSeconds timeoutDelay = new WaitForSeconds(6f);
    private IEnumerator EatingUITimeoutCoroutine()
    {
        yield return timeoutDelay;
        eatingUIAnimator.SetTrigger("NomNom");

        EatingUIAnimation();
    }

    private void EatingUIAnimation()
    {
        eatingUIAnimator.ResetTrigger("NomNom");
        eatingUIAnimator.SetTrigger("NomNom");
    }

    private IEnumerator WobbleCoroutine(Transform wobbleTransform, System.Action onComplete = null)
    {
        float time = 0f;
        const float duration = 1f;
        const float multiplier = -0.2f;
        Vector3 baseScale = Vector3.one;

        while (time <= duration)
        {
            float T = time / duration;
            wobbleTransform.localScale = baseScale + (eatingAnimationCurve.Evaluate(T) * multiplier * Vector3.one);

            time += Time.deltaTime;
            yield return null;
        }

        wobbleTransform.localScale = baseScale;
        onComplete?.Invoke();
    }

    private IEnumerator RevealCollectableCoroutine()
    {
        float time = 0f;
        const float duration = 0.6f;
        Vector2 collectableStartPosition = new Vector2(0f, 235f);
        Vector2 collectableTargetPosition = new Vector2(0f, -8f);
        collectableGroup.anchoredPosition = collectableStartPosition;

        yield return new WaitForSeconds(0.2f);

        while (time <= duration)
        {
            float T = time / duration;
            collectableGroup.anchoredPosition = Vector2.Lerp(collectableStartPosition, collectableTargetPosition, T);

            time += Time.deltaTime;
            yield return null;
        }

        collectableGroup.anchoredPosition = collectableTargetPosition;
    }

    private IEnumerator RevealEatingCoroutine()
    {
        float time = 0f;
        const float duration = 0.6f;
        Vector2 eatingStartPosition = new Vector2(500f, 0f);
        Vector2 eatingTargetPosition = Vector2.zero;
        baseEatingGroup.anchoredPosition = eatingStartPosition;

        while (time <= duration)
        {
            float T = time / duration;
            baseEatingGroup.anchoredPosition = Vector2.Lerp(eatingStartPosition, eatingTargetPosition, T);

            time += Time.deltaTime;
            yield return null;
        }

        baseEatingGroup.anchoredPosition = eatingTargetPosition;
    }
}
