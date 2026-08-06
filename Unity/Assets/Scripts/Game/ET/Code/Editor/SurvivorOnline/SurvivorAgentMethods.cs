using System;
using System.Threading;
using AgentBridge;
using Cysharp.Threading.Tasks;
using ET;
using ET.Client;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    public static class SurvivorAgentMethods
    {
        private const string BattleSceneName = "SurvivorBattle";

        [AgentCallable("通过现有 UI 自动登录、加入唯一测试房间并开始 Survivor 战斗。调用前需先进入 Play Mode。", 150)]
        private static async UniTask EnterSurvivorBattle()
        {
            EnsurePlaying("启动自动进入战斗");
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(150));
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            string token = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
            string account = $"AI{token}";
            string password = "AI_TEST_PASSWORD";
            string roomCode = $"AI{token}";

            Debug.Log($"[SurvivorAgent] UI automation started account={account} room={roomCode}");

            MonoUIFormSurvivorLogin login = null;
            await UniTask.WaitUntil(() => (login = FindActiveView<MonoUIFormSurvivorLogin>()) != null, cancellationToken: cancellationToken, cancelImmediately: true);
            await UniTask.NextFrame();
            EnsurePlaying("准备操作登录界面");
            EnsureBindings(login.AccountInputField, login.PasswordInputField, login.LoginButton, "登录界面");
            login.AccountInputField.text = account;
            login.PasswordInputField.text = password;
            login.LoginButton.onClick.Invoke();

            MonoUIFormSurvivorLobby lobby = null;
            await UniTask.WaitUntil(() => (lobby = FindActiveView<MonoUIFormSurvivorLobby>()) != null, cancellationToken: cancellationToken, cancelImmediately: true);
            await UniTask.NextFrame();
            EnsurePlaying("准备操作房间界面");
            EnsureBindings(lobby.RoomCodeInputField, lobby.JoinButton, lobby.StartButton, "房间界面");
            lobby.RoomCodeInputField.text = roomCode;
            lobby.JoinButton.onClick.Invoke();

            await UniTask.WaitUntil(
                () => lobby != null
                    && lobby.StartButton != null
                    && lobby.StartButton.gameObject.activeInHierarchy
                    && lobby.StartButton.interactable,
                cancellationToken: cancellationToken,
                cancelImmediately: true);
            lobby.StartButton.onClick.Invoke();

            await UniTask.WaitUntil(
                () => SceneManager.GetSceneByName(BattleSceneName).isLoaded
                    && FindActiveView<MonoUIFormSurvivorHud>() != null,
                cancellationToken: cancellationToken,
                cancelImmediately: true);

            MonoUIFormSurvivorSkillChoice skillChoice = null;
            await UniTask.WaitUntil(
                () => (skillChoice = FindActiveView<MonoUIFormSurvivorSkillChoice>()) != null,
                cancellationToken: cancellationToken,
                cancelImmediately: true);
            await UniTask.NextFrame();
            EnsurePlaying("准备选择初始技能");
            EnsureBindings(
                skillChoice.Choice1Button,
                skillChoice.Choice2Button,
                skillChoice.Choice3Button,
                "技能选择界面");

            UIFormSurvivorSkillChoiceComponent skillChoiceForm =
                    skillChoice.UGFUIForm as UIFormSurvivorSkillChoiceComponent;
            if (skillChoiceForm == null)
            {
                throw new CommandException(
                    "SURVIVOR_SKILL_FORM_MISSING",
                    "初始技能界面的 ET 组件尚未完成绑定");
            }

            await UniTask.WaitUntil(() => IsInitialSkillChoiceReady(skillChoiceForm, skillChoice), cancellationToken: cancellationToken, cancelImmediately: true);

            SurvivorClientComponent client = skillChoiceForm.Client;
            SurvivorPlayerState playerState = client.LocalPlayerState();
            long choiceRevision = playerState.SkillChoiceRevision;
            skillChoice.Choice1Button.onClick.Invoke();

            await UniTask.WaitUntil(() => IsInitialSkillChoiceAccepted(client, choiceRevision), cancellationToken: cancellationToken, cancelImmediately: true);
            await UniTask.WaitUntil(() => IsSkillChoiceClosed(client, skillChoice), cancellationToken: cancellationToken, cancelImmediately: true);

            Debug.Log($"[SurvivorAgent] Battle ready account={account} room={roomCode}");
        }

        private static bool IsInitialSkillChoiceReady(
            UIFormSurvivorSkillChoiceComponent skillChoiceForm,
            MonoUIFormSurvivorSkillChoice skillChoice)
        {
            SurvivorClientComponent client = skillChoiceForm.Client;
            EnsureBattleRunning(client, "等待初始技能可选择");
            SurvivorPlayerState state = client.LocalPlayerState();
            return state != null
                    && state.UnspentSkillPoints > 0
                    && state.SkillChoiceRevision > 0
                    && state.SkillChoice1 != SurvivorSkillType.None
                    && skillChoice != null
                    && skillChoice.Choice1Button.interactable;
        }

        private static bool IsInitialSkillChoiceAccepted(
            SurvivorClientComponent client,
            long choiceRevision)
        {
            EnsureBattleRunning(client, "等待服务器确认初始技能");
            SurvivorPlayerState state = client.LocalPlayerState();
            return state != null && state.SkillChoiceRevision != choiceRevision;
        }

        private static bool IsSkillChoiceClosed(
            SurvivorClientComponent client,
            MonoUIFormSurvivorSkillChoice skillChoice)
        {
            EnsureBattleRunning(client, "等待技能选择界面关闭");
            return skillChoice == null || !skillChoice.gameObject.activeInHierarchy;
        }

        private static void EnsureBattleRunning(SurvivorClientComponent client, string stage)
        {
            SurvivorWorldComponent world = client == null ? null : client.World;
            if (world?.Data?.Phase == SurvivorRoomPhase.Ended)
            {
                throw new CommandException(
                    "SURVIVOR_BATTLE_ENDED_EARLY",
                    $"{stage}失败：战斗已提前结束");
            }
        }

        private static T FindActiveView<T>() where T : AETMonoUGFUIForm
        {
            T[] views = UnityEngine.Object.FindObjectsByType<T>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (T view in views)
            {
                if (view != null
                    && view.gameObject.scene.IsValid()
                    && view.gameObject.activeInHierarchy
                    && view.isActiveAndEnabled
                    && view.UGFUIForm != null)
                {
                    return view;
                }
            }

            return null;
        }

        private static void EnsureBindings(UnityEngine.Object first, UnityEngine.Object second,
            UnityEngine.Object third, string viewName)
        {
            if (first == null || second == null || third == null)
            {
                throw new CommandException("SURVIVOR_UI_BINDING_MISSING", $"{viewName}存在未绑定的自动化控件");
            }
        }

        private static void EnsurePlaying(string stage)
        {
            if (!EditorApplication.isPlaying)
            {
                throw new CommandException("SURVIVOR_NOT_PLAYING", $"{stage}失败：Unity 当前不在 Play Mode");
            }
        }

    }
}
