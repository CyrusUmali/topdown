//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/NPC/NpcInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @NpcInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @NpcInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""NpcInputActions"",
    ""maps"": [
        {
            ""name"": ""Npc"",
            ""id"": ""2b72ec5e-f478-46d8-a25a-60ede65814da"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""ae9c64e1-9c3a-4222-b480-dd834d253e2a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7504cafe-fa67-41a6-90b4-a5715851fc86"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Npc
        m_Npc = asset.FindActionMap("Npc", throwIfNotFound: true);
        m_Npc_Move = m_Npc.FindAction("Move", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Npc
    private readonly InputActionMap m_Npc;
    private List<INpcActions> m_NpcActionsCallbackInterfaces = new List<INpcActions>();
    private readonly InputAction m_Npc_Move;
    public struct NpcActions
    {
        private @NpcInputActions m_Wrapper;
        public NpcActions(@NpcInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Npc_Move;
        public InputActionMap Get() { return m_Wrapper.m_Npc; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NpcActions set) { return set.Get(); }
        public void AddCallbacks(INpcActions instance)
        {
            if (instance == null || m_Wrapper.m_NpcActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_NpcActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
        }

        private void UnregisterCallbacks(INpcActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
        }

        public void RemoveCallbacks(INpcActions instance)
        {
            if (m_Wrapper.m_NpcActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(INpcActions instance)
        {
            foreach (var item in m_Wrapper.m_NpcActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_NpcActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public NpcActions @Npc => new NpcActions(this);
    public interface INpcActions
    {
        void OnMove(InputAction.CallbackContext context);
    }
}