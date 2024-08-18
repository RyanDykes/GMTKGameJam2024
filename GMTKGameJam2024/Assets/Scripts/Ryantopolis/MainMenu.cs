using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance = null;

    public bool IsPlaying { get; private set; } = false;

    [SerializeField] private Image background = null;
    [SerializeField] private RectTransform highlight = null;
    [SerializeField] private List<RectTransform> menuOptions = null;
    [SerializeField] private List<RectTransform> settingsOptions = null;

    private List<RectTransform> activeOptions = null;
    private bool buttonClicked = false;
    private int menuIndex = 0;

    private bool inSettings = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(Transform option in menuOptions)
        {
            option.gameObject.SetActive(false);
            option.localPosition = option.localPosition + (Vector3.right * 1000f);
        }

        foreach (Transform option in settingsOptions)
        {
            option.gameObject.SetActive(false);
        }

        LoadMenu(menuOptions);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        if (IsPlaying && (Input.GetKeyDown(KeyCode.Escape)))
        {
            LoadMenu(menuOptions);
            return;
        }

        if (IsPlaying)
            return;

        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical != 0 && !buttonClicked)
        {
            buttonClicked = true;
            NavigateMenu((int)vertical);
        }

        if (vertical == 0 && buttonClicked)
        {
            buttonClicked = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inSettings)
                SelectSettingsOption(menuIndex);
            else
                SelectMenuOption(menuIndex);
        }
    }

    private void NavigateMenu(int value)
    {
        menuIndex -= value;
        menuIndex = menuIndex < 0 ? activeOptions.Count - 1 : menuIndex;
        menuIndex %= activeOptions.Count;
        highlight.anchoredPosition = new Vector2(0f, activeOptions[menuIndex].anchoredPosition.y);
    }

    private void SelectMenuOption(int value)
    {
        switch (value)
        {
            case 0:
                //print("PLAY");
                Play();
                break;
            case 1:
                //print("SETTINGS");
                Settings();
                break;
            case 2:
                //print("EXIT");
                Exit();
                break;
        }
    }

    private void SelectSettingsOption(int value)
    {
        switch (value)
        {
            case 0:
                //print("YEAH RIGHT");
                break;
            case 1:
                //print("BACK");
                Back();
                break;
        }
    }

    private void Play()
    {
        IsPlaying = true;
        GameUI.Instance.RevealUI();

        for (int i = 0; i < menuOptions.Count; i++)
        {
            StartCoroutine(HideTextCoroutine(menuOptions[i]));
        }

        StartCoroutine(FadeOutBackgroundCoroutine());
        SetHightlightState(false);
    }

    private void Settings()
    {
        inSettings = true;

        StartCoroutine(HideTextCoroutine(menuOptions[0], () => LoadMenu(settingsOptions)));
        for (int i = 1; i < menuOptions.Count; i++)
        {
            StartCoroutine(HideTextCoroutine(menuOptions[i]));
        }
    }

    private void Back()
    {
        inSettings = false;
        StartCoroutine(HideTextCoroutine(settingsOptions[0], () => LoadMenu(menuOptions)));
        for (int i = 1; i < settingsOptions.Count; i++)
        {
            StartCoroutine(HideTextCoroutine(settingsOptions[i]));
        }
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void LoadMenu(List<RectTransform> newOptions)
    {
        IsPlaying = false;
        GameUI.Instance.HideUI();

        activeOptions = new List<RectTransform>(newOptions);

        for (int i = 0; i < activeOptions.Count; i++)
        {
            activeOptions[i].gameObject.SetActive(true);
            StartCoroutine(RevealTextCoroutine(activeOptions[i]));
        }

        if(!background.gameObject.activeInHierarchy)
            StartCoroutine(FadeInBackgroundCoroutine());

        SetHightlightState(true);
        menuIndex = 0;
        highlight.anchoredPosition = new Vector2(0f, activeOptions[menuIndex].anchoredPosition.y);
    }

    private void SetHightlightState(bool state)
    {
        highlight.gameObject.SetActive(state);
    }

    private IEnumerator RevealTextCoroutine(Transform option, System.Action onComplete = null)
    {
        float time = 0f;
        const float duration = 0.3f;
        Vector3 startPosition = option.localPosition;
        Vector3 targetPosition = new Vector3(-275f, option.localPosition.y, 0f);

        while (time <= duration)
        {
            float T = time / duration;
            option.localPosition = Vector3.Lerp(startPosition, targetPosition, T);

            time += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }

    private IEnumerator HideTextCoroutine(Transform option, System.Action onComplete = null)
    {
        float time = 0f;
        const float duration = 0.3f;
        Vector3 startPosition = option.localPosition;
        Vector3 targetPosition = option.localPosition + (Vector3.right * 1000f);

        while (time <= duration)
        {
            float T = time / duration;
            option.localPosition = Vector3.Lerp(startPosition, targetPosition, T);

            time += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }

    private IEnumerator FadeInBackgroundCoroutine()
    {
        float time = 0f;
        const float duration = 0.4f;
        const float baseAlpha = 0.3f;

        background.gameObject.SetActive(true);

        while (time <= duration)
        {
            float T = time / duration;
            Color alpha = background.color;
            alpha.a = T * baseAlpha;
            background.color = alpha;

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutBackgroundCoroutine()
    {
        float time = 0f;
        const float duration = 0.4f;
        const float baseAlpha = 0.3f;

        while(time <= duration)
        {
            float T = time / duration;
            Color alpha = background.color;
            alpha.a = (1f - T) * baseAlpha;
            background.color = alpha;

            time += Time.deltaTime;
            yield return null;
        }

        background.gameObject.SetActive(false);
    }
}
