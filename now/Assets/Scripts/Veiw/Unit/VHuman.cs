
using UnityEngine;

/// <summary>
/// �� �޸� ��ũ��Ʈ
/// </summary>
public class VHuman : VModel
{
    protected override string PersonalRoute => "Human";

    public VHuman(GameObject gameObject)
    {
        Object = gameObject;
    }
}
