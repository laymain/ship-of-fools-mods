using System.Collections.Generic;
using UnityEngine;

namespace ParagonMod;

public class ParagonEnemyManager
{
    private readonly ParagonState _state;

    public ParagonEnemyManager(ParagonState state)
    {
        _state = state;
    }

    public void OnEnemySpawn(Enemy enemy)
    {
        if ((_state.CurrentRunType == ParagonState.RunType.PARAGON || _state.CurrentDifficulty != ParagonDifficulty.DEFAULT) && !enemy.hideFlags.HasFlag(HideFlags.NotEditable))
        {
            enemy.hideFlags |= HideFlags.NotEditable;
            Plugin.DefaultLogger.LogDebug($"Applying bonuses to enemy [{enemy.name}][&{enemy.GetInstanceID()}][@{enemy.enemyAi?.GetType()}][^{enemy.enemyAi?.Movement?.GetType()}]");
            ApplyHealthPointsBonus(enemy);
            if (enemy.enemyAi != null) {
                ApplyMovementBonus(enemy);
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
                    case PuffyAi:
                        break;
                    case HatcherAi ai:
                        ai.initialEggLayFrequency = Mathf.Max(6f, ai.initialEggLayFrequency - _state.DifficultyModifier / 10f);
                        ai.minimumEggLayFrequency = Mathf.Max(2f, ai.minimumEggLayFrequency - _state.DifficultyModifier / 10f);
                        break;
                    case OwletAi ai:
                        ai.chargeBoatMovement.defaultSpeed *= 1f + _state.DifficultyModifier / 15f;
                        ApplyRotationZonesBonus(ai.rotationZoneMovement.rotationZones);
                        break;
                    case OwlAi ai:
                        ai.attackDelay = ApplyMinAttackDelayBonus(ai.attackDelay);
                        ApplyRotationZonesBonus(ai.rotationZones);
                        break;
                    case PeckerAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.anticipationTimer = new Timer(ai.anticipationTime);
                        break;
                    case FlyerEnemyAi ai:
                        ai.anticipationDelay = ApplyAnticipationDurationBonus(ai.anticipationDelay);
                        ai.minimumAttackDelay = ApplyMinAttackDelayBonus(ai.minimumAttackDelay);
                        ai.maximumAttackDelay = ApplyMaxAttackDelayBonus( ai.maximumAttackDelay);
                        break;
                    case ScannerAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.minimumAttackDelayTimer = new Timer(Mathf.Max(1f, ai.minimumAttackDelayTimer.Duration - _state.DifficultyModifier / 15f));
                        break;
                    case TentacleAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.anticipationTimer = new Timer(ai.anticipationTime);
                        break;
                    case TentaclesHeadAi ai:
                        ai.maxSlurgEggAmount += Mathf.RoundToInt(_state.DifficultyModifier / 50f);
                        break;
                    case SerpentExtremityAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.anticipationTimer = new Timer(ai.anticipationTime);
                        ai.attackTime = ApplyAttackTimeBonus(ai.attackTime);
                        ai.attackTimer = new Timer(ai.attackTime);
                        break;
                    case SerpentLoopAi:
                        break;
                    case ClawsHeadAi ai:
                        ai.anticipationDuration = ApplyAnticipationDurationBonus(ai.anticipationDuration);
                        ai.minimumAttackDelay = ApplyMinAttackDelayBonus(ai.minimumAttackDelay, 2f, 10f);
                        ai.maximumAttackDelay = ApplyMaxAttackDelayBonus(ai.maximumAttackDelay, 4f, 10f);
                        break;
                    default:
                        Plugin.DefaultLogger.LogDebug($"Unknown enemy ai: {enemy.enemyAi}");
                        break;
                }
            }
        }
    }

    private float ApplyMinAttackDelayBonus(float current, float lowerBound = .5f, float ratio = 15f)
    {
        var newValue = Mathf.Max(Mathf.Min(current, lowerBound), current - _state.DifficultyModifier / ratio);
        Plugin.DefaultLogger.LogDebug($"\tMinAttackDelay: {current} -> {newValue}");
        return newValue;
    }

    private float ApplyMaxAttackDelayBonus(float current, float lowerBound = .8f, float ratio = 15f)
    {
        var newValue = Mathf.Max(Mathf.Min(current, lowerBound), current - _state.DifficultyModifier / ratio);
        Plugin.DefaultLogger.LogDebug($"\tMaxAttackDelay: {current} -> {newValue}");
        return newValue;
    }

    private float ApplyAttackProbabilityBonus(float current)
    {
        var newValue = Mathf.Min(.8f, current + _state.DifficultyModifier / 500f);
        Plugin.DefaultLogger.LogDebug($"\tAttackProbability: {current} -> {newValue}");
        return newValue;
    }

    private float ApplyAttackTimeBonus(float current)
    {
        var newValue = current + _state.DifficultyModifier / 10f;
        Plugin.DefaultLogger.LogDebug($"\tAttackTime: {current} -> {newValue}");
        return newValue;
    }

    private void ApplyMovementBonus(Enemy enemy, float ratio = 15f)
    {
        var movement = enemy.enemyAi.Movement;
        if (movement != null)
        {
            var newValue = movement.defaultSpeed * (1f + _state.DifficultyModifier / ratio);
            Plugin.DefaultLogger.LogDebug($"\tMovementSpeed: {movement.defaultSpeed} -> {newValue}");
            movement.defaultSpeed = newValue;
            movement.speed = newValue;
            if (movement is RotationZonesMovement rotationZonesMovement && enemy.enemyAi is not TentacleAi) // Tentacle boss dies instantly when modifying rotation zones ¯\_(ツ)_/¯
                ApplyRotationZonesBonus(rotationZonesMovement.rotationZones, ratio);
        }
    }

    private void ApplyRotationZonesBonus(IEnumerable<RotationZone> rotationZones, float ratio = 50f)
    {
        foreach (RotationZone zone in rotationZones)
        {
            if (!zone.hideFlags.HasFlag(HideFlags.NotEditable))
            {
                var newValue = zone.angularVelocity * (1 + _state.DifficultyModifier / ratio);
                if (newValue < -100f)
                    newValue = -100f;
                else if (newValue > 100f)
                    newValue = 100f;

                Plugin.DefaultLogger.LogDebug($"\t{zone.name}: {zone.angularVelocity} -> {newValue}");
                zone.angularVelocity = newValue;
                zone.hideFlags |= HideFlags.NotEditable;
            }
        }
    }

    private void ApplyHealthPointsBonus(Enemy enemy, float ratio = 30f)
    {
        if (enemy.HealthPoints != null)
        {
            switch (enemy.name)
            {
                case "Tentacle(Clone)": // Tentacles are cloned from TentaclesHead...
                case "SerpentTail": // SerpentTails are cloned from SerpentHead...
                case "SerpentLoop(Clone)": // SerpentLoop are cloned from SerpentTail...
                case "Crusher": // Crusher is a clone of Ranger...
                    return; // ...which already has health bonuses applied
            }
            var newValue = enemy.HealthPoints.Maximum * (1f + _state.DifficultyModifier / ratio);
            Plugin.DefaultLogger.LogDebug($"\tHealth: {enemy.HealthPoints.Maximum} -> {newValue}");
            enemy.HealthPoints.Maximum = newValue;
            enemy.HealthPoints.Base = newValue;
        }
    }

    private float ApplyAnticipationDurationBonus(float current, float lowerBound = .5f, float ratio = 15f)
    {
        var newValue = Mathf.Max(Mathf.Min(current, lowerBound), current - _state.DifficultyModifier / ratio);
        Plugin.DefaultLogger.LogDebug($"\tAnticipationDuration: {current} -> {newValue}");
        return newValue;
    }

}
