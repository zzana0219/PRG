using UnityEngine;

/// <summary>
/// 휴먼 인게임 시뮬레이션 정보 스크립트
/// </summary>
public class Human : Model
{
    public override string Route { get => "Model/Human"; }
    public override int MaxHP { get => maxHP; }
    public override int MaxMP => 100;
    public override Status Status { get => status; set => status = value; }
    public override Vector3 Position { get => position; }
    public override Camp CampType { get => type; }

    private Status status;
    private Vector3 position;
    private Camp type;
    private int maxHP;

    public Human() { }
    public Human(string name, Vector3 position, Camp camp = Camp.Ally, int MaxHp = 300) 
    { 
        Name = name; 
        this.position = position;
        this.type = camp;
        this.maxHP = MaxHp;
    }

    public override void OnInit()
    {
        if (Name == null)
            Name = "Human";
        HP = MaxHP;
        MP = MaxMP;
        status = Status.Idle;
    }

    public override void Dispose()
    {
    }
}
