using System;
using System.Linq;
using ParagonMod.Patch;

namespace ParagonMod;

public class ParagonMapManager
{
    private readonly ParagonState _state;
    private readonly WeakReference<GameState> _gameState;

    private int _currentSectorIdx = 0;
    private Sector _currentSector = null;
    private bool _justChangedSector = false;

    public ParagonMapManager(ParagonState state, WeakReference<GameState> gameState)
    {
        _state = state;
        _gameState = gameState;
        ExtendedBoatHelm.OnActivate += OnBoatHelmActivate;
        ExtendedEncounterManager.OnSelectEncounter += OnSelectEncounter;
    }

    private bool RunNotStarted => _currentSectorIdx == 0;
    private bool ShouldRandomlyChangeSector => _state.EndlessLevel > 0 && !_justChangedSector && _state.EndlessLevel % UnityEngine.Random.Range(3, 7) == 0;

    private bool OnBoatHelmActivate()
    {
        if (_state.CurrentRunType != ParagonState.RunType.ENDLESS || !_gameState.TryGetTarget(out GameState gameState))
            return true;
        if ((RunNotStarted || ShouldRandomlyChangeSector) && ChangeSector(gameState))
        {
            _justChangedSector = true;
            return false;
        }
        _justChangedSector = false;
        MapNode node = RandomMapNode();
        Plugin.DefaultLogger.LogDebug($"New random map node of type {node.Type.ToString()} and subtype {node.Subtype.ToString()} with reward type {node.RewardType.ToString()} for endless mode");
        gameState.gameEvents.NextNodeSelected(node);
        return false;
    }

    private MapNode RandomMapNode()
    {
        switch (UnityEngine.Random.Range(0f, 1f))
        {
            case < 0.75f:
                return RandomEncounterMapNode();
            case < 0.95f:
                return RandomIslandMapNode();
            default:
                return RandomBossMapNode();
        }
    }

    private MapNode.MapNodeRewardType RandomRewardType()
    {
        switch (UnityEngine.Random.Range(0f, 1f))
        {
            case < 0.50f:
                return MapNode.MapNodeRewardType.None;
            case < 0.85f:
                return MapNode.MapNodeRewardType.Basic;
            default:
                return MapNode.MapNodeRewardType.Rare;
        }
    }

    private MapNode.MapNodeSubtype RandomIslandType()
    {
        return (MapNode.MapNodeSubtype)OneOf(
            _currentSector.islandConfigurations
                .Where(configuration =>
                    configuration.type != MapNode.IslandNodeSubtype.Junkyard
                    && configuration.type != MapNode.IslandNodeSubtype.Curse
                    && configuration.type != MapNode.IslandNodeSubtype.EntranceScenery
                    && configuration.type != MapNode.IslandNodeSubtype.NpcUnlock)
                .Select(configuration => configuration.type)
                .ToArray()
        );
    }

    private MapNode RandomIslandMapNode()
    {
        return new MapNode
        {
            Ending = false,
            Stormed = false,
            Type = MapNode.MapNodeType.Island,
            Subtype = RandomIslandType(),
            RewardType = RandomRewardType()
        };
    }

    private MapNode.MapNodeSubtype RandomEncounterType()
    {
        switch (UnityEngine.Random.Range(0f, 1f))
        {
            case < 0.80f:
                return MapNode.MapNodeSubtype.Standard;
            default:
                return MapNode.MapNodeSubtype.Special;
        }
    }

    private MapNode RandomEncounterMapNode()
    {
        return new MapNode
        {
            Ending = false,
            Stormed = false,
            Type = MapNode.MapNodeType.Encounter,
            Subtype = RandomEncounterType(),
            RewardType = RandomRewardType()
        };
    }

    private MapNode RandomBossMapNode()
    {
        return new MapNode
        {
            Ending = false,
            Stormed = false,
            Type = MapNode.MapNodeType.Encounter,
            Subtype = MapNode.MapNodeSubtype.Boss,
            RewardType = RandomRewardType()
        };
    }

    private bool ChangeSector(GameState gameState)
    {
        var newSector = UnityEngine.Random.Range(1, 4);
        if (newSector == _currentSectorIdx)
            return false;
        _currentSectorIdx = newSector;
        gameState.gameEvents.NextSectorSelected(_currentSectorIdx);
        return true;
    }

    private Encounter OnSelectEncounter(EncounterManager encounterManager, MapNode node)
    {
        if (_state.CurrentRunType != ParagonState.RunType.ENDLESS)
            return null;
        var difficultyLevel = _state.CurrentDifficulty.GetDifficultyLevel();
        if (node.LinkedEncounterName != null)
            return encounterManager.GetSpecificEncounterByName(node.LinkedEncounterName, difficultyLevel);
        switch (node.Subtype)
        {
            case MapNode.MapNodeSubtype.Boss:
                return RandomEncounterFrom(encounterManager.bossEncounters, difficultyLevel);
            case MapNode.MapNodeSubtype.Special:
                return RandomEncounterFrom(encounterManager.specialEncounters, difficultyLevel);
            default:
                return RandomEncounterFrom(encounterManager.standardEncounters, difficultyLevel);
        }
    }

    private static Encounter RandomEncounterFrom(Encounter[] encounters, int difficultyLevel)
    {
        Encounter encounter = encounters[UnityEngine.Random.Range(0, encounters.Length)];
        encounter.difficultyLevel = difficultyLevel;
        return encounter;
    }

    public void Reset()
    {
        _currentSectorIdx = 0;
    }

    private static T OneOf<T>(params T[] items)
    {
        return items[UnityEngine.Random.Range(0, items.Length)];
    }

    public void OnSectorChanged(Sector sector)
    {
        _currentSector = sector;
    }
}
