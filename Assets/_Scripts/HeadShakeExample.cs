using System.Collections;
using UnityEngine;

public class HeadShakeExample : MonoBehaviour
{
    [SerializeField] private Transform _headTransform; //used to track head movement and also to place Soots
    [SerializeField] private GameObject _sootSpritesPrefab;
    [SerializeField] private GameObject _exclamation;
    [SerializeField] private float _speedThreshold;
    [SerializeField] private float _angularSpeedThreshold;
    private float[] _recentSpeeds = new float[5];
    private float[] _recentAngularSpeeds = new float[5];
    private float _currentAverageSpeed;
    private float _currentAverageAngularSpeed;
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private int _counter;
    private bool _isMoving, _isShaking = true; //we start with a shake and wait for the user to be still

    [SerializeField] private bool _useExclamation = false;
    
    /// <summary>
    /// Classic Unity initialization
    /// </summary>
    private void Start()
    {
        _lastPosition = _headTransform.position;
        _lastRotation = _headTransform.rotation;
        _speedThreshold *= _speedThreshold; //Get the square so that we can compare it against square magnitudes
        StartCoroutine(Shake());
    }

    /// <summary>
    /// Game loop mostly for updating the average movement of user's head over the last 5 frames
    /// Secondly to trigger events, like calling Shake()
    /// </summary>
    private void Update()
    {
        _currentAverageAngularSpeed -= _recentAngularSpeeds[_counter] / _recentAngularSpeeds.Length;
        _currentAverageSpeed -= _recentSpeeds[_counter]/_recentSpeeds.Length;
        
        _recentAngularSpeeds[_counter] = Quaternion.Angle(_headTransform.rotation, _lastRotation) / Time.deltaTime;
        _recentSpeeds[_counter] = (_headTransform.position - _lastPosition).sqrMagnitude/Time.deltaTime;
        
        _currentAverageAngularSpeed += _recentAngularSpeeds[_counter] / _recentAngularSpeeds.Length;
        _currentAverageSpeed += _recentSpeeds[_counter]/_recentSpeeds.Length;
        
        _counter = ++_counter % _recentSpeeds.Length;
        
        _lastPosition = _headTransform.position;
        _lastRotation = _headTransform.rotation;

        if (_currentAverageSpeed < _speedThreshold && _currentAverageAngularSpeed < _angularSpeedThreshold)
        {
            _isMoving = false;
            return;
        }
        if(!_isShaking) //check if head went from moving to stopped during this frame
        {
            _isShaking = true;
            StartCoroutine(Shake());
        }
        _isMoving = true;
    }
    
    /// <summary>
    /// Here we call the Launch() method of each SootSprite to make them fly off your head when you shake it
    /// We then wait for 2 seconds (And then we also wait until the user is not shaking) until we spawn new Soot Sprites 
    /// </summary>
    private IEnumerator Shake()
    {
        _headTransform.BroadcastMessage("Launch",SendMessageOptions.DontRequireReceiver);
        if(_useExclamation)
            _exclamation.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        _exclamation.SetActive(false);
        yield return new WaitWhile(() => _isMoving);
        Instantiate(_sootSpritesPrefab,_headTransform,false);
        _isShaking = false;
    }
    
}
