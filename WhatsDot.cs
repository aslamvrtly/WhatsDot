using Microsoft.Web.WebView2.WinForms;
using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WhatsDotLib
{
    public class WhatsDot
    {
        private string baseURL = $"https://web.whatsapp.com";
        private WebView2 mainWebView;
        private static Timer loadTimer;
        private static Timer checkLogoutTimer;
        private static Timer scanTimer;
        private static Timer diconnectTimer;
        public bool isLoggedIn = false;
        private bool isRedirected = false;
        private bool isScanned = false;
        private bool isBarcode = false;
        private bool isCheck = true;
        private string status = "waiting";
        private string oldHead = "";
        private WebView2 loaderWebView;
        private int barcodeHeight = 0;
        private int barcodeWidth = 0;


        private string loader = @"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <title>Loader</title>
                            <style>
                                body {
                                    margin: 0;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    height: 100vh;
                                    background: #fff;
                                }

                                svg {
                                    width: 100px;
                                    height: 100px;
                        }
                            </style>
                        </head>
                        <body>
                            <svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 200'>
                                <circle fill='#000000' stroke='#000000' stroke-width='6' r='15' cx='40' cy='65'>
                                    <animate attributeName='cy' calcMode='spline' dur='2' values='65;135;65;' keySplines='.5 0 .5 1;.5 0 .5 1' repeatCount='indefinite' begin='-.4'></animate>
                                </circle>
                                <circle fill='#000000' stroke='#000000' stroke-width='6' r='15' cx='100' cy='65'>
                                    <animate attributeName='cy' calcMode='spline' dur='2' values='65;135;65;' keySplines='.5 0 .5 1;.5 0 .5 1' repeatCount='indefinite' begin='-.2'></animate>
                                </circle>
                                <circle fill='#000000' stroke='#000000' stroke-width='6' r='15' cx='160' cy='65'>
                                    <animate attributeName='cy' calcMode='spline' dur='2' values='65;135;65;' keySplines='.5 0 .5 1;.5 0 .5 1' repeatCount='indefinite' begin='0'></animate>
                                </circle>
                            </svg>
                        </body>
                        </html>
                        ";

        public WhatsDot(WebView2 userWebView)
        {
            mainWebView = userWebView ?? throw new ArgumentNullException(nameof(userWebView));
            loadWhatsapp();
        }
        public async void loadWhatsapp()
        {
            mainWebView.Visible = false;
            var parent = mainWebView.Parent;
            loaderWebView = new WebView2();
            loaderWebView.Visible = false;
            loaderWebView.Width = mainWebView.Width;
            loaderWebView.Height = mainWebView.Height;
            loaderWebView.Location = mainWebView.Location;
            mainWebView.CoreWebView2.ContextMenuRequested += (s, e) =>
            {
                e.Handled = true;
            };
            mainWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            mainWebView.ZoomFactor = 1.0;

            mainWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;

            parent.Controls.Add(loaderWebView);
            await loaderWebView.EnsureCoreWebView2Async(null);
            loaderWebView.CoreWebView2.ContextMenuRequested += (s, e) =>
            {
                e.Handled = true;
            };
            loaderWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            loaderWebView.ZoomFactor = 1.0;

            loaderWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;

            loaderWebView.NavigateToString(loader);
            mainWebView.Source = new Uri(baseURL);
        }

        public async System.Threading.Tasks.Task<string> checkLogin()
        {

            if (mainWebView.CoreWebView2 != null && isCheck)
            {
                loaderWebView.Visible = true;

                string scriptScan = @"(function() {
                                            const divs = document.querySelectorAll('div');
                                            for (let div of divs) {
                                                if (div.textContent.includes('Loading your chats')) {
                                                    return true;
                                                }
                                            }
                                            return false;
                                        })();
                                    ";
                string loginScan = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptScan);
                bool isScan = bool.Parse(loginScan);

                if (isScan == true)
                {
                    status = "waiting";

                }

                string waStartup = @"(document.querySelector(""#wa_web_initial_startup"") !== null)";
                string waStartupStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(waStartup);
                bool startupStatus = bool.Parse(waStartupStatus);

                if (!startupStatus)
                {
                    string scriptLogin = @"(document.querySelector(""#link-device-phone-number-code-screen-instructions"") !== null)";
                    string loginStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptLogin);
                    isLoggedIn = !bool.Parse(loginStatus);

                    if (isLoggedIn == false)
                    {
                        string checkUse = @"(!document.querySelector('canvas') == false)";
                        string useStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(checkUse);

                        if (useStatus == "true")
                        {
                            string checkHead = @"document.querySelector('head').outerHTML";
                            oldHead = await mainWebView.CoreWebView2.ExecuteScriptAsync(checkHead);

                            using (Graphics g = mainWebView.CreateGraphics())
                            {
                                float dpi = g.DpiY;

                                float scalingFactor = dpi / 96f;

                                barcodeHeight = (int)(mainWebView.Height / scalingFactor) - 25;
                                barcodeWidth = (int)(mainWebView.Width / scalingFactor) - 25;
                            }

                          

                            string clickUse = @"
                        (function() {
                            document.querySelector('body').append(document.querySelector('canvas'));
                            document.querySelector('#app').remove();
                            document.querySelector('body').style.display = 'flex';
                            document.querySelector('body').style.alignItems = 'center';
                            document.querySelector('body').style.justifyContent = 'center';
                            document.querySelector('canvas').style.width = '" + Convert.ToString(barcodeWidth) + @"px';
                            document.querySelector('canvas').style.height = '" + Convert.ToString(barcodeHeight) + @"px';
                            document.querySelector('body').style.background = '#fff';
                        })();
                    ";

                            await mainWebView.CoreWebView2.ExecuteScriptAsync(clickUse);
                            isCheck = false;
                            isBarcode = true;
                            status = "disconnected";
                            loaderWebView.Visible = false;
                        }
                    }
                }

                actionControl();
            }
            return status;
        }


        private async void actionControl()
        {
            Console.WriteLine(isLoggedIn);
            if (isLoggedIn)
            {
                if (checkLogoutTimer == null)
                {
                    checkLogoutTimer = new Timer();
                    checkLogoutTimer.Interval = 1000;
                    checkLogoutTimer.Tick += (sender1, args1) => checkLogoutAllTime();
                }
                if (!checkLogoutTimer.Enabled)
                {
                    checkLogoutTimer.Start();
                }
                if (loadTimer != null)
                {
                    loadTimer.Stop();
                    loadTimer.Dispose();
                    loadTimer = null;
                }
            }
            else
            {
                if (checkLogoutTimer != null && checkLogoutTimer.Enabled)
                {
                    checkLogoutTimer.Stop();
                    checkLogoutTimer.Dispose();
                    checkLogoutTimer = null;
                }
            }
        }

        private async System.Threading.Tasks.Task checkLogoutAllTime()
        {
            if (mainWebView.CoreWebView2 != null)
            {
                string waStartup = @"(document.querySelector(""#wa_web_initial_startup"") !== null)";
                string waStartupStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(waStartup);
                bool startupStatus = bool.Parse(waStartupStatus);

                if (!startupStatus)
                {
                    string scriptLogin = @"(document.querySelector(""#link-device-phone-number-code-screen-instructions"") !== null)";
                    string loginStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptLogin);
                    isLoggedIn = !bool.Parse(loginStatus);
                    if (isLoggedIn)
                    {
                        status = "connected";
                        loaderWebView.Visible = false;
                        isCheck = false;
                        isBarcode = true;
                    }
                    else
                    {
                        status = "waiting";
                        isCheck = true;
                        isBarcode = false;
                    }

                    actionControl();
                }
            }
        }



        public void connectWhatsapp()
        {

            if (MessageBox.Show("Do you really want to connect?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                mainWebView.Visible = true;

                isRedirected = false;
                if (mainWebView.CoreWebView2 != null)
                {
                    scanTimer = new Timer();
                    scanTimer.Interval = 1000;
                    scanTimer.Tick += (sender1, args1) => checkNewScan();
                    scanTimer.Start();
                }
            }
        }

        private async System.Threading.Tasks.Task checkNewScan()
        {
            string checkUse = @"Array.from(document.querySelectorAll('div')).some(div => div.textContent.trim() === 'Use here');";
            string useStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(checkUse);

            if (useStatus == "true")
            {
                string clickUse = @"Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Use here').closest(""button"").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(clickUse);
            }


            string scriptScan0 = @"document.querySelector('head').outerHTML;";
            string currentHead = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptScan0);

            if (oldHead == currentHead)
            {
                isScanned = false;
            }
            else
            {
                isScanned = true;
            }

            if (isScanned == true && isRedirected == false)
            {
                string url = $"https://web.whatsapp.com";
                await mainWebView.EnsureCoreWebView2Async(null);
                mainWebView.CoreWebView2.Navigate(url);
                status = "waiting";
                mainWebView.Visible = false;
                loaderWebView.Visible = true;
                isRedirected = true;
            }



            string scriptScan = @"(function() {
                                            const divs = document.querySelectorAll('div');
                                            for (let div of divs) {
                                                if (div.textContent.includes('Loading your chats')) {
                                                    return true;
                                                }
                                            }
                                            return false;
                                        })();
                                    ";
            string loginScan = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptScan);
            string scriptLogin = @"(document.querySelector(""[aria-label = Settings]"") !== null)";
            string loginStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptLogin);
            bool isScan = bool.Parse(loginStatus);
            bool isScan1 = bool.Parse(loginScan);
            if (isScan1 == true)
            {
                mainWebView.Visible = false;
                loaderWebView.Visible = true;
                status = "waiting";
            }

            if (isScan == true)
            {
                scanTimer.Stop();
                scanTimer.Dispose();
                scanTimer = null;
                mainWebView.Visible = false;
                loaderWebView.Visible = false;
                isLoggedIn = true;
                status = "connected";
                actionControl();
            }
        }



        public void logoutWhatsapp()
        {
            if (MessageBox.Show("Do you really want to disconnect?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                loaderWebView.Visible = true;
                diconnectTimer = new Timer();
                diconnectTimer.Interval = 1000;
                diconnectTimer.Tick += (sender1, args1) => checkLogout();
                diconnectTimer.Start();
            }
        }

        private async System.Threading.Tasks.Task checkLogout()
        {
            string scriptLogin = @"(document.querySelector(""[aria-label = Settings]"") !== null)";
            string loginStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptLogin);
            bool isLogout = bool.Parse(loginStatus);
            if (isLogout == true)
            {
                string openSettings = @"document.querySelector(""[aria-label = Settings]"").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(openSettings);
                string clickLO = @"Array.from(document.querySelectorAll('span')).find(span => span.textContent.trim() === 'Log out').closest(""button"").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(clickLO);
                string clickLogout = @"Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Log out').closest(""button"").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(clickLogout);
            }
            else
            {
                string scriptScan = @"(function() {
                                            const divs = document.querySelectorAll('div');
                                            for (let div of divs) {
                                                if (div.textContent.includes('Loading your chats')) {
                                                    return true;
                                                }
                                            }
                                            return false;
                                        })();
                                    ";
                string loginScan = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptScan);
                bool isScan = bool.Parse(loginScan);

                if (isScan == true)
                {
                    String clickLogout = @"Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Log out').querySelector(""button"").click();";
                    await mainWebView.CoreWebView2.ExecuteScriptAsync(clickLogout);
                    status = "waiting";
                }
                else
                {
                    string waStartup = @"(document.querySelector(""#wa_web_initial_startup"") !== null)";
                    string waStartupStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(waStartup);
                    bool startupStatus = bool.Parse(waStartupStatus);

                    if (!startupStatus)
                    {
                        string scriptLogin1 = @"(document.querySelector(""#link-device-phone-number-code-screen-instructions"") !== null)";
                        string loginStatus1 = await mainWebView.CoreWebView2.ExecuteScriptAsync(scriptLogin1);
                        isLoggedIn = !bool.Parse(loginStatus1);
                        if (isLoggedIn == false)
                        {
                            status = "disconnected";
                            if (diconnectTimer != null && diconnectTimer.Enabled)
                            {
                                diconnectTimer.Stop();
                                diconnectTimer.Dispose();
                                diconnectTimer = null;
                            }

                            if (checkLogoutTimer != null && checkLogoutTimer.Enabled)
                            {
                                checkLogoutTimer.Stop();
                                checkLogoutTimer.Dispose();
                                checkLogoutTimer = null;
                            }
                            loaderWebView.Visible = true;

                            isCheck = true;
                            isBarcode = false;
                            status = "waiting";
                        }
                        else
                        {
                            status = "connected";
                        }
                    }
                }
            }


        }
        public async void sendMessage(string phoneNum, string message)
        {
            if (mainWebView.CoreWebView2 != null)
            {

                string url = $"https://web.whatsapp.com/send/?phone={phoneNum}&text={message}&type=phone_number&app_absent=0";

                await mainWebView.EnsureCoreWebView2Async(null);
                mainWebView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                mainWebView.CoreWebView2.Navigate(url);

                Timer varTimer = new Timer { Interval = 1000 };
                varTimer.Tick += async (sender2, e2) =>
                {
                    string varScript = @"(function() {
                                        if (typeof isSendClicked === 'undefined') {
                                            isSendClicked = false;
                                        }

                                        if (typeof isPending === 'undefined') {
                                            isPending = true;
                                        }

                                        if (typeof isSendClicked !== 'undefined' && typeof isPending !== 'undefined') {
                                            return true;
                                        } else {
                                            return false;
                                        }
                                    })();";
                    string varStatus = await mainWebView.CoreWebView2.ExecuteScriptAsync(varScript);
                    if (varStatus == "true")
                    {
                        varTimer.Stop();
                    }
                };

                varTimer.Start();


                Timer sendTimer = new Timer { Interval = 1000 };
                sendTimer.Tick += async (sender2, e2) =>
                {


                    string script = @"if(document.querySelector(""[aria-label = Settings]"") !== null){
                                        if(document.querySelector(""[aria-label ='Starting chat']"") === null){
                                        (function() {

                                            if(isSendClicked && isPending == false){
                                                return true;
                                            }

                                            var pending = document.querySelector(""span[aria-label=' Pending ']"");
                                            if(pending){
                                                isPending = true;console.log('false1');
                                                return false;
                                            }else{
                                                if(isSendClicked){
                                                    isPending = false;console.log('false2');
                                                    return false;
                                                }
                                                var sendButton = document.querySelector('button[aria-label=""Send""]');
                                                if (sendButton) {
                                                    sendButton.click();
                                                    isSendClicked=true;console.log('false3');
                                                    return false;
                                                }
                                            }
                                            return false;
                                        })();}}";

                    string status = await mainWebView.CoreWebView2.ExecuteScriptAsync(script);
                    if (status == "true")
                    {
                        sendTimer.Stop();

                    }
                };

                sendTimer.Start();



            }

        }

    }
}
