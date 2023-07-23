using UnityEngine;

/// <summary>
/// 스켈레톤 인게임 시뮬레이션 정보 스크립트
/// </summary>
public class Skeleton : Model
{
    public Skeleton() { }
    public Skeleton(string name) { Name = name; }
    public Skeleton(string name, Vector3 position, Camp camp = Camp.Enemy, int MaxHp = 50)
    {
        Name = name; 
        this.position = position; 
        this.type = camp;
        this.maxHP = MaxHp;
    }
    public override string Route => "Model/Skeleton";

    public override int MaxHP { get => maxHP; }
    public override int MaxMP => 50;
    public override Status Status { get => status; set => status = value; }
    public override Vector3 Position { get => position; }
    public override Camp CampType { get => type; }

    private Status status;
    private Vector3 position;
    private Camp type;
    private int maxHP;

    public override void OnInit()
    {
        if (Name == null)
            Name = "Skeleton";
        //Debug.Log($"{Name} : {ID}");
        HP = MaxHP;
        MP = MaxMP;
        status = Status.Idle;
    }

    public override void Dispose()
    {
    }
}
