using System;
using System.Collections.Generic;
using FixMath.NET;
using FS.Math;
using FS.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FS.Manager
{
    public class FSPlayerInputManager:MonoBaseManager
    {
        private string ClassName = "";
        private GameManager _gameManager;
        private static FSPlayerInputManager _instance;
        public static FSPlayerInputManager Instance => _instance;
        public PlayerInputInfo[] CurrentFrameInput;

        private InputSystem_Actions _inputSystemActions;
        private int _currentFrameIndex = 0;
        public int InputActionCount => _currentFrameIndex;
        private int _maxFrameCount = 1000;
        
        private bool isMovePressed = false;
        private FVector2 _moveVector = FVector2.Zero;
        
        public override void DoAwake()
        {
            _inputSystemActions = new InputSystem_Actions();
            _instance = this;

            ClassName = GetType().FullName;
            Debug.Log($"[{ClassName}] Awake");
            CurrentFrameInput = new PlayerInputInfo[_maxFrameCount];
            for(int i = 0;i<_maxFrameCount;i++)
            {
                CurrentFrameInput[i] = new PlayerInputInfo();
            }
            _inputSystemActions.Player.Move.performed+=OnMove;
            _inputSystemActions.Player.Move.canceled += OnMoveCanceled;
            _inputSystemActions.Player.Look.performed += OnLook;    
            
            _inputSystemActions.Enable();
        }


        public override void DoStart()
        {
            _gameManager = GameManager.Instance;
        }

        #region Look


        private void OnLook(InputAction.CallbackContext obj)
        {
            if (obj.performed)
            {
                
                
                var lookVector = (FVector2)(obj.ReadValue<Vector2>());
                PushPlayInput(new PlayerInputInfo()
                {
                    mouseInput = lookVector,
                    keyboardInput = FVector2.Zero
                });
                
                 
            }
        }
        

        #endregion

        #region  Move

        private void ProcessMove()
        {
            if (isMovePressed)
            {
                PushPlayInput(new PlayerInputInfo()
                {
                    mouseInput = FVector2.Zero,
                    keyboardInput = _moveVector
                });
            }
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext obj)
        {
            if (obj.canceled)
            {
                isMovePressed = false;
                _moveVector = FVector2.Zero;
            }
        }


        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _moveVector =  (FVector2)(context.ReadValue<Vector2>());

                isMovePressed = true;

            }
        }

        

        #endregion
        
        public void _DoUpdate(Fix64 deltaTime)
        {
    
            
            ProcessMove();
        }


        public override void DoLateUpdate()
        {
            _currentFrameIndex = 0;
        }
 
        
        public void PushPlayInput(PlayerInputInfo playerInputInfo)
        {
            if (_currentFrameIndex >= _maxFrameCount)
            {
                return; // Reset to avoid overflow
            }
            int index =(int) _gameManager.FrameCount% _maxFrameCount;

            
            
            CurrentFrameInput[_currentFrameIndex].keyboardInput = playerInputInfo.keyboardInput;
            CurrentFrameInput[_currentFrameIndex].mouseInput = playerInputInfo.mouseInput;
            _currentFrameIndex++;
        }


        public PlayerInputInfo  GetCurrentFrameInfo(out int inputCount)
        {
            //inputCount = _currentFrameIndex;
            inputCount = 1;
            int index =(int) _gameManager.FrameCount% _maxFrameCount;
            //Debug.Log($"[{ClassName}] Current Logic Frame Input: {_currentFrameIndex}");
            return CurrentFrameInput[index];
        }
    }
}