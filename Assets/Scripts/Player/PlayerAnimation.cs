using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private GameController _gameController;
    private Animator _animator;
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.meterSlider.value < -_gameController.currentMeterSpreadValue)
        {
            _animator.SetBool("isSad", true);
            _animator.SetBool("isHappy", false);
        }
        
        else if (_gameController.meterSlider.value > _gameController.currentMeterSpreadValue)
        {
            _animator.SetBool("isSad", false);
            _animator.SetBool("isHappy", true);
        }
        else
        {
            _animator.SetBool("isSad", false);
            _animator.SetBool("isHappy", false);
        }
    }
}
