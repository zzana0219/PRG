using System.Collections;
using UnityEngine;

/// <summary>
/// �� ���̷��� ��ũ��Ʈ
/// </summary>
public class VSkeleton : VModel
{
    protected override string PersonalRoute => "Skeleton";

    public VSkeleton(GameObject gameObject)
    {
        Object = gameObject;
    }
}
