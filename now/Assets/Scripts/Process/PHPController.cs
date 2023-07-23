using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// HP UI 관리 프로세스
/// </summary>
public class PHPController : IMProcessor
{
    private Human _player = null;
    private Model _boss = null;
    private Image _hpBar = null;
    private Image _mpBar = null;
    private Slider _bossHpBar = null;

    public PHPController()
    {
        EventManager.CreateModel.AddListener(OnSetHp);
    }


    public void Update(List<IModel> models)
    {
        if (_hpBar != null && _player != null)
            _hpBar.fillAmount = (_player.HP / _player.MaxHP);
        if (_mpBar != null && _player != null)
            _mpBar.fillAmount = (_player.MP / _player.MaxMP);

        if (_bossHpBar != null && _boss != null)
            _bossHpBar.value = (_boss.HP / _boss.MaxHP);
    }

    private void OnSetHp(IModel imodel)
    {
        _hpBar = ViewManager.Instance.PlayerHp;
        _mpBar = ViewManager.Instance.PlayerMp;
        _bossHpBar = ViewManager.Instance.BossHp;

        if (imodel is Model model)
        {
            if (model.CampType == Model.Camp.Player && model is Human human)
                _player = human;
            if (model.CampType == Model.Camp.Boss)
                _boss = model;
        }
    }
}
