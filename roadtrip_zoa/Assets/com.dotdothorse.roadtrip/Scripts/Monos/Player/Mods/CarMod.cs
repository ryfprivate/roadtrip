using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace com.dotdothorse.roadtrip
{
    public abstract class CarMod : MonoBehaviour
    {
        public abstract void InitializeMod(CarBody carBody);
        public abstract void ReleaseMod();
        public abstract void EnableMod();
        public abstract void DisableMod();
        public abstract void ReadKeyboardInput();
        public abstract void ReadTouchInput(Touch touch);
    }
}