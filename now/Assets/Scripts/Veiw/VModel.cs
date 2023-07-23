using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ºä ¸ðµ¨ Ãß»ó Å¬·¡½º
/// </summary>
public abstract class VModel
{
    public string Route { get => route + PersonalRoute; }

    private readonly string route = $"Model/";
    protected abstract string PersonalRoute { get; }
    
    public GameObject Object;

    public BillBoard billBoard;
}
