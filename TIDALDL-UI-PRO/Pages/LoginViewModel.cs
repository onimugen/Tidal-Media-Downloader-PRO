using AIGS.Common;
using AIGS.Helper;
using HandyControl.Controls;
using Stylet;
using System;
using System.Threading.Tasks;
using TIDALDL_UI.Else;
using TidalLib;

namespace TIDALDL_UI.Pages
{
    public class LoginViewModel : ModelBase
    {
        public bool BtnLoginEnable { get; set; } = true;
        public bool HaveInit { get; set; } = false;
        public TidalDeviceCode DeviceCode { get; set; }
        public string AccessTokenInput { get; set; } = null;
        public UserSettings Settings { get; set; } = UserSettings.Read();
        private IWindowManager Manager;
        private MainViewModel VMMain;

        public LoginViewModel(IWindowManager manager, MainViewModel vmmain)
        {
            Manager = manager;
            VMMain  = vmmain;
            VMMain.VMLogin = this;
            return;
        }

        protected override async void OnViewLoaded()
        {
            if (HaveInit)
                return;
            HaveInit = true;

            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls13;

            BtnLoginEnable = false;

            //Proxy
            HttpHelper.ProxyInfo PROXY = Settings.ProxyEnable ? new HttpHelper.ProxyInfo(Settings.ProxyHost, Settings.ProxyPort, Settings.ProxyUser, Settings.ProxyPwd) : null;

            //Auto login by accessToken
            string msg;
            LoginKey key;
            if (Settings.Accesstoken.IsNotBlank())
            {
                (msg, key) = await Client.Login(Settings.Accesstoken, PROXY);
                if (msg.IsBlank())
                    goto LOGIN_SUCCESS;
                if (Settings.Refreshtoken.IsNotBlank())
                {
                    (msg, key) = await Client.RefreshAccessToken(Settings.Refreshtoken, PROXY);
                    if (msg.IsBlank())
                    {
                        Settings.Userid = key.UserID;
                        Settings.Accesstoken = key.AccessToken;
                        Settings.Save();
                        Global.AccessKey = key;
                        goto LOGIN_SUCCESS;
                    }
                }
            }

            //apply user-saved keys (override Gist if set)
            Client.SetApiKey(Settings.ClientId, Settings.ClientSecret);

            //refresh API key from Gist (only if user hasn't set custom keys)
            if (Settings.ClientId.IsBlank())
                await Client.RefreshApiKey(PROXY);

            //get device code
            (string msg1, TidalDeviceCode code) = await Client.GetDeviceCode(PROXY);
            if (msg1.IsNotBlank() || code == null)
                Growl.Error(Language.Get("strmsgGetDeviceCodeFailed") + ": " + msg1, Global.TOKEN_LOGIN);
            else
                DeviceCode = code;
            goto RETURN_POINT;

        LOGIN_SUCCESS:
            Global.AccessKey = key;
            Global.CommonKey = key;
            Global.VideoKey = key;
            Manager.ShowWindow(VMMain);
            RequestClose();

        RETURN_POINT:
            BtnLoginEnable = true;
            return;
        }

        public async void Login()
        {
            BtnLoginEnable = false;

            //Proxy
            HttpHelper.ProxyInfo PROXY = Settings.ProxyEnable ? new HttpHelper.ProxyInfo(Settings.ProxyHost, Settings.ProxyPort, Settings.ProxyUser, Settings.ProxyPwd) : null;

            if (DeviceCode == null)
            {
                //get device code
                (string msg1, TidalDeviceCode code) = await Client.GetDeviceCode(PROXY);
                if (msg1.IsNotBlank() || code == null)
                {
                    Growl.Error(msg1.IsNotBlank() ? msg1 : Language.Get("strmsgGetDeviceCodeFailed"), Global.TOKEN_LOGIN);
                    BtnLoginEnable = true;
                    return;
                }
                DeviceCode = code;
            }

            ThreadHelper.Start(CheckAuthThreadFunc);
            return;
        }

        public async void LoginWithToken()
        {
            if (AccessTokenInput.IsBlank())
            {
                Growl.Error("Please paste an access token.", Global.TOKEN_LOGIN);
                return;
            }

            BtnLoginEnable = false;

            //Proxy
            HttpHelper.ProxyInfo PROXY = Settings.ProxyEnable ? new HttpHelper.ProxyInfo(Settings.ProxyHost, Settings.ProxyPort, Settings.ProxyUser, Settings.ProxyPwd) : null;

            (string msg, LoginKey key) = await Client.Login(AccessTokenInput.Trim(), PROXY);
            if (msg.IsNotBlank() || key == null)
            {
                Growl.Error("Token login failed: " + msg, Global.TOKEN_LOGIN);
                BtnLoginEnable = true;
                return;
            }

            Settings.Userid = key.UserID;
            Settings.Countrycode = key.CountryCode;
            Settings.Accesstoken = AccessTokenInput.Trim();
            Settings.Save();
            Global.AccessKey = key;
            Global.CommonKey = key;
            Global.VideoKey = key;

            Manager.ShowWindow(VMMain);
            RequestClose();
        }

        public void SaveProxy()
        {
            Settings.Save();
        }

        public void SaveKeys()
        {
            Settings.Save();
            Client.SetApiKey(Settings.ClientId, Settings.ClientSecret);
        }

