using UnityEngine;
using UnityEngine.InputSystem;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorHudComponent))]
    [ETReactiveSystem]
    public static partial class UIFormSurvivorHudComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIFormSurvivorHudComponent self)
        {
            self.Client = self.Root().GetComponent<SurvivorClientComponent>();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(this UIFormSurvivorHudComponent self, float elapseSeconds, float realElapseSeconds)
        {
            Vector2 movement = ReadMovement();
            self.Client.UpdateLocalInput((int)(movement.x * SurvivorDefaults.InputScale), (int)(movement.y * SurvivorDefaults.InputScale), realElapseSeconds);
            self.ObserveChanges();
        }

        private static Vector2 ReadMovement()
        {
            Vector2 movement = Vector2.zero;
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    movement.x -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    movement.x += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    movement.y -= 1f;
                }

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    movement.y += 1f;
                }
            }

            if (movement != Vector2.zero)
            {
                return movement;
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad == null)
            {
                return Vector2.zero;
            }

            Vector2 stickMovement = gamepad.leftStick.ReadValue();
            Vector2 dpadMovement = gamepad.dpad.ReadValue();
            return dpadMovement.sqrMagnitude > stickMovement.sqrMagnitude ? dpadMovement : stickMovement;
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorHudComponent self, bool isShutdown)
        {
            self.ResetReactive();
        }

        [ETReactiveBind(nameof(UIFormSurvivorHudComponent.Level))]
        private static void OnLevelChanged(this UIFormSurvivorHudComponent self, int level)
        {
            self.View.LevelText.text = $"LV  {level}";
        }

        [ETReactiveBind(nameof(UIFormSurvivorHudComponent.RoomCode))]
        private static void OnRoomCodeChanged(this UIFormSurvivorHudComponent self, string roomCode)
        {
            self.View.RoomText.text = $"ROOM  {roomCode}";
        }

        [ETReactiveBind(nameof(UIFormSurvivorHudComponent.ServerTick))]
        private static void OnServerTickChanged(this UIFormSurvivorHudComponent self, long serverTick)
        {
            self.View.TickText.text = $"TIME  {serverTick / SurvivorDefaults.SimulationTicksPerSecond}s";
        }

        [ETReactiveBind(nameof(UIFormSurvivorHudComponent.Phase))]
        private static void OnPhaseChanged(this UIFormSurvivorHudComponent self, SurvivorRoomPhase phase)
        {
            self.View.PhaseText.text = $"STATE  {phase.ToString().ToUpperInvariant()}";
        }

        [ETReactiveBind(nameof(UIFormSurvivorHudComponent.Hp), nameof(UIFormSurvivorHudComponent.MaxHp))]
        private static void OnHealthChanged(this UIFormSurvivorHudComponent self, int hp, int maxHp)
        {
            self.View.HpText.text = $"HP  {hp}/{maxHp}";
        }
    }
}
