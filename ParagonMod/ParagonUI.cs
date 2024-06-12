using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace ParagonMod;

[RegisterTypeInIl2Cpp]
public class ParagonUI : MonoBehaviour
{
    private static readonly Options.OptionsSettings DifficultyOptionsSettings = (Options.OptionsSettings)"DifficultyChooser.Difficulty".GetHashCode();

    private readonly Lazy<Sprite> _paragonSprite = new(() => Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == "P-Key-Filled@8x"));
    private readonly Lazy<Sprite> _endlessSprite = new(() => Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == "E-Key-Filled@8x"));

    private ParagonState _state;
    private ResourceCountText _paragonLevelText;
    private ResourceCountText _endlessLevelText;

    public ParagonUI(IntPtr ptr) : base(ptr)
    {
    }

    public ParagonUI() : base(ClassInjector.DerivedConstructorPointer<ParagonUI>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }

    public void Inject()
    {
        _state = Paragon._state;
        _state.OnUnlockStateChanged += SetParagonUnlockState;
        _state.OnRunTypeChanged += RunTypeChanged;
        _state.OnParagonLevelChanged += SetParagonLevel;
        _state.OnEndlessLevelChanged += SetEndlessLevel;
        RefreshFromState();
    }

    private void Awake()
    {
        GameObject resourceCountContainer = GameObject.Find("ResourceCountContainer");
        if (resourceCountContainer != null)
        {
            Transform shardsCount = resourceCountContainer.transform.Find("ShardsCount");
            if (shardsCount != null)
            {
                Transform paragonLevel = Instantiate(shardsCount, resourceCountContainer.transform);
                paragonLevel.name = "ParagonLevel";
                paragonLevel.localPosition = shardsCount.localPosition - new Vector3(0, 50f, 0f);
                paragonLevel.rotation = shardsCount.rotation;
                paragonLevel.localScale = shardsCount.localScale;
                _paragonLevelText = paragonLevel.GetComponent<ResourceCountText>();
                _paragonLevelText.gameObject.SetActive(false);
                _paragonLevelText.logoImage.sprite = _paragonSprite.Value;
                _paragonLevelText.lowColor = new Color(1f, 1f, 1f, 0.02f);
                _paragonLevelText.defaultColor = new Color(1f, 0.7902f, 0.4906f);

                Transform endlessLevel = Instantiate(shardsCount, resourceCountContainer.transform);
                endlessLevel.name = "EndlessLevel";
                endlessLevel.localPosition = shardsCount.localPosition - new Vector3(0, 120f, 0f);
                endlessLevel.rotation = shardsCount.rotation;
                endlessLevel.localScale = shardsCount.localScale;
                _endlessLevelText = endlessLevel.GetComponent<ResourceCountText>();
                _endlessLevelText.gameObject.SetActive(false);
                _endlessLevelText.logoImage.sprite = _endlessSprite.Value;
                _endlessLevelText.lowColor = new Color(1f, 1f, 1f, 0.02f);
                _endlessLevelText.defaultColor = new Color(1f, 0.7902f, 0.4906f);

                RefreshFromState();
            }
        }
    }

    private void OnDestroy()
    {
        if (_state != null)
        {
            _state.OnUnlockStateChanged -= SetParagonUnlockState;
            _state.OnRunTypeChanged -= RunTypeChanged;
            _state.OnParagonLevelChanged -= SetParagonLevel;
            _state.OnEndlessLevelChanged -= SetEndlessLevel;
        }
    }

    private void RefreshFromState()
    {
        if (_state != null && _paragonLevelText != null && _endlessLevelText != null)
        {
            try
            {
                SetParagonUnlockState(_state.Unlocked);
                SetParagonLevel(_state.ParagonLevel);
                SetEndlessLevel(_state.EndlessLevel);
                RunTypeChanged(_state.CurrentRunType);
            }
            catch (Exception e)
            {
                Mod.DefaultLogger.Warning($"Could not update UI:\n\t{e}");
            }
        }
    }

    private void SetParagonUnlockState(bool unlocked)
    {
        _paragonLevelText?.gameObject.SetActive(unlocked);
        _endlessLevelText?.gameObject.SetActive(unlocked);
    }

    private void RunTypeChanged(ParagonState.RunType runType)
    {
        UpdateColors();
    }

    private void SetParagonLevel(int level)
    {
        _paragonLevelText.UpdateCount(level);
        UpdateColors();
    }

    private void SetEndlessLevel(int level)
    {
        _endlessLevelText.UpdateCount(level);
        UpdateColors();
    }

    private void UpdateColors()
    {
        _paragonLevelText.SetColor(_state.CurrentRunType == ParagonState.RunType.PARAGON ? 1 : 0);
        _endlessLevelText.SetColor(_state.CurrentRunType == ParagonState.RunType.ENDLESS ? 1 : 0);
    }

    public static void InjectGeneralOptions(ParagonState state, Options options)
    {
        Mod.DefaultLogger.Msg("Injecting difficulty option in menu...");
        try
        {
            GameObject sliderDifficultyGameObject = Instantiate(options.sliderLanguage.gameObject, options.sliderLanguage.transform.parent);
            sliderDifficultyGameObject.name = "Difficulty";
            sliderDifficultyGameObject.transform.localPosition = new Vector3(0, 50f, 0f);
            sliderDifficultyGameObject.transform.rotation = options.sliderLanguage.transform.rotation;
            sliderDifficultyGameObject.transform.localScale = options.sliderLanguage.transform.localScale;
            sliderDifficultyGameObject.transform.SetAsFirstSibling();

            Transform sliderTitleDifficulty = sliderDifficultyGameObject.transform.Find("Slider Title");
            Destroy(sliderTitleDifficulty.GetComponent<LocalizeStringEvent>()); // Disable translated string
            var titleDifficulty = sliderTitleDifficulty.GetComponentInChildren<TextMeshProUGUI>();
            titleDifficulty.SetText("Difficulty");

            var settingsSlider = sliderDifficultyGameObject.GetComponent<OptionsSettingsSlider>();
            settingsSlider.setting = DifficultyOptionsSettings;
            settingsSlider.settingsList = new Il2CppSystem.Collections.Generic.List<string>();
            foreach (string difficulty in Enum.GetNames(typeof(ParagonDifficulty)))
                settingsSlider.settingsList.Add(difficulty);
            settingsSlider.Slider.maxValue = settingsSlider.settingsList.Count - 1;
            settingsSlider.SetSliderValue((float)state.CurrentDifficulty);
            settingsSlider.SliderUpdated();
            settingsSlider.Slider.onValueChanged.AddListener(new Action<float>(value =>
            {
                state.CurrentDifficulty = (ParagonDifficulty)Mathf.RoundToInt(value);
                Mod.DefaultLogger.Msg($"Difficulty set to {state.CurrentDifficulty}");
            }));

            var selectableResolution = options.sliderResolution.GetComponent<Selectable>();
            var selectableDifficulty = options.sliderLanguage.GetComponent<Selectable>();
            Navigation navigationResolution = selectableResolution.navigation;
            selectableDifficulty.navigation = new Navigation
            {
                mode = navigationResolution.mode,
                selectOnUp = navigationResolution.selectOnUp,
                selectOnDown = selectableResolution
            };
            selectableResolution.navigation = new Navigation
            {
                mode = navigationResolution.mode,
                selectOnUp = selectableDifficulty,
                selectOnDown = navigationResolution.selectOnDown
            };
        }
        catch (Exception ex)
        {
            Mod.DefaultLogger.Error("An unexpected error has occurred", ex);
        }
    }
}
