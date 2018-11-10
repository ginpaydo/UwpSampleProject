using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;

namespace App2.BackgroundTasks
{
    /// <summary>
    /// 抽象クラス
    /// </summary>
    public abstract class BackgroundTask
    {
        /// <summary>
        /// 登録処理
        /// </summary>
        public abstract void Register();

        /// <summary>
        /// 処理内容
        /// </summary>
        /// <param name="taskInstance">対象タスク</param>
        /// <returns></returns>
        public abstract Task RunAsyncInternal(IBackgroundTaskInstance taskInstance);

        // 全タスク共通で存在するイベント処理は、ここで抽象メソッドを定義する
        /// <summary>
        /// キャンセル時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        public abstract void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason);

        /// <summary>
        /// 型名チェック
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Match(string name)
        {
            return name == GetType().Name;
        }

        /// <summary>
        /// 開始する
        /// </summary>
        /// <param name="taskInstance">開始対象のタスク</param>
        /// <returns></returns>
        public Task RunAsync(IBackgroundTaskInstance taskInstance)
        {
            // 各種イベントを登録
            SubscribeToEvents(taskInstance);

            // 実行
            return RunAsyncInternal(taskInstance);
        }

        /// <summary>
        /// 各種イベントを登録
        /// </summary>
        /// <param name="taskInstance"></param>
        public void SubscribeToEvents(IBackgroundTaskInstance taskInstance)
        {
            // タスクがキャンセルされたときの処理を登録
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
        }
    }
}
