using UnityEngine;

/// <summary>
/// �� �ΰ��� �ùķ��̼� ���� �߻� Ŭ����
/// </summary>
public abstract class Model : IModel
{
    protected Model() 
    {
        // ���߿� �𵨸��� �����ڵ� ����� Ŭ���� �ʿ��ҵ�?
        ID = GetHashCode();
    }

    // �� ����
    public enum Camp
    {
        Player = 0,
        Ally,
        Enemy,
        Boss,
        neutral,
    }

    public float HP;
    public float MP;

    public string Name { get; set; }
    public int ID { get; }
    public abstract string Route { get; }
    public abstract int MaxHP { get; }
    public abstract int MaxMP { get; }
    public abstract void OnInit();
    public abstract void Dispose();
    public abstract Vector3 Position { get; }
    public abstract Status Status { get; set; }
    public abstract Camp CampType { get; }

    public bool IsWalking { get; set; }
    public bool IsRunning { get; set; }
    public bool IsAttack { get; set; }
    public bool IsAttacked { get; set; }
    public bool IsJumping { get; set; }
}