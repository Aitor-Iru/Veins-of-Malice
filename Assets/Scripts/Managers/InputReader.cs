using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// InputReader — ScriptableObject centralizado para el Input System.
/// No depende de la clase C# generada por Unity (no requiere "Generate C# Class").
/// Se conecta directamente al asset .inputactions mediante la API de InputActionAsset.
///
/// Uso:
///   1. El asset se crea automáticamente desde GreyboxSceneBuilder (Tools > Veins of Malice > Create Greybox Scene)
///      o manualmente: Assets > Create > Veins of Malice > Input Reader
///   2. Asignar el InputReader en el Inspector del PlayerController.
/// </summary>
[CreateAssetMenu(fileName = "InputReader", menuName = "Veins of Malice/Input Reader")]
public class InputReader : ScriptableObject
{
    // ── Player Events ─────────────────────────────────────────────────────────
    public event Action<Vector2> OnMoveEvent;
    public event Action          OnJumpStarted;
    public event Action          OnJumpCanceled;
    public event Action          OnDashStarted;
    public event Action          OnAttackStarted;
    public event Action          OnInteractStarted;
    public event Action          OnPauseStarted;

    // ── UI Events ─────────────────────────────────────────────────────────────
    public event Action OnUISubmit;
    public event Action OnUICancel;

    // ── Internal ──────────────────────────────────────────────────────────────
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap _playerMap;
    private InputActionMap _uiMap;

    private InputAction _move;
    private InputAction _jump;
    private InputAction _dash;
    private InputAction _attack;
    private InputAction _interact;
    private InputAction _pause;
    private InputAction _submit;
    private InputAction _cancel;

    private void OnEnable()
    {
        // Load the asset from the known path if not assigned in Inspector
        if (inputActions == null)
            inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions")
                        ?? UnityEditor_LoadFallback();

        if (inputActions == null)
        {
            Debug.LogError("[InputReader] InputActionAsset not found. Assign it in the Inspector.");
            return;
        }

        _playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        _uiMap     = inputActions.FindActionMap("UI",     throwIfNotFound: true);

        // Bind player actions
        _move     = _playerMap.FindAction("Move",     throwIfNotFound: true);
        _jump     = _playerMap.FindAction("Jump",     throwIfNotFound: true);
        _dash     = _playerMap.FindAction("Dash",     throwIfNotFound: true);
        _attack   = _playerMap.FindAction("Attack",   throwIfNotFound: true);
        _interact = _playerMap.FindAction("Interact", throwIfNotFound: true);
        _pause    = _playerMap.FindAction("Pause",    throwIfNotFound: true);

        // Bind UI actions
        _submit = _uiMap.FindAction("Submit", throwIfNotFound: true);
        _cancel = _uiMap.FindAction("Cancel", throwIfNotFound: true);

        // Register callbacks
        _move.performed  += OnMove;
        _move.canceled   += OnMove;
        _jump.started    += OnJump;
        _jump.canceled   += OnJumpEnd;
        _dash.started    += OnDash;
        _attack.started  += OnAttack;
        _interact.started += OnInteract;
        _pause.started   += OnPause;
        _submit.started  += OnSubmit;
        _cancel.started  += OnCancel;

        EnableGameplayInput();
    }

    private void OnDisable()
    {
        if (_move == null) return;

        _move.performed  -= OnMove;
        _move.canceled   -= OnMove;
        _jump.started    -= OnJump;
        _jump.canceled   -= OnJumpEnd;
        _dash.started    -= OnDash;
        _attack.started  -= OnAttack;
        _interact.started -= OnInteract;
        _pause.started   -= OnPause;
        _submit.started  -= OnSubmit;
        _cancel.started  -= OnCancel;

        DisableAllInput();
    }

    // ── Input Mode Switching ──────────────────────────────────────────────────

    public void EnableGameplayInput()
    {
        _playerMap?.Enable();
        _uiMap?.Disable();
    }

    public void EnableUIInput()
    {
        _playerMap?.Disable();
        _uiMap?.Enable();
    }

    public void DisableAllInput()
    {
        _playerMap?.Disable();
        _uiMap?.Disable();
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private void OnMove(InputAction.CallbackContext ctx)
        => OnMoveEvent?.Invoke(ctx.ReadValue<Vector2>());

    private void OnJump(InputAction.CallbackContext ctx)
        => OnJumpStarted?.Invoke();

    private void OnJumpEnd(InputAction.CallbackContext ctx)
        => OnJumpCanceled?.Invoke();

    private void OnDash(InputAction.CallbackContext ctx)
        => OnDashStarted?.Invoke();

    private void OnAttack(InputAction.CallbackContext ctx)
        => OnAttackStarted?.Invoke();

    private void OnInteract(InputAction.CallbackContext ctx)
        => OnInteractStarted?.Invoke();

    private void OnPause(InputAction.CallbackContext ctx)
        => OnPauseStarted?.Invoke();

    private void OnSubmit(InputAction.CallbackContext ctx)
        => OnUISubmit?.Invoke();

    private void OnCancel(InputAction.CallbackContext ctx)
        => OnUICancel?.Invoke();

    // ── Fallback loader (Editor only) ─────────────────────────────────────────

    private InputActionAsset UnityEditor_LoadFallback()
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(
            "Assets/InputSystem_Actions.inputactions");
#else
        return null;
#endif
    }
}
