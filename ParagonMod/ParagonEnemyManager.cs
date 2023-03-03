using System.Collections.Generic;
using UnityEngine;

namespace ParagonMod;

public class ParagonEnemyManager
{
    private readonly ISet<int> _enemyIds = new HashSet<int>(); // OnEnemySpawn may be called multiple times for same enemy, we don't want to apply bonus multiple times
    private readonly ParagonState _state;

    public ParagonEnemyManager(ParagonState state)
    {
        _state = state;
    }

    public void OnEnemySpawn(Enemy enemy)
    {
        if (_state.Enabled && !_enemyIds.Contains(enemy.GetInstanceID()))
        {
            _enemyIds.Add(enemy.GetInstanceID());
            if (enemy.name == "Tentacle(Clone)") return; // Tentacles are cloned from TentacleHead which already has bonuses applied
            Plugin.DefaultLogger.LogDebug($"Applying bonuses to enemy {enemy.name}@{enemy.GetInstanceID()}");
            if (enemy.HealthPoints != null)
                ApplyHealthPointsBonus(enemy.HealthPoints);
            if (enemy.enemyAi != null) {
                ApplyMovementBonus(enemy.enemyAi.Movement);
                switch (enemy.enemyAi)
                {
                    case CrabsterEnemyAi ai:
                        ai.attackDelay = ApplyMinAttackDelayBonus(ai.attackDelay);
                        break;
                    case CrawlerAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        break;
                    case SpiderAi ai:
                        ai.minAttackDelay = ApplyMinAttackDelayBonus(ai.minAttackDelay);
                        ai.maxAttackDelay = ApplyMaxAttackDelayBonus( ai.minAttackDelay);
                        break;
                    case BullyAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.minDelayBeforeAttack = ApplyMinAttackDelayBonus(ai.minDelayBeforeAttack);
                        ai.maxDelayBeforeAttack = ApplyMaxAttackDelayBonus(ai.maxDelayBeforeAttack);
                        ai.attackProbability = ApplyAttackProbabilityBonus(ai.attackProbability);
                        break;
                    case OwletAi ai:
                        ai.chargeBoatMovement.defaultSpeed *= 1f + _state.Level / 40f;
                        break;
                    case OwlAi ai:
                        ai.attackDelay = ApplyMinAttackDelayBonus(ai.attackDelay);
                        break;
                    case FlyerEnemyAi ai:
                        ai.anticipationDelay = ApplyAnticipationDurationBonus(ai.anticipationDelay);
                        ai.minimumAttackDelay = ApplyMinAttackDelayBonus(ai.minimumAttackDelay);
                        ai.maximumAttackDelay = ApplyMaxAttackDelayBonus( ai.maximumAttackDelay);
                        break;
                    case ScannerAi ai:
                        ai.anticipationTime  = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.minimumAttackDelayTimer = new Timer(Mathf.Max(1f, ai.minimumAttackDelayTimer.Duration - _state.Level / 35f));
                        break;
                    case TentaclesHeadAi ai:
                        ai.anticipationTime  = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.EggAmountMultiplier = Mathf.Max(10f, ai.EggAmountMultiplier + _state.Level / 10f);
                        break;
                    case ClawsHeadAi ai:
                        ai.anticipationDuration = ApplyAnticipationDurationBonus(ai.anticipationDuration);
                        ai.minimumAttackDelay = ApplyMinAttackDelayBonus(ai.minimumAttackDelay, 2f, 10f);
                        ai.maximumAttackDelay = ApplyMaxAttackDelayBonus(ai.maximumAttackDelay, 4f, 10f);
                        break;
                }
            }
        }
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        if (_state.Enabled)
            _enemyIds.Remove(enemy.GetInstanceID());
    }

    public void OnRunEnded(bool victory)
    {
        _enemyIds.Clear();
    }

    private float ApplyMinAttackDelayBonus(float delay, float lowerBound = .5f, float ratio = 35f)
    {
        var newValue = Mathf.Max(lowerBound, delay - _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tMinAttackDelay: {delay} -> {newValue}");
        return newValue;
    }

    private float ApplyMaxAttackDelayBonus(float delay, float lowerBound = .8f, float ratio = 35f)
    {
        var newValue = Mathf.Max(lowerBound, delay - _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tMaxAttackDelay: {delay} -> {newValue}");
        return newValue;
    }

    private float ApplyAttackProbabilityBonus(float current)
    {
        var newValue = Mathf.Min(.8f, current + _state.Level / 500f);
        Plugin.DefaultLogger.LogDebug($"\tAttackProbability: {current} -> {newValue}");
        return newValue;
    }

    private void ApplyMovementBonus(Movement movement, float ratio = 40f)
    {
        var newValue = movement.defaultSpeed * (1f + _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tMovementSpeed: {movement.defaultSpeed} -> {newValue}");
        movement.defaultSpeed = newValue;
    }

    private void ApplyHealthPointsBonus(HealthPoints healthPoints, float ratio = 50f)
    {
        var newValue = healthPoints.Maximum * (1f + _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tHealth: {healthPoints.Maximum} -> {newValue}");
        healthPoints.Maximum = newValue;
        healthPoints.Base = newValue;
    }

    private float ApplyAnticipationDurationBonus(float current, float lowerBound = .5f, float ratio = 35f)
    {
        var newValue = Mathf.Max(lowerBound, current - _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tAnticipationDuration: {current} -> {newValue}");
        return newValue;
    }

}
