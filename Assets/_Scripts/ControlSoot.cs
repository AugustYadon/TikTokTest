using System;
using UnityEngine;
using Random = UnityEngine.Random;



public class ControlSoot : MonoBehaviour
{
    public enum SootState {walking,waving,launching} //Enum just to make it more readable.
    
    private Animator _animator;

    private SootState _sootState = SootState.walking; //keeps track of what to do in the gameloop
    private DateTime _startDelay; //Breaks them up to make them look less static
    private float _walkingCounter; 
    private float _walkSpeed; //Helps with the same effect as _startDelay
    private Quaternion _startAngle;
    private Quaternion _endAngle;
    private const float WalkTime = 5f;
    
    [SerializeField] private float _gravity = 4.5f; //rather than using physics we're just making our own trajectory with a simplified high drag setup
    private DateTime _launchTime;
    private float _launchLength = 2f;
    private Vector3 _downVelocity = Vector3.zero;
    
    /// <summary>
    /// This is where we randomize and initialize variables when a soot sprite instantiated
    /// Randomization is to add variation to the creatures so that they look less static
    /// </summary>
    private void Start()
    {
        _startDelay = DateTime.Now.AddSeconds(Random.Range(0f,3f));
        _walkSpeed = Random.Range(1f, 2.5f); //
        
        _animator = GetComponent<Animator>();
        _animator.SetFloat("walkspeed", _walkSpeed); //match the animation speed with the 3D movement speed
        
        _startAngle = transform.parent.localRotation;
        _endAngle = _startAngle * Quaternion.Euler(0, 0, 90); //this is based on the orientation of the 3D model
    }

    /// <summary>
    /// Standard Update() Gameloop logic to do different actions depending on the current state which we define with bools
    /// </summary>
    private void Update()
    {
        switch(_sootState)
        {
            case SootState.launching:
                transform.parent.position += ((transform.up * 2f) + _downVelocity) * Time.deltaTime;
                _downVelocity += Vector3.down * _gravity * Time.deltaTime;
            
                if(DateTime.Now > _launchTime.AddSeconds(_launchLength))
                    Destroy(transform.parent.gameObject);
                break;
            case SootState.walking:
                if(DateTime.Now < _startDelay) return;
                _walkingCounter += (Time.deltaTime * _walkSpeed)/ WalkTime;
                transform.parent.localRotation = Quaternion.Lerp(_startAngle,_endAngle, _walkingCounter);
                break;
            case SootState.waving:
                //todo add waving animation
                break;
        }
    }

    /// <summary>
    /// A function that can be called to fling this Soot Sprite. Only used when the user shakes their head.
    /// </summary>
    public void Launch()
    {
        if (_sootState == SootState.launching) return;
        
        _sootState = SootState.launching;
        transform.parent.SetParent(null); //make sure we don't still have this soot set as a child of the face 
        _launchTime = DateTime.Now; // we want to delete these objects once they've significantly exited the viewport
        _animator.SetBool("launch",true);
    }
}
