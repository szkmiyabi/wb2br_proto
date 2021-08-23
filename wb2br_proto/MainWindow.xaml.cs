using System;
using System.Collections.Generic;
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
