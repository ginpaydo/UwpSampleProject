using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using App2.BackgroundTasks;
using App2.Helpers;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;

// アウトプロセスの場合、プロジェクトのマニフェストに宣言を追加しないと、バックグラウンドタスクは利用できない（例外が出る）
// これだと、"App2.BackgroundTasks.BackgroundTask1"みたいな感じで指定？
// インプロセスだと不要

namespace App2.Services
{
    /// <summary>
    /// App.xamlで登録
    /// </summary>
    internal class BackgroundTaskService : IBackgroundTaskService
    {
        /// <summary>
        /// 存在するバックグラウンドタスク全てのリスト
        /// </summary>
        public static IEnumerable<BackgroundTask> BackgroundTasks => BackgroundTaskInstances.Value;

        /// <summary>
        /// 遅延初期化でバックグラウンドタスクのリストを列挙し、そのインスタンスを全て作成
        /// </summary>
        private static readonly Lazy<IEnumerable<BackgroundTask>> BackgroundTaskInstances =
            new Lazy<IEnumerable<BackgroundTask>>(() => CreateInstances());

        /// <summary>
        /// バックグラウンド タスクを登録する
        /// タスク名とエントリポイント、更新間隔を指定して登録（これは、各バックグラウンドクラスのRegisterメソッドで指定）
        /// </summary>
        /// <returns></returns>
        public async Task RegisterBackgroundTasksAsync()
        {
            // アプリケーションが強制終了された場合に更新されたアプリケーションを起動できない問題の対応
            // ロック画面のアプリケーション リストから呼び出し元アプリケーションを削除します。
            BackgroundExecutionManager.RemoveAccess();

            // タスク登録前に必要
            var result = await BackgroundExecutionManager.RequestAccessAsync();

            // システムやユーザに拒否されていたら登録しない
            if (result == BackgroundAccessStatus.DeniedBySystemPolicy
                || result == BackgroundAccessStatus.DeniedByUser)
            {
                return;
            }

            // ここで管理している全てのバックグラウンド タスクを登録する
            foreach (var task in BackgroundTasks)
            {
                task.Register();
            }
        }

        /// <summary>
        /// 指定したバックグラウンドタスクを開始する
        /// </summary>
        /// <param name="taskInstance">開始するタスクのインスタンス</param>
        public void Start(IBackgroundTaskInstance taskInstance)
        {
            //
            var task = BackgroundTasks.FirstOrDefault(b => b.Match(taskInstance?.Task?.Name));

            if (task == null)
            {
                // この状態にはならないはず。
                // あるバックグラウンドタスクを始めるために必要なものが、このサービスで管理するバックグラウンドタスクに無かったことを意味する。
                // その時はCreateInstancesメソッドに、必要なバックグラウンドタスクを追加し忘れてないか確認すること。
                return;
            }

            // 非同期メソッドをawaitしないので、FireAndForgetを使う
            task.RunAsync(taskInstance).FireAndForget();
        }

        /// <summary>
        /// BackgroundTaskのインスタンスを全て作成
        /// TODO:BackgroundTaskを新しく作成したら、ここに追加すること
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<BackgroundTask> CreateInstances()
        {
            var backgroundTasks = new List<BackgroundTask>
            {
                new BackgroundTask1()
            };
            return backgroundTasks;
        }
    }
}
