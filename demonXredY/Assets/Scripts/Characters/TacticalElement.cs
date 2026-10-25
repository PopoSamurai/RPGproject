using UnityEngine;

public enum TacticalElement { Fire, Ice, Lightning, Light, Dark }

public static class TacticalElementRules
{
    // Ice > Fire > Lightning > Ice. Light and Dark counter each other.
    // A strong matchup is x1.5; a weak matchup is x0.75.
    public static float Multiplier(TacticalElement attack, TacticalElement defense)
    {
        if (attack == defense) return 0.75f;
        switch (attack)
        {
            case TacticalElement.Ice:
                if (defense == TacticalElement.Fire) return 1.5f;
                if (defense == TacticalElement.Lightning) return 0.75f;
                break;
            case TacticalElement.Fire:
                if (defense == TacticalElement.Lightning) return 1.5f;
                if (defense == TacticalElement.Ice) return 0.75f;
                break;
            case TacticalElement.Lightning:
                if (defense == TacticalElement.Ice) return 1.5f;
                if (defense == TacticalElement.Fire) return 0.75f;
                break;
            case TacticalElement.Light:
                if (defense == TacticalElement.Dark) return 1.5f;
                break;
            case TacticalElement.Dark:
                if (defense == TacticalElement.Light) return 1.5f;
                break;
        }
        return 1f;
    }

    public static int Resistance(TacticalStats stats, TacticalElement element)
    {
        switch (element)
        {
            case TacticalElement.Fire: return stats.fireRes;
            case TacticalElement.Ice: return stats.iceRes;
            case TacticalElement.Lightning: return stats.lightningRes;
            case TacticalElement.Light: return stats.lightRes;
            case TacticalElement.Dark: return stats.darkRes;
            default: return 0;
        }
    }
}
