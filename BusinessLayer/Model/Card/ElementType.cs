using System;
using System.Collections.Generic;

public enum ElementType
{
    FIRE, WATER, NORMAL
}

public static class ElementTypeMethods
{
    public static String GetString(this ElementType type)
    {
        switch(type)
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
                    { ElementType.NORMAL, 2.0 },   // Fire > Normal
                    { ElementType.WATER, 0.5 }    // Fire < Water
                }
            },
            { ElementType.WATER, new Dictionary<ElementType, double>()
                {
                    { ElementType.FIRE, 2.0 },    // Water > Fire
                    { ElementType.NORMAL, 0.5 }   // Water < Normal
                }
            },
            { ElementType.NORMAL, new Dictionary<ElementType, double>()
                {
                    { ElementType.WATER, 2.0 },   // Normal > Water
                    { ElementType.FIRE, 0.5 }     // Normal < Fire
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
        return 1.0; // No effect
    }
}


