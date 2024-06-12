using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ParagonMod;

public class ParagonEnemyManager
{
    private readonly ParagonState _state;

    public ParagonEnemyManager(ParagonState state)
    {
        _state = state;
    }

    private bool TryCast<T>(EnemyAi ai, out T output) where T : Il2CppObjectBase
    {
        output = ai.TryCast<T>();
        return output != null;
    }

    public void OnEnemySpawn(Enemy enemy)
    {
        if ((_state.CurrentRunType == ParagonState.RunType.PARAGON || _state.CurrentDifficulty != ParagonDifficulty.DEFAULT) && !enemy.hideFlags.HasFlag(HideFlags.NotEditable))
        {
            enemy.hideFlags |= HideFlags.NotEditable;
            Mod.DefaultLogger.Msg($"Applying bonuses to enemy [{enemy.name}][&{enemy.GetInstanceID()}][@{enemy.enemyAi?.GetType()}][^{enemy.enemyAi?.Movement?.GetType()}]");
            ApplyHealthPointsBonus(enemy);
            if (enemy.enemyAi != null) {
                ApplyMovementBonus(enemy);
                if (TryCast(enemy.enemyAi, out CrabsterEnemyAi crabster))
                {
                    crabster.attackDelay = ApplyMinAttackDelayBonus(crabster.attackDelay);
                }
                else if (TryCast(enemy.enemyAi, out CrawlerAi crawler))
                {
                    crawler.anticipationTime = ApplyAnticipationDurationBonus(crawler.anticipationTime);
                }
                else if (TryCast(enemy.enemyAi, out SpiderAi spider))
                {
                    spider.minAttackDelay = ApplyMinAttackDelayBonus(spider.minAttackDelay);
                    spider.maxAttackDelay = ApplyMaxAttackDelayBonus(spider.minAttackDelay);
                }
                else if (TryCast(enemy.enemyAi, out BullyAi bully))
                {
                    bully.anticipationTime = ApplyAnticipationDurationBonus(bully.anticipationTime);
                    bully.minDelayBeforeAttack = ApplyMinAttackDelayBonus(bully.minDelayBeforeAttack);
                    bully.maxDelayBeforeAttack = ApplyMaxAttackDelayBonus(bully.maxDelayBeforeAttack);
                    bully.attackProbability = ApplyAttackProbabilityBonus(bully.attackProbability);
                }
                else if (TryCast(enemy.enemyAi, out PuffyAi puffy))
                {
                }
                else if (TryCast(enemy.enemyAi, out HatcherAi hatcher))
                {
                    hatcher.initialEggLayFrequency = Mathf.Max(6f, hatcher.initialEggLayFrequency - _state.DifficultyModifier / 10f);
                    hatcher.minimumEggLayFrequency = Mathf.Max(2f, hatcher.minimumEggLayFrequency - _state.DifficultyModifier / 10f);
                }
                else if (TryCast(enemy.enemyAi, out OwletAi owlet))
                {
                    owlet.chargeBoatMovement.defaultSpeed *= 1f + _state.DifficultyModifier / 15f;
                    ApplyRotationZonesBonus(owlet.rotationZoneMovement.rotationZones);
                }
                else if (TryCast(enemy.enemyAi, out OwlAi owl))
                {
                    owl.attackDelay = ApplyMinAttackDelayBonus(owl.attackDelay);
                    ApplyRotationZonesBonus(owl.rotationZones);
                }
                else if (TryCast(enemy.enemyAi, out PeckerAi pecker))
                {
                    pecker.anticipationTime = ApplyAnticipationDurationBonus(pecker.anticipationTime);
                    pecker.anticipationTimer = new Il2Cpp.Timer(pecker.anticipationTime);
                }
                else if (TryCast(enemy.enemyAi, out FlyerEnemyAi flyer))
                {
                    flyer.anticipationDelay = ApplyAnticipationDurationBonus(flyer.anticipationDelay);
                    flyer.minimumAttackDelay = ApplyMinAttackDelayBonus(flyer.minimumAttackDelay);
                    flyer.maximumAttackDelay = ApplyMaxAttackDelayBonus(flyer.maximumAttackDelay);
                }
                else if (TryCast(enemy.enemyAi, out ScannerAi scanner))
                {
                    scanner.anticipationTime = ApplyAnticipationDurationBonus(scanner.anticipationTime);
                    scanner.minimumAttackDelayTimer = new Il2Cpp.Timer(Mathf.Max(1f, scanner.minimumAttackDelayTimer.Duration - _state.DifficultyModifier / 15f));
                }
                else if (TryCast(enemy.enemyAi, out TentacleAi tentacle))
                {
                    tentacle.anticipationTime = ApplyAnticipationDurationBonus(tentacle.anticipationTime);
                    tentacle.anticipationTimer = new Il2Cpp.Timer(tentacle.anticipationTime);
                }
                else if (TryCast(enemy.enemyAi, out TentaclesHeadAi tentaclesHead))
                {
                    tentaclesHead.maxSlurgEggAmount += Mathf.RoundToInt(_state.DifficultyModifier / 50f);
                }
                else if (TryCast(enemy.enemyAi, out SerpentExtremityAi serpentExtremity))
                {
                    serpentExtremity.anticipationTime = ApplyAnticipationDurationBonus(serpentExtremity.anticipationTime);
                    serpentExtremity.anticipationTimer = new Il2Cpp.Timer(serpentExtremity.anticipationTime);
                    serpentExtremity.attackTime = ApplyAttackTimeBonus(serpentExtremity.attackTime);
                    serpentExtremity.attackTimer = new Il2Cpp.Timer(serpentExtremity.attackTime);
                }
                else if (TryCast(enemy.enemyAi, out SerpentLoopAi serpentLoop))
                {
                }
                else if (TryCast(enemy.enemyAi, out ClawsHeadAi clawsHead))
                {
                    clawsHead.anticipationDuration = ApplyAnticipationDurationBonus(clawsHead.anticipationDuration);
                    clawsHead.minimumAttackDelay = ApplyMinAttackDelayBonus(clawsHead.minimumAttackDelay, 2f, 10f);
                    clawsHead.maximumAttackDelay = ApplyMaxAttackDelayBonus(clawsHead.maximumAttackDelay, 4f, 10f);
                }
                else Mod.DefaultLogger.Msg($"Unknown enemy ai: {enemy.enemyAi}");
            }
        }
    }

