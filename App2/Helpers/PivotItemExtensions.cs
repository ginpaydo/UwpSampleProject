using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App2.Helpers
{
    /// <summary>
    /// ピボット画面の拡張
    /// </summary>
    public static class PivotItemExtensions
    {
        /// <summary>
        /// フレーム内のページを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pivotItem"></param>
        /// <returns></returns>
        public static T GetPage<T>(this PivotItem pivotItem)
        {
            if (pivotItem.Content is Frame frame)
            {
                if (frame.Content is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.DataContext is T element)
                    {
                        return element;
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// 指定したページが表示されているか判定する
        /// </summary>
        /// <param name="pivotItem"></param>
        /// <param name="pageToken"></param>
        /// <returns></returns>
        public static bool IsOfPageType(this PivotItem pivotItem, string pageToken)
        {
            if (pivotItem.Content is Frame frame)
            {
                if (frame.Content.GetType().Name == $"{pageToken}Page")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
