public interface IBoostable
{
    void Boost(float boostAmount, AccuracyLevel level);
}

public enum AccuracyLevel
{
    PERFECT,
    GREAT,
    OKAY,
    POOR,
}
