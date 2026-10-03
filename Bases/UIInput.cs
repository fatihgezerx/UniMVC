#if HAS_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace UniMVC
{
    /// <summary>
    /// Which kind of device the player used last, for UI that behaves differently with a gamepad: a panel that
    /// selects a control for d-pad navigation, an inventory slot that picks an item up instead of being clicked.
    /// </summary>
    public static class UIInput
    {
        /// <summary>
        /// Whether the gamepad is the device used most recently: it was updated later than the keyboard and the mouse.
        /// Needs the Input System (<c>HAS_INPUT_SYSTEM</c>); without it this is always false.
        /// </summary>
        public static bool IsGamepadActive
        {
            get
            {
#if HAS_INPUT_SYSTEM
                var gamepad = Gamepad.current;
                if (gamepad == null)
                {
                    return false;
                }

                var keyboard = Keyboard.current;
                var mouse = Mouse.current;
                return gamepad.lastUpdateTime > (keyboard != null ? keyboard.lastUpdateTime : 0d)
                       && gamepad.lastUpdateTime > (mouse != null ? mouse.lastUpdateTime : 0d);
#else
                return false;
#endif
            }
        }
    }
}
