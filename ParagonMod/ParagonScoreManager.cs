using ParagonMod.Patch;
using UnityEngine;

namespace ParagonMod;

public class ParagonScoreManager
{
    private readonly ParagonState _state;

    public ParagonScoreManager(ParagonState state)
    {
        _state = state;
        ExtendedRunStatsManager.OnApplyScoreModifier += ApplyScoreBonus;
    }

    private int ApplyScoreBonus(string name, int score)
    {
        if (_state.CurrentRunType != ParagonState.RunType.PARAGON)
            return score;
        var newScore = score + (int)Mathf.Floor(score * _state.ParagonLevel / 20f);
        Plugin.DefaultLogger.LogDebug($"Applying bonus to {name}: {score} -> {newScore}");
        return newScore;
    }
}
