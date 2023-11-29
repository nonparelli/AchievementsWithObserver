using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        JUMPING,
        STANDING,
        CROUCHING,
        METAL_JUMPING,
        METAL_STANDING,
        METAL_CROUCHING
    }
    // Keep Track of Playerstate
    public PlayerState my_state = PlayerState.JUMPING;

    Rigidbody _rigidbody;

    [SerializeField]
    bool _startUndo = false;
    [SerializeField]
    bool _undoActive = false;
    [SerializeField]
    bool _replayActive = false, _startReplay = false;
    [SerializeField]
    float _waitTime = 0.1f;
    [SerializeField]
    Material _defaultMaterial, _camoMaterial,_metalMaterial;

    // Commands
    Command cmd_W = new MoveForwardCommand();
    Command cmd_A = new MoveLeftCommand();
    Command cmd_S = new MoveBackwardCommand();
    Command cmd_D = new MoveRightCommand();
    CommandHistory _history = new CommandHistory();

    private void OnTriggerEnter(Collider other)
    {
        Enemy isenemy = other.gameObject.GetComponent<Enemy>();
        if (isenemy != null)
        {
            if (my_state == PlayerState.METAL_JUMPING || my_state == PlayerState.METAL_STANDING)
                isenemy.OnDamaged(6f);
            else
                isenemy.OnDamaged(2f);
            return;
        }
        // Otherwise it's a coin so
        if (my_state == PlayerState.JUMPING)
        { my_state = PlayerState.METAL_JUMPING; gameObject.GetComponent<MeshRenderer>().material = _metalMaterial; }
        else if (my_state == PlayerState.STANDING)
        { my_state = PlayerState.METAL_STANDING; gameObject.GetComponent<MeshRenderer>().material = _metalMaterial;}
        else if(my_state==PlayerState.CROUCHING)
            my_state = PlayerState.METAL_CROUCHING;
        Destroy(other.gameObject); // Destroy the coin!

    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we're in a replay state
        if (_startReplay)
        {
            StartCoroutine(ReplayCommands());
            _startReplay = false;
            return;
        }
        if (_startUndo && _history.undo.Count > 0)
        {
            //_undo_commands.Pop().Undo(_rigidbody);
            _undoActive = true;
            StartCoroutine(UndoCommand());
            _startUndo = false;
            return;

        }
        else if (_undoActive || _replayActive)
            return;
        //
        _startUndo = false;

        // Player inputs
        switch (my_state)
        {
            case PlayerState.JUMPING: // Jumping is disabled, Crouching is disabled
                DirectionalInputs();
                break;

            case PlayerState.STANDING: // Jumping is enabled, Crouching is enabled
                if (Input.GetKeyDown(KeyCode.LeftControl))
                { my_state = PlayerState.CROUCHING; gameObject.GetComponent<MeshRenderer>().material = _camoMaterial; }
                else if (Input.GetKeyUp(KeyCode.LeftControl))
                { my_state = PlayerState.STANDING; gameObject.GetComponent<MeshRenderer>().material = _defaultMaterial; }
                JumpInputs();
                DirectionalInputs();
                break;

            case PlayerState.CROUCHING: // Jumping is disabled
                if (Input.GetKeyDown(KeyCode.LeftControl))
                { my_state = PlayerState.CROUCHING; gameObject.GetComponent<MeshRenderer>().material = _camoMaterial; }
                else if (Input.GetKeyUp(KeyCode.LeftControl))
                { my_state = PlayerState.STANDING; gameObject.GetComponent<MeshRenderer>().material = _defaultMaterial; }
                DirectionalInputs();
                break;
            case PlayerState.METAL_JUMPING: // Jumping is disabled, Crouching is disabled
                DirectionalInputs();
                break;

            case PlayerState.METAL_STANDING: // Jumping is enabled, Crouching is enabled
                if (Input.GetKeyDown(KeyCode.LeftControl))
                { my_state = PlayerState.METAL_CROUCHING; gameObject.GetComponent<MeshRenderer>().material = _camoMaterial; }
                else if (Input.GetKeyUp(KeyCode.LeftControl))
                { my_state = PlayerState.METAL_STANDING; gameObject.GetComponent<MeshRenderer>().material = _metalMaterial; }
                JumpInputs();
                DirectionalInputs();
                break;

            case PlayerState.METAL_CROUCHING: // Jumping is disabled
                if (Input.GetKeyDown(KeyCode.LeftControl))
                { my_state = PlayerState.METAL_CROUCHING; gameObject.GetComponent<MeshRenderer>().material = _camoMaterial; }
                else if (Input.GetKeyUp(KeyCode.LeftControl))
                { my_state = PlayerState.METAL_STANDING; gameObject.GetComponent<MeshRenderer>().material = _metalMaterial; }
                DirectionalInputs();
                break;


            default:
                break;
        }

    }

    void JumpInputs()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (my_state == PlayerState.METAL_STANDING)
            {
                _rigidbody.AddForce(5.0f * transform.up, ForceMode.Impulse);
                my_state = PlayerState.METAL_JUMPING;
            }
            if (my_state == PlayerState.STANDING)
            {
                _rigidbody.AddForce(10.0f * transform.up, ForceMode.Impulse);
                my_state = PlayerState.JUMPING;
            }
        }
    }

    void CrouchInputs()
    {
        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
            my_state = PlayerState.CROUCHING;

        if (Input.GetKeyUp(KeyCode.LeftControl))
            my_state = PlayerState.STANDING;
    }

    void DirectionalInputs()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            cmd_W.Execute(_rigidbody);

            _history.undo.Push(cmd_W);

            _history.redo.Clear();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            cmd_A.Execute(_rigidbody);

            _history.undo.Push(cmd_A);

            _history.redo.Clear();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cmd_S.Execute(_rigidbody);

            _history.undo.Push(cmd_S);

            _history.redo.Clear();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cmd_D.Execute(_rigidbody);

            _history.undo.Push(cmd_D);

            _history.redo.Clear();
        }
    }

    void ReplayControls()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        { // undo, do we need to add this to replay somehow??
            if (_history.undo.Count > 0)
            {
                _history.redo.Push(_history.undo.Peek());
                _history.undo.Pop().Undo(_rigidbody);
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) // rewind undo
        {
            _startUndo = true;
        }
        if (Input.GetKeyDown(KeyCode.R)) // redo
        {
            if (_history.redo.Count > 0)
            {
                _history.undo.Push(_history.redo.Peek());
                _history.redo.Pop().Execute(_rigidbody);
            }
        }
        if (Input.GetKeyDown(KeyCode.T)) // Replay moves so far from start
        {
            _startReplay = true;
        }
        /*
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwapCommands(ref cmd_A,ref cmd_D);
        }
        */
    }

    // We can jump again whenever we hit Anything
    void OnCollisionEnter()
    {
        if(my_state==PlayerState.JUMPING)
         my_state = PlayerState.STANDING;
        if (my_state == PlayerState.METAL_JUMPING)
            my_state = PlayerState.METAL_STANDING;
    }

    IEnumerator UndoCommand()
    {

        do
        {
            _history.redo.Push(_history.undo.Peek());
            _history.undo.Pop().Undo(_rigidbody);
            yield return new WaitForSeconds(_waitTime);
        } while (_history.undo.Count > 0);

        _undoActive = false;
    }
    IEnumerator ReplayCommands()
    {

        Stack<Command> temp = new Stack<Command>();
        while (_history.undo.Count > 0)
        {
            temp.Push(_history.undo.Peek());
            _history.undo.Pop().Undo(_rigidbody);
        }

        while (temp.Count > 0)
        {
            _history.undo.Push(temp.Peek());
            temp.Pop().Execute(_rigidbody);
            yield return new WaitForSeconds(_waitTime);
        }
        _replayActive = false;
    }

}
