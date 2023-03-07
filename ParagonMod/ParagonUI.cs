using System;
using System.Linq;
using UnityEngine;

namespace ParagonMod;

public class ParagonUI : MonoBehaviour
{
    private readonly Lazy<Sprite> _paragonSprite = new(() => Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == "P-Key-Filled@8x"));
    private readonly Lazy<Sprite> _endlessSprite = new(() => Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == "E-Key-Filled@8x"));

    private ParagonState _state;
    private ResourceCountText _paragonLevelText;
    private ResourceCountText _endlessLevelText;

    public void Inject(ParagonState state)
    {
        _state = state;
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
                Plugin.DefaultLogger.LogWarning($"Could not update UI:\n\t{e}");
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
}
