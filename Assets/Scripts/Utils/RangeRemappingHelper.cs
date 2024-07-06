using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class to remap a value from one range to another
/// </summary>
public static class RangeRemappingHelper
{
    /// <summary>
    /// Remaps a value from one range to another, given the endpoints of the two ranges
    /// </summary>
    /// <param name="value"> The value to remap </param>
    /// <param name="in_high"> The high endpoint of the input range </param>
    /// <param name="in_low"> The low endpoint of the input range </param>
    /// <param name="out_high"> The high endpoint of the output range </param>
    /// <param name="out_low"> The low endpoint of the output range </param>
    /// <returns></returns>
    public static float Remap(float value, float in_high, float in_low, float out_high, float out_low)
    {
        return (value - in_low) * (out_high - out_low) / (in_high - in_low) + out_low;
    }
    
}
