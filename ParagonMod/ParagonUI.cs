using System;
using System.Linq;
using UnityEngine;

namespace ParagonMod.UI;

public class ParagonUI : MonoBehaviour
{
    private readonly Lazy<Sprite> _paragonSprite = new(() => Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == "P-Key-Filled@8x"));
    private ParagonState _state;
    private ResourceCountText _levelText;

    public void Inject(ParagonState state)
    {
        _state = state;
        _state.OnUnlockStateChanged += SetParagonUnlockState;
        _state.OnEnableStateChanged += SetParagonEnableState;
        _state.OnLevelChanged += SetParagonLevel;
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
                Transform level = Instantiate(shardsCount, resourceCountContainer.transform);
                level.name = "ParagonLevel";
                level.localPosition = shardsCount.localPosition - new Vector3(0, 50f, 0f);
                level.rotation = shardsCount.rotation;
                level.localScale = shardsCount.localScale;
                _levelText = level.GetComponent<ResourceCountText>();
                _levelText.gameObject.SetActive(false);
                _levelText.logoImage.sprite = _paragonSprite.Value;
                _levelText.lowColor = new Color(1f, 1f, 1f, 0.02f);
                _levelText.defaultColor = new Color(1f, 0.7902f, 0.4906f);
                RefreshFromState();
            }
        }
    }

    private void OnDestroy()
    {
        if (_state != null)
        {
            _state.OnUnlockStateChanged -= SetParagonUnlockState;
            _state.OnEnableStateChanged -= SetParagonEnableState;
            _state.OnLevelChanged -= SetParagonLevel;
        }
    }

    private void RefreshFromState()
    {
        if (_state != null && _levelText != null)
        {
            SetParagonUnlockState(_state.Unlocked);
            SetParagonLevel(_state.Level);
            SetParagonEnableState(_state.Enabled);
        }
    }

    private void SetParagonUnlockState(bool unlocked)
    {
        _levelText?.gameObject.SetActive(unlocked);
    }

    private void SetParagonEnableState(bool isEnabled)
    {
        UpdateColor();
    }

    private void SetParagonLevel(int level)
    {
        _levelText.UpdateCount(level);
        UpdateColor();
    }

    private void UpdateColor()
    {
        _levelText.SetColor(_state.Enabled ? 1 : 0);
    }
}
