using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace App2.BackgroundTasks
{
    /// <summary>
    /// バックグラウンドの動作定義
    /// </summary>
    public sealed class BackgroundTask1 : BackgroundTask
    {
        /// <summary>
        /// 動作中かどうかのメッセージ
        /// これをどこかで表示すればよい
        /// </summary>
        public static string Message { get; set; }

        #region PrivateField
        /// <summary>
        /// キャンセルしたか
        /// </summary>
        private volatile bool _cancelRequested = false;

        /// <summary>
        /// 対象のタスクインスタンス
        /// </summary>
        private IBackgroundTaskInstance _taskInstance;

        /// <summary>
        /// 遅延
        /// </summary>
        private BackgroundTaskDeferral _deferral;
        #endregion

        /// <summary>
        /// 登録処理
        /// </summary>
        public override void Register()
        {
            // タスク名を指定する（この例ではクラス名をタスク名とする）
            var taskName = GetType().Name;

            if (!BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == taskName))
            {
                // 標準で用意されているビルダーを通じて登録を行う
                var builder = new BackgroundTaskBuilder()
                {
                    // 名前の指定
                    Name = taskName
                };

                // 必要ならエントリポイントもここで設定する（いらんやろ？）

                // TODO:バックグラウンド処理のトリガーを定義し、実行条件をセットする
                // 15分間隔で定期的に処理する指定
                // 15分未満だと、例外が発生するらしい？
                builder.SetTrigger(new TimeTrigger(15, false));
                // ユーザが存在するとき（ユーザが不在なら実行しない）
                // ここでは、ネットの接続状態、非従量課金かどうかなども指定できる
                builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

                // 登録
                builder.Register();
            }
        }

        /// <summary>
        /// 処理を行う
        /// </summary>
        /// <param name="taskInstance"></param>
        /// <returns></returns>
        public override Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                return null;
            }

            // 非同期コードを実行するためには遅延を要求する
            _deferral = taskInstance.GetDeferral();

            return Task.Run(() =>
            {
                //// TODO WTS: 実行されるべきバックグラウンドタスクをここに書く
                //// このサンプルはタイマを100で初期化し、10ごとにメッセージを更新する。

                // "BackgroundTaskService.GetBackgroundTasksRegistration"を呼ぶと、messageを表示できる

                _taskInstance = taskInstance;

                // 1秒ごとに実行するタイマ処理をセットする
                // ゼロを指定すると1度だけ実行
                ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(SampleTimerCallback), TimeSpan.FromSeconds(1));
            });
        }

        /// <summary>
        /// キャンセル時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;

           // TODO: キャンセル時の処理を書く
        }

        /// <summary>
        /// タイマコールバックの例
        /// このメソッドをタイマのハンドラに登録する
        /// </summary>
        /// <param name="timer"></param>
        private void SampleTimerCallback(ThreadPoolTimer timer)
        {
            // 進捗が100になるか、キャンセルするまで
            if ((_cancelRequested == false) && (_taskInstance.Progress < 100))
            {
                // 進捗に10加える、動作中メッセージを表示
                _taskInstance.Progress += 10;
                Message = $"Background Task {_taskInstance.Task.Name} running ({_taskInstance.Progress}/100)";
            }
            else
            {
                // 終わるか、キャンセルの場合

                // タイマ解除
                timer.Cancel();

                // 結果表示
                if (_cancelRequested)
                {
                    Message = $"Background Task {_taskInstance.Task.Name} cancelled";
                }
                else
                {
                    Message = $"Background Task {_taskInstance.Task.Name} finished";
                }

                // 非同期コードが完了したら、遅延を解放して完了通知する
                _deferral?.Complete();
            }
        }
    }
}
