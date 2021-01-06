using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private GameManager _gameManager;
    private Animator _animator;
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.meterSlider.value < -_gameManager.currentMeterSpreadValue)
        {
            _animator.SetBool("isSad", true);
            _animator.SetBool("isHappy", false);
        }
        
        else if (_gameManager.meterSlider.value > _gameManager.currentMeterSpreadValue)
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
