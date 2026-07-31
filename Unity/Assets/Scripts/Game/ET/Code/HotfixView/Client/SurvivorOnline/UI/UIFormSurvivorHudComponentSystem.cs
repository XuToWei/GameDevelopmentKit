using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorHudComponent))]
    public static partial class UIFormSurvivorHudComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorHudComponent self)
        {
            self.Observer = new SurvivorHudReactiveObserver(
                self,
                self.Root().GetComponent<SurvivorClientComponent>(),
                new SurvivorHudReactionSink());
            self.Observer.ResetChanges();
            self.Root()
                    .GetComponent<SurvivorClientComponent>()
                    .RegisterPresentationObserver(self.Observer);
            self.Observer.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(
            this UIFormSurvivorHudComponent self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.Root().GetComponent<SurvivorClientComponent>().SendInput(
                (int)(Input.GetAxisRaw("Horizontal") * SurvivorDefaults.InputScale),
                (int)(Input.GetAxisRaw("Vertical") * SurvivorDefaults.InputScale));
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorHudComponent self, bool isShutdown)
        {
            self.Root()
                    .GetComponent<SurvivorClientComponent>()
                    .UnregisterPresentationObserver(self.Observer);
            self.Observer.ResetChanges();
            self.Observer = null;
        }
    }
}
