
using UnityEngine;

/// <summary>
/// ºä ÈÞ¸Õ ½ºÅ©¸³Æ®
/// </summary>
public class VHuman : VModel
{
    protected override string PersonalRoute => "Human";

    public VHuman(GameObject gameObject)
    {
        Object = gameObject;
    }
}
