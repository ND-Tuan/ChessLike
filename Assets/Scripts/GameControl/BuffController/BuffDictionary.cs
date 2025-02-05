using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuffDictionary
{

    public static Type GetBuffType(BuffID buffID)
    {
        return BuffMap[buffID];
    }

    private static readonly Dictionary<BuffID, Type> BuffMap = new Dictionary<BuffID, Type>
    {
        {BuffID.Bounce,         typeof(Bounce)},
        {BuffID.SoulEating,     typeof(SoulEater)},
        {BuffID.Freeze,         typeof(Freeze)},
        {BuffID.Burrn,          typeof(Burning)},
    };

    
}
