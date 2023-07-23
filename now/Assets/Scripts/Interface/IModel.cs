using System;
using UnityEngine;

public interface IModel : IDisposable
{
    /// <summary> 
    /// 이름
    /// </summary>
    string Name { get; set; }

    /// <summary> 
    /// 고유값
    /// </summary>
    int ID { get; }

    /// <summary> 
    /// 시작 위치
    /// </summary>
    Vector3 Position { get; }

    /// <summary> 
    /// 프리팹 위치
    /// </summary>
    string Route { get; }

    /// <summary> 
    /// 초기화
    /// </summary>
    void OnInit();
}
