namespace MTCG.BusinessLayer.Model.Card;

public enum ElementType
{
    FIRE, WATER, NORMAL
}

public static class ElementTypeMethods
{
    public static string GetString(this ElementType type)
    {
        switch (type)
        {
            case ElementType.FIRE:
                return "Fire ";
            case ElementType.WATER:
                return "Water ";
            default:
                return "";
        }
    }
}

public static class ElementEffectiveness
{

    private static readonly Dictionary<ElementType, Dictionary<ElementType, double>> effectivenessMap;

    static ElementEffectiveness()
    {
        effectivenessMap = new Dictionary<ElementType, Dictionary<ElementType, double>>()
        {
            { ElementType.FIRE, new Dictionary<ElementType, double>()
                {
                    { ElementType.NORMAL, 2.0 },
                    { ElementType.WATER, 0.5 }
                }
            },
            { ElementType.WATER, new Dictionary<ElementType, double>()
                {
                    { ElementType.FIRE, 2.0 },
                    { ElementType.NORMAL, 0.5 }
                }
            },
            { ElementType.NORMAL, new Dictionary<ElementType, double>()
                {
                    { ElementType.WATER, 2.0 },
                    { ElementType.FIRE, 0.5 }
                }
            }
        };
    }

    public static double GetEffectiveness(ElementType attacker, ElementType defender)
    {
        if (effectivenessMap.ContainsKey(attacker) && effectivenessMap[attacker].ContainsKey(defender))
        {
            return effectivenessMap[attacker][defender];
        }
        return 1.0;
    }
}


