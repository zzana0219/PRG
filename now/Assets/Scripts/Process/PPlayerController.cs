using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 컨트롤 프로세스 (현재 사용 X)
/// </summary>
public class PPlayerController : IMProcessor
{
    private Human _player = null;
    private bool isClick = false;
    

    public PPlayerController()
    {
        EventManager.CreateModel.AddListener(OnSetPlayer);
    }

    public void Update(List<IModel> models)
    {
        if (_player == null) return;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            _player.IsWalking = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isClick = true;
        }
        if (Input.GetMouseButtonUp(0))
            isClick = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _player.IsJumping = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _player.IsAttack = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _player.IsAttack = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _player.IsAttack = true; 
        }
    }

    private void OnSetPlayer(IModel model)
    {
        if (model is Human player)
            if (player.CampType == Model.Camp.Player)
                _player = player;
    }

}
