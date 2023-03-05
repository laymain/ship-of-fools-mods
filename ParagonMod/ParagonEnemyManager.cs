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
        if (_state.Enabled && !enemy.hideFlags.HasFlag(HideFlags.NotEditable))
        {
            enemy.hideFlags |= HideFlags.NotEditable;
            Plugin.DefaultLogger.LogDebug($"Applying bonuses to enemy [{enemy.name}][&{enemy.GetInstanceID()}][@{enemy.enemyAi?.GetType()}][^{enemy.enemyAi?.Movement?.GetType()}]");
            ApplyHealthPointsBonus(enemy);
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
                    case PuffyAi ai:
                        break;
                    case HatcherAi ai:
                        ai.initialEggLayFrequency = Mathf.Max(6f, ai.initialEggLayFrequency - _state.Level / 10f);
                        ai.minimumEggLayFrequency = Mathf.Max(2f, ai.minimumEggLayFrequency - _state.Level / 10f);
                        break;
                    case OwletAi ai:
                        ai.chargeBoatMovement.defaultSpeed *= 1f + _state.Level / 15f;
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
                        ai.minimumAttackDelayTimer = new Timer(Mathf.Max(1f, ai.minimumAttackDelayTimer.Duration - _state.Level / 15f));
                        break;
                    case TentacleAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.anticipationTimer = new Timer(ai.anticipationTime);
                        break;
                    case TentaclesHeadAi ai:
                        ai.anticipationTime  = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        break;
                    case SerpentExtremityAi ai:
                        ai.anticipationTime = ApplyAnticipationDurationBonus(ai.anticipationTime);
                        ai.anticipationTimer = new Timer(ai.anticipationTime);
                        ai.attackTime = ApplyAttackTimeBonus(ai.attackTime);
                        ai.attackTimer = new Timer(ai.attackTime);
                        break;
                    case SerpentLoopAi ai:
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

    private float ApplyMinAttackDelayBonus(float delay, float lowerBound = .5f, float ratio = 15f)
    {
        var newValue = Mathf.Max(lowerBound, delay - _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tMinAttackDelay: {delay} -> {newValue}");
        return newValue;
    }

    private float ApplyMaxAttackDelayBonus(float delay, float lowerBound = .8f, float ratio = 15f)
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

    private float ApplyAttackTimeBonus(float current)
    {
        var newValue = current + _state.Level / 10f;
        Plugin.DefaultLogger.LogDebug($"\tAttackTime: {current} -> {newValue}");
        return newValue;
    }

    private void ApplyMovementBonus(Movement movement, float ratio = 15f)
    {
        if (movement != null)
        {
            var newValue = movement.defaultSpeed * (1f + _state.Level / ratio);
            Plugin.DefaultLogger.LogDebug($"\tMovementSpeed: {movement.defaultSpeed} -> {newValue}");
            movement.defaultSpeed = newValue;
            movement.speed = newValue;
            if (movement is RotationZonesMovement rotationZonesMovement)
                ApplyRotationZonesBonus(rotationZonesMovement.rotationZones, ratio);
        }
    }

    private void ApplyRotationZonesBonus(IEnumerable<RotationZone> rotationZones, float ratio = 15f)
    {
        foreach (RotationZone zone in rotationZones)
        {
            if (!zone.hideFlags.HasFlag(HideFlags.NotEditable))
            {
                var newValue = zone.angularVelocity * (1 + _state.Level / ratio);
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
                case "Tentacle(Clone)": // Tentacles are cloned from TentacleHead...
                case "SerpentTail": // SerpentTails are cloned from SerpentHead
                case "SerpentLoop(Clone)": // SerpentLoop are cloned from SerpentTail
                    return; // ...which already has health bonuses applied
            }
            var newValue = enemy.HealthPoints.Maximum * (1f + _state.Level / ratio);
            Plugin.DefaultLogger.LogDebug($"\tHealth: {enemy.HealthPoints.Maximum} -> {newValue}");
            enemy.HealthPoints.Maximum = newValue;
            enemy.HealthPoints.Base = newValue;
        }
    }

    private float ApplyAnticipationDurationBonus(float current, float lowerBound = .5f, float ratio = 15f)
    {
        var newValue = Mathf.Max(lowerBound, current - _state.Level / ratio);
        Plugin.DefaultLogger.LogDebug($"\tAnticipationDuration: {current} -> {newValue}");
        return newValue;
    }

}
