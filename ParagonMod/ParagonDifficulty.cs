namespace ParagonMod;

public enum ParagonDifficulty
{
    DEFAULT,
    MEDIUM,
    HARD,
    FOOLS,
    ABYSMAL
}

public static class DifficultyExtensions
{
    public static int GetDifficultyLevel(this ParagonDifficulty difficulty)
    {
        return difficulty switch
        {
            ParagonDifficulty.MEDIUM => 5,
            ParagonDifficulty.HARD => 10,
            ParagonDifficulty.FOOLS => 15,
            ParagonDifficulty.ABYSMAL => 20,
            _ => 0
        };
    }

    public static int GetDifficultyModifier(this ParagonDifficulty difficulty)
    {
        return difficulty switch
        {
            ParagonDifficulty.MEDIUM => 25,
            ParagonDifficulty.HARD => 50,
            ParagonDifficulty.FOOLS => 100,
            ParagonDifficulty.ABYSMAL => 250,
            _ => 0
        };
    }
}
