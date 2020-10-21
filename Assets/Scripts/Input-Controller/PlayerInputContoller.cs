// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input-Controller/PlayerInputContoller.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputContoller : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputContoller()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputContoller"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""49e75ab7-2c93-4485-9ce7-5e809ba271b6"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""bf32e5fb-2b4f-4f82-b5f9-5d981f5a6a2d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""15ddcbba-78ca-43b6-9200-956fc2ae9c57"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SingleShot"",
                    ""type"": ""Button"",
                    ""id"": ""b7a208e1-02f6-4130-b369-c1b4e96b54f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BurstShot"",
                    ""type"": ""Button"",
                    ""id"": ""c15e0580-1116-42cc-b5f8-be750efa5dc0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ContinuousShot"",
                    ""type"": ""Button"",
                    ""id"": ""5d6af001-b9b5-4492-a7bb-2820fec28651"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5186560a-1a17-4743-b160-36759bbf5f06"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Move"",
                    ""id"": ""7233ab68-c3fd-44c4-bde5-c541ebd7326b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""441daf43-7127-4c7b-b7bd-e9fdfada9382"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bcf29bc9-e0c9-41cd-aaed-d9163d161174"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""178f2198-be2e-4f4b-9f5f-7b16ca4d6d8c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6fdb0c83-88d5-4cf2-8f6d-8d32dae10b69"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5f8ffcac-95a9-49f5-82b1-317807e4c7a4"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SingleShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1bfd5161-a1ab-47b1-8409-b526b8e4e634"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SingleShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a028d82f-b390-4bd1-8678-28afdab23a0d"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BurstShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""287bc26b-a766-430f-a93e-9ba36836c51d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold(duration=1.5)"",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ContinuousShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_SingleShot = m_Player.FindAction("SingleShot", throwIfNotFound: true);
        m_Player_BurstShot = m_Player.FindAction("BurstShot", throwIfNotFound: true);
        m_Player_ContinuousShot = m_Player.FindAction("ContinuousShot", throwIfNotFound: true);
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
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_SingleShot;
    private readonly InputAction m_Player_BurstShot;
    private readonly InputAction m_Player_ContinuousShot;
    public struct PlayerActions
    {
        private @PlayerInputContoller m_Wrapper;
        public PlayerActions(@PlayerInputContoller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @SingleShot => m_Wrapper.m_Player_SingleShot;
        public InputAction @BurstShot => m_Wrapper.m_Player_BurstShot;
        public InputAction @ContinuousShot => m_Wrapper.m_Player_ContinuousShot;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @SingleShot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSingleShot;
                @SingleShot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSingleShot;
                @SingleShot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSingleShot;
                @BurstShot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBurstShot;
                @BurstShot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBurstShot;
                @BurstShot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBurstShot;
                @ContinuousShot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnContinuousShot;
                @ContinuousShot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnContinuousShot;
                @ContinuousShot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnContinuousShot;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @SingleShot.started += instance.OnSingleShot;
                @SingleShot.performed += instance.OnSingleShot;
                @SingleShot.canceled += instance.OnSingleShot;
                @BurstShot.started += instance.OnBurstShot;
                @BurstShot.performed += instance.OnBurstShot;
                @BurstShot.canceled += instance.OnBurstShot;
                @ContinuousShot.started += instance.OnContinuousShot;
                @ContinuousShot.performed += instance.OnContinuousShot;
                @ContinuousShot.canceled += instance.OnContinuousShot;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSingleShot(InputAction.CallbackContext context);
        void OnBurstShot(InputAction.CallbackContext context);
        void OnContinuousShot(InputAction.CallbackContext context);
    }
}