    private float ApplyMinAttackDelayBonus(float current, float lowerBound = .5f, float ratio = 15f)
    {
        var newValue = Mathf.Max(Mathf.Min(current, lowerBound), current - _state.DifficultyModifier / ratio);
        Mod.DefaultLogger.Msg($"\tMinAttackDelay: {current} -> {newValue}");
        return newValue;
    }

    private float ApplyMaxAttackDelayBonus(float current, float lowerBound = .8f, float ratio = 15f)
    {
        var newValue = Mathf.Max(Mathf.Min(current, lowerBound), current - _state.DifficultyModifier / ratio);
        Mod.DefaultLogger.Msg($"\tMaxAttackDelay: {current} -> {newValue}");
        return newValue;
    }

    private float ApplyAttackProbabilityBonus(float current)
    {
        var newValue = Mathf.Min(.8f, current + _state.DifficultyModifier / 500f);
        Mod.DefaultLogger.Msg($"\tAttackProbability: {current} -> {newValue}");
        return newValue;
    }

    private float ApplyAttackTimeBonus(float current)
    {
        var newValue = current + _state.DifficultyModifier / 10f;
        Mod.DefaultLogger.Msg($"\tAttackTime: {current} -> {newValue}");
        return newValue;
    }

    private void ApplyMovementBonus(Enemy enemy, float ratio = 15f)
    {
        var movement = enemy.enemyAi.Movement;
        if (movement != null)
        {
            var newValue = movement.defaultSpeed * (1f + _state.DifficultyModifier / ratio);
            Mod.DefaultLogger.Msg($"\tMovementSpeed: {movement.defaultSpeed} -> {newValue}");
            movement.defaultSpeed = newValue;
            movement.speed = newValue;
            if (movement is RotationZonesMovement rotationZonesMovement && enemy.enemyAi is not TentacleAi) // Tentacle boss dies instantly when modifying rotation zones ¯\_(ツ)_/¯
                ApplyRotationZonesBonus(rotationZonesMovement.rotationZones, ratio);
        }
    }

    private void ApplyRotationZonesBonus(Il2CppReferenceArray<RotationZone> rotationZones, float ratio = 50f)
    {
        foreach (RotationZone zone in rotationZones)
        {
            ApplyRotationZoneBonus(zone, ratio);
        }
    }

    private void ApplyRotationZonesBonus(Il2CppSystem.Collections.Generic.List<RotationZone> rotationZones, float ratio = 50f)
    {
        foreach (RotationZone zone in rotationZones)
        {
            ApplyRotationZoneBonus(zone, ratio);
        }
    }

    private void ApplyRotationZoneBonus(RotationZone rotationZone, float ratio)
    {
        if (!rotationZone.hideFlags.HasFlag(HideFlags.NotEditable))
        {
            var newValue = rotationZone.angularVelocity * (1 + _state.DifficultyModifier / ratio);
            if (newValue < -100f)
                newValue = -100f;
            else if (newValue > 100f)
                newValue = 100f;

            Mod.DefaultLogger.Msg($"\t{rotationZone.name}: {rotationZone.angularVelocity} -> {newValue}");
            rotationZone.angularVelocity = newValue;
            rotationZone.hideFlags |= HideFlags.NotEditable;
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
            Mod.DefaultLogger.Msg($"\tHealth: {enemy.HealthPoints.Maximum} -> {newValue}");
            enemy.HealthPoints.Maximum = newValue;
            enemy.HealthPoints.Base = newValue;
        }
    }

    private float ApplyAnticipationDurationBonus(float current, float lowerBound = .5f, float ratio = 15f)
    {
        var newValue = Mathf.Max(Mathf.Min(current, lowerBound), current - _state.DifficultyModifier / ratio);
        Mod.DefaultLogger.Msg($"\tAnticipationDuration: {current} -> {newValue}");
        return newValue;
    }

}
