using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class AnimationSetupTool : EditorWindow
{
    private const string ANIMATOR_PATH = "Assets/Art/Animations/PlayerAnimator.controller";
    private const string ANIM_FOLDER = "Assets/Art/Animations";

    [MenuItem("Tools/Veins of Malice/Setup Player Animator")]
    public static void SetupPlayerAnimator()
    {
        // Ensure directory exists
        if (!Directory.Exists(ANIM_FOLDER))
        {
            Directory.CreateDirectory(ANIM_FOLDER);
            AssetDatabase.Refresh();
        }

        // Create or load Animator Controller
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ANIMATOR_PATH);
        
        if (controller == null)
        {
            Debug.LogError($"[AnimationSetupTool] Failed to create animator at {ANIMATOR_PATH}");
            return;
        }

        // 1. Setup Parameters
        AddParameter(controller, "Speed", AnimatorControllerParameterType.Float);
        AddParameter(controller, "VerticalVelocity", AnimatorControllerParameterType.Float);
        AddParameter(controller, "IsGrounded", AnimatorControllerParameterType.Bool);
        AddParameter(controller, "IsDashing", AnimatorControllerParameterType.Bool);
        AddParameter(controller, "IsBlocking", AnimatorControllerParameterType.Bool);
        AddParameter(controller, "Attack", AnimatorControllerParameterType.Trigger);
        AddParameter(controller, "HeavyAttack", AnimatorControllerParameterType.Trigger);
        AddParameter(controller, "ComboStep", AnimatorControllerParameterType.Int);

        // 2. Setup Layers and States
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Locomotion State (Blend Tree placeholder or simple state)
        AnimatorState locomotionState = rootStateMachine.AddState("Locomotion");
        rootStateMachine.defaultState = locomotionState;

        // Air States
        AnimatorState jumpState = rootStateMachine.AddState("Jump");
        AnimatorState fallState = rootStateMachine.AddState("Fall");

        // Combat States
        AnimatorState attackState = rootStateMachine.AddState("Attack");
        AnimatorState heavyAttackState = rootStateMachine.AddState("HeavyAttack");
        AnimatorState blockState = rootStateMachine.AddState("Block");

        // 3. Setup Transitions
        
        // Locomotion -> Jump (Not Grounded & Y velocity > 0)
        var toJump = locomotionState.AddTransition(jumpState);
        toJump.AddCondition(AnimatorConditionMode.IfNot, 0, "IsGrounded");
        toJump.AddCondition(AnimatorConditionMode.Greater, 0.1f, "VerticalVelocity");

        // Locomotion -> Fall (Not Grounded & Y velocity <= 0)
        var toFall = locomotionState.AddTransition(fallState);
        toFall.AddCondition(AnimatorConditionMode.IfNot, 0, "IsGrounded");
        toFall.AddCondition(AnimatorConditionMode.Less, 0.1f, "VerticalVelocity");

        // Jump -> Fall
        var jumpToFall = jumpState.AddTransition(fallState);
        jumpToFall.AddCondition(AnimatorConditionMode.Less, 0.1f, "VerticalVelocity");

        // Jump/Fall -> Locomotion (Grounded)
        jumpState.AddTransition(locomotionState).AddCondition(AnimatorConditionMode.If, 0, "IsGrounded");
        fallState.AddTransition(locomotionState).AddCondition(AnimatorConditionMode.If, 0, "IsGrounded");

        // AnyState -> Attack
        var anyToAttack = rootStateMachine.AddAnyStateTransition(attackState);
        anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");

        // AnyState -> HeavyAttack
        var anyToHeavy = rootStateMachine.AddAnyStateTransition(heavyAttackState);
        anyToHeavy.AddCondition(AnimatorConditionMode.If, 0, "HeavyAttack");

        // Return from attacks
        attackState.AddTransition(locomotionState).hasExitTime = true;
        heavyAttackState.AddTransition(locomotionState).hasExitTime = true;

        // Block Transitions
        var toBlock = rootStateMachine.AddAnyStateTransition(blockState);
        toBlock.AddCondition(AnimatorConditionMode.If, 0, "IsBlocking");
        
        var fromBlock = blockState.AddTransition(locomotionState);
        fromBlock.AddCondition(AnimatorConditionMode.IfNot, 0, "IsBlocking");

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"<color=green>[AnimationSetupTool]</color> Animator Controller created successfully at: {ANIMATOR_PATH}");
    }

    private static void AddParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        // Check if exists
        foreach (var param in controller.parameters)
        {
            if (param.name == name) return;
        }
        controller.AddParameter(name, type);
    }
}
