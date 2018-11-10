using System;
using System.Threading.Tasks;

namespace App2.Helpers
{
    /// <summary>
    /// タスクの拡張
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// このメソッドは非同期メソッドをawaitしないで呼ぶことを許す。
        /// タスク完了を待たなくて良いときに使う。
        /// </summary>
        /// <param name="task"></param>
        public static void FireAndForget(this Task task)
        {
        }
    }
}
