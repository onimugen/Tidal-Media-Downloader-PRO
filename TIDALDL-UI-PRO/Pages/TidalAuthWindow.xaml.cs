using System;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace TIDALDL_UI.Pages
{
    public partial class TidalAuthWindow : Window
    {
        public string CapturedToken { get; private set; }

        public TidalAuthWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => await InitWebView();
        }

        private async System.Threading.Tasks.Task InitWebView()
        {
            try
            {
                await WebView.EnsureCoreWebView2Async();

                // Inject script into every page before it runs — intercepts fetch + XHR Authorization headers
                await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
(function() {
    function tryCapture(auth) {
        if (auth && auth.indexOf('Bearer ') === 0) {
            var token = auth.substring(7).trim();
            if (token.length > 20) {
                try { window.chrome.webview.postMessage(token); } catch(e) {}
            }
        }
    }

    // Intercept fetch
    var origFetch = window.fetch;
    window.fetch = function(input, init) {
        if (init && init.headers) {
            var h = init.headers;
            if (typeof h.get === 'function') {
                tryCapture(h.get('Authorization') || h.get('authorization'));
            } else {
                tryCapture(h['Authorization'] || h['authorization']);
            }
        }
        return origFetch.apply(this, arguments);
    };

    // Intercept XHR
    var origSetHeader = XMLHttpRequest.prototype.setRequestHeader;
    XMLHttpRequest.prototype.setRequestHeader = function(name, value) {
        if (name.toLowerCase() === 'authorization') tryCapture(value);
        return origSetHeader.apply(this, arguments);
    };
})();
");

                // Receive the token posted from JS
                WebView.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    string token = e.TryGetWebMessageAsString();
                    if (!string.IsNullOrWhiteSpace(token) && token.Length > 20 && CapturedToken == null)
                    {
                        CapturedToken = token;
                        Dispatcher.Invoke(() => DialogResult = true);
                    }
                };

                WebView.CoreWebView2.Navigate("https://listen.tidal.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2 error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
        }
    }
}
