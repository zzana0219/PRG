using UnityEngine;

/// <summary>
/// 모델 인게임 시뮬레이션 정보 추상 클래스
/// </summary>
public abstract class Model : IModel
{
    protected Model() 
    {
        // 나중에 모델마다 고유코드 만드는 클래스 필요할듯?
        ID = GetHashCode();
    }

    // 모델 진영
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