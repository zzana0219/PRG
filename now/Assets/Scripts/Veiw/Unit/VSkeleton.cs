using System.Collections;
using UnityEngine;

/// <summary>
/// ºä ½ºÄÌ·¹Åæ ½ºÅ©¸³Æ®
/// </summary>
public class VSkeleton : VModel
{
    protected override string PersonalRoute => "Skeleton";

    public VSkeleton(GameObject gameObject)
    {
        Object = gameObject;
    }
}
