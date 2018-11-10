using System;
using System.Globalization;
using System.Threading.Tasks;

using App2.Services;

using Microsoft.Practices.Unity;

using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace App2
{
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンテナのコンフィグ
        /// 作成したサービスなどを登録する
        /// </summary>
        protected override void ConfigureContainer()
        {
            // register a singleton using Container.RegisterType<IInterface, Type>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
            // バックグラウンドタスクサービスの登録
            // ContainerControlledLifetimeManagerを渡すと、シングルトン管理されるぞ
            // 渡さない場合は、[Depandency]でインジェクションするたびにインスタンス生成される。
            Container.RegisterType<IBackgroundTaskService, BackgroundTaskService>(new ContainerControlledLifetimeManager());
            // Typeと名前をキー情報にして、Typeにキャスト可能なインスタンスを登録する。以下のように名前を指定しない場合、構成ファイルで指定したものが注入される
            // 名前を指定した場合は[Depandency("名前")]で注入できる
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterType<IWebViewService, WebViewService>();
            Container.RegisterType<ISampleDataService, SampleDataService>();

            // RegisterInstanceは動的な関連付け、外部で作成したインスタンスをコンテナに渡す。
            // RegisterTypeは静的な関連付け、コンテナ内でインスタンスを作成する（ための情報）を定義。
        }

        /// <summary>
        /// アプリケーション起動時
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            // トップページの作成
            await LaunchApplicationAsync(PageTokens.PivotPage, null);
        }

        /// <summary>
        /// アプリケーション起動時に呼ばれる
        /// 最初のページを表示する
        /// </summary>
        /// <param name="page">トップページ名</param>
        /// <param name="launchParam"></param>
        /// <returns></returns>
        private async Task LaunchApplicationAsync(string page, object launchParam)
        {
            // 呼ぶときに必要
            await ThemeSelectorService.SetRequestedThemeAsync();

            // 多分これでページ遷移する
            NavigationService.Navigate(page, launchParam);

            // ウインドウ有効化
            Window.Current.Activate();
        }

        protected override async Task OnActivateApplicationAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// バックグラウンドが有効化されたとき、そのタスクを全て開始する
        /// </summary>
        /// <param name="args"></param>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            CreateAndConfigureContainer();
            Container.Resolve<IBackgroundTaskService>().Start(args.TaskInstance);
        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await ThemeSelectorService.InitializeAsync().ConfigureAwait(false);

            // We are remapping the default ViewNamePage and ViewNamePageViewModel naming to ViewNamePage and ViewNameViewModel to
            // gain better code reuse with other frameworks and pages within Windows Template Studio
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "App2.ViewModels.{0}ViewModel, App2", viewType.Name.Substring(0, viewType.Name.Length - 4));
                return Type.GetType(viewModelTypeName);
            });
            await Container.Resolve<IBackgroundTaskService>().RegisterBackgroundTasksAsync();
            await base.OnInitializeAsync(args);
        }
    }
}