        public async void Login2()
        {
            BtnLoginEnable = false;

            if (Settings.Username.IsBlank() || Settings.Password.IsBlank())
            {
                Growl.Error(Language.Get("strmsgUsenamePasswordErr"), Global.TOKEN_LOGIN);
                goto RETURN_POINT;
            }

            //Proxy
            HttpHelper.ProxyInfo PROXY = Settings.ProxyEnable ? new HttpHelper.ProxyInfo(Settings.ProxyHost, Settings.ProxyPort, Settings.ProxyUser, Settings.ProxyPwd) : null;

            //token
            (string token1, string token2) = await GetToken();

            //Login (lossless key \ video key)
            (string msg, LoginKey key)   = await Client.Login(Settings.Username, Settings.Password, token1, PROXY);
            (string msg3, LoginKey key3) = await Client.Login(Settings.Username, Settings.Password, token2, PROXY);
            if (msg.IsNotBlank() || key == null)
            {
                Growl.Error(Language.Get("strmsgLoginErr") + msg, Global.TOKEN_LOGIN);
                goto RETURN_POINT;
            }

            //Auto get accesstoken(master key)
            string printSuccess = null;
            string printWarning = null;
            (string msg2, LoginKey key2) = Client.GetAccessTokenFromTidalDesktop(key.UserID);
            if (key2 != null && msg2.IsBlank() && key2.AccessToken != Settings.Accesstoken)
            {
                (msg2, key2) = await Client.Login(key2.AccessToken, PROXY);
                if (msg2.IsBlank() && key2 != null)
                {
                    printSuccess = "Auto get accesstoken success!";
                    Settings.Accesstoken = key2.AccessToken;
                }
            }
            else
                key2 = null;

            if (key2 == null && Settings.Accesstoken.IsNotBlank())
            {
                (msg2, key2) = await Client.Login(Settings.Accesstoken, PROXY);
                if (msg2.IsNotBlank() || key2 == null)
                    printWarning = "Accesstoken is not valid! " + msg;
            }
            
            if (!Settings.Remember)
                Settings.Password = null;
            Settings.Userid      = key.UserID;
            Settings.Sessionid1  = key.SessionID;
            Settings.Accesstoken = Settings.Accesstoken;
            Settings.Save();
            Global.CommonKey = key;
            Global.VideoKey = key3;
            Global.AccessKey = key2;

            Manager.ShowWindow(VMMain);
            if (printSuccess.IsNotBlank())
                Growl.Success(printSuccess, Global.TOKEN_MAIN);
            else if (printWarning.IsNotBlank())
                Growl.Warning(printWarning, Global.TOKEN_MAIN);

            RequestClose();

        RETURN_POINT:
            BtnLoginEnable = true;
            return;
        }

        public void WindowMove()
        {
            ((LoginView)this.View).DragMove();
        }

        public void WindowClose()
        {
            ThreadTool.Close();
            RequestClose();
        }


        public async Task<(string, string)> GetToken()
        {
            try
            {
                HttpHelper.Result result = await HttpHelper.GetOrPostAsync("https://cdn.jsdelivr.net/gh/yaronzz/CDN@latest/app/tidal/token.json");
                if (result.sData.IsNotBlank())
                {
                    string token = JsonHelper.GetValue(result.sData, "token");
                    string token2 = JsonHelper.GetValue(result.sData, "token2");
                    return (token, token2);
                }
            }
            catch { }
            return Client.GetDefaultToken();
        }

        public void CheckAuthThreadFunc(object[] datas)
        {
            try
            {
                if (DeviceCode == null)
                {
                    this.View.Dispatcher.Invoke(() => Growl.Error(Language.Get("strmsgGetDeviceCodeFailed"), Global.TOKEN_LOGIN));
                    goto RETURN_POINT;
                }

                NetHelper.OpenWeb("https://" + DeviceCode.VerificationUri + "/" + DeviceCode.UserCode);

                //Proxy
                HttpHelper.ProxyInfo PROXY = Settings.ProxyEnable ? new HttpHelper.ProxyInfo(Settings.ProxyHost, Settings.ProxyPort, Settings.ProxyUser, Settings.ProxyPwd) : null;

                (string msg, LoginKey key) = Client.CheckAuthStatus(DeviceCode, PROXY).Result;
                if (msg.IsNotBlank())
                {
                    this.View.Dispatcher.Invoke(() => Growl.Error(msg, Global.TOKEN_LOGIN));
                    goto RETURN_POINT;
                }

                Settings.Userid = key.UserID;
                Settings.Countrycode = key.CountryCode;
                Settings.Accesstoken = key.AccessToken;
                Settings.Refreshtoken = key.RefreshToken;
                Settings.Save();
                Global.AccessKey = key;
                Global.CommonKey = key;
                Global.VideoKey = key;

                this.View.Dispatcher.Invoke(() => {
                    Manager.ShowWindow(VMMain);
                    RequestClose();
                });
                return;
            }
            catch (Exception ex)
            {
                this.View.Dispatcher.Invoke(() => Growl.Error("Login error: " + ex.Message, Global.TOKEN_LOGIN));
            }

        RETURN_POINT:
            this.View.Dispatcher.Invoke(() => { BtnLoginEnable = true; });
        }

    }



}
