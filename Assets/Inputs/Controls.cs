// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a8e4e2d6-d806-4cb3-9740-e00e5d48562f"",
            ""actions"": [
                {
                    ""name"": ""MoveGesture"",
                    ""type"": ""Value"",
                    ""id"": ""286d5757-9f33-4959-9e22-d2ba9ecafbfa"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""090cdc04-a8c7-4cd9-9fab-e247cc3baee0"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""MoveGesture"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""01693d71-3d20-4935-bffb-b918e51b0395"",
            ""actions"": [
                {
                    ""name"": ""Tap"",
                    ""type"": ""Value"",
                    ""id"": ""57307c76-5861-4d8a-9633-4b2c3b013414"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TouchPosition"",
                    ""type"": ""Value"",
                    ""id"": ""004d9e93-629c-43e4-adee-44c0295e0b38"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ba647767-d49b-496a-9295-db5d20e7e55f"",
                    ""path"": ""<Touchscreen>/touch0"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Tap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9e20c92b-2e9d-4044-aa0a-553f75b23817"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""TouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mobile"",
            ""bindingGroup"": ""Mobile"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MoveGesture = m_Player.FindAction("MoveGesture", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Tap = m_UI.FindAction("Tap", throwIfNotFound: true);
        m_UI_TouchPosition = m_UI.FindAction("TouchPosition", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MoveGesture;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveGesture => m_Wrapper.m_Player_MoveGesture;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MoveGesture.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveGesture;
                @MoveGesture.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveGesture;
                @MoveGesture.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveGesture;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveGesture.started += instance.OnMoveGesture;
                @MoveGesture.performed += instance.OnMoveGesture;
                @MoveGesture.canceled += instance.OnMoveGesture;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Tap;
    private readonly InputAction m_UI_TouchPosition;
    public struct UIActions
    {
        private @Controls m_Wrapper;
        public UIActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Tap => m_Wrapper.m_UI_Tap;
        public InputAction @TouchPosition => m_Wrapper.m_UI_TouchPosition;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Tap.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTap;
                @Tap.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTap;
                @Tap.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTap;
                @TouchPosition.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTouchPosition;
                @TouchPosition.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTouchPosition;
                @TouchPosition.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTouchPosition;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Tap.started += instance.OnTap;
                @Tap.performed += instance.OnTap;
                @Tap.canceled += instance.OnTap;
                @TouchPosition.started += instance.OnTouchPosition;
                @TouchPosition.performed += instance.OnTouchPosition;
                @TouchPosition.canceled += instance.OnTouchPosition;
            }
        }
    }
    public UIActions @UI => new UIActions(this);
    private int m_MobileSchemeIndex = -1;
    public InputControlScheme MobileScheme
    {
        get
        {
            if (m_MobileSchemeIndex == -1) m_MobileSchemeIndex = asset.FindControlSchemeIndex("Mobile");
            return asset.controlSchemes[m_MobileSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMoveGesture(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnTap(InputAction.CallbackContext context);
        void OnTouchPosition(InputAction.CallbackContext context);
    }
}
