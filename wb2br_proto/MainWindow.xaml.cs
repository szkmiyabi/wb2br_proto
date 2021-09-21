using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace wb2br_proto
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //メンバ
        bool _isNavigating = false;

        private ObservableCollection<UrlEntity> _urlEntities = new ObservableCollection<UrlEntity>();

        public static RoutedCommand LoadFile = new RoutedCommand();
        public static RoutedCommand UrlComboBoxChanged = new RoutedCommand();
        public static RoutedCommand UrlNext = new RoutedCommand();
        public static RoutedCommand UrlPrev = new RoutedCommand();
        public static RoutedCommand SimCssCut = new RoutedCommand();
        public static RoutedCommand SimImageAlt = new RoutedCommand();

        //コンストラクタ
        public MainWindow()
        {
            InitializeComponent();
            AttachControlEventHandlers(webView);
        }

        //初期ロード時イベント紐付け（ラッパー）
        void AttachControlEventHandlers(WebView2 control)
        {
            //ナビゲーション開始イベント
            control.NavigationStarting += WebView_NavigationStarting;
            //ナビゲーション完了イベント
            control.NavigationCompleted += WebView_NavigationCompleted;
            //control.CoreWebView2InitializationCompleted +=
            //control.KeyDown +=
        }

        //Nextボタンの実行可否
        void UrlNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0 && 
                urlComboBox.SelectedIndex < (_urlEntities.Count<UrlEntity>() - 1);
        }

        //Nextボタンの実行処理
        void UrlNextExecuted(object target, ExecutedRoutedEventArgs e)
        {
            int crIndex = urlComboBox.SelectedIndex;
            crIndex++;
            urlComboBox.SelectedIndex = crIndex;
        }

        //Prevボタンの実行可否
        void UrlPrevCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0 && urlComboBox.SelectedIndex > 0;
        }

        //Prevボタンの実行処理
        void UrlPrevExecuted(object target, ExecutedRoutedEventArgs e)
        {
            int crIndex = urlComboBox.SelectedIndex;
            crIndex--;
            urlComboBox.SelectedIndex = crIndex;
        }

        //Loadボタンの実行可否
        void LoadFileCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //Loadボタンの実行処理
        void LoadFileCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            _urlEntities = new ObservableCollection<UrlEntity>();
            FileRepository fr = new FileRepository();
            _urlEntities = fr.GetUrlEntities();
            urlComboBox.ItemsSource = _urlEntities;
            urlComboBox.SelectedIndex = 0;
        }

        //UrlComboBoxChangedアクションの実行可否
        void UrlComboBoxChangedCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0;
        }

        //UrlComboBoxChangedアクションの実行処理
        async void UrlComboBoxChangedExecute(object target, ExecutedRoutedEventArgs e)
        {
            //UrlComboBoxが初期化された場合はリターン
            if (urlComboBox.SelectedIndex == -1) return;

            var crUrl = _urlEntities[urlComboBox.SelectedIndex].pageUrl;
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Navigate(crUrl);
            url.Text = crUrl;
        }


        //全シミュレーションの実行可否
        void SimAllCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid() && !_isNavigating;
        }

        //CssCutシミュレーション
        async void SimCssCutExecute(object target, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.css_cut());
        }


        //ImageAltシミュレーション
        async void SimImageAltExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.image_alt());

        }

        //webViewインスタンスの有効判定
        bool IsWebViewValid()
        {
            try
            {
                return webView != null && webView.CoreWebView2 != null;
            }
            catch(Exception ex) when(ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                return false;
            }
        }

        //ナビゲーション開始イベント
        void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _isNavigating = true;
            RequeryCommands();
        }

        //ナビゲーション完了イベント
        void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _isNavigating = false;
            RequeryCommands();
        }

        //CanExecuteChangedを検知するメソッド
        void RequeryCommands()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        //ブラウザの基本コマンド
        void BackCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && webView.CanGoBack;
        }

        void BackCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.GoBack();
        }

        void ForwardCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && webView.CanGoForward;
        }

        void ForwardCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.GoForward();
        }

        void RefreshCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid() && !_isNavigating;
        }

        void RefreshCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.Reload();
        }

        void BrowseStopCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid() && _isNavigating;
        }

        void BrowseStopCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.Stop();
        }

        void GoToPageCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && !_isNavigating;
        }

        async void GoToPageCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();

            var rawUrl = (string)e.Parameter;
            Uri uri = null;

            if(Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute))
            {
                uri = new Uri(rawUrl);
            }
            else if(!rawUrl.Contains(" ") && rawUrl.Contains("."))
            {
                uri = new Uri("http://" + rawUrl);
            }
            else
            {
                uri = new Uri("https://google.co,jp/search?=hl=ja&q=" +
                    String.Join("&", Uri.EscapeDataString(rawUrl).Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
            }

            webView.CoreWebView2.Navigate(uri.ToString());
        }

    }
}
