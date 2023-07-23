using System;
using UnityEngine;

public interface IModel : IDisposable
{
    /// <summary> 
    /// �̸�
    /// </summary>
    string Name { get; set; }

    /// <summary> 
    /// ������
    /// </summary>
    int ID { get; }

    /// <summary> 
    /// ���� ��ġ
    /// </summary>
    Vector3 Position { get; }

    /// <summary> 
    /// ������ ��ġ
    /// </summary>
    string Route { get; }

    /// <summary> 
    /// �ʱ�ȭ
    /// </summary>
    void OnInit();
}
