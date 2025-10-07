using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing.Text;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WhatsDotLib
{


    public class WhatsDot
    {
        private string baseURL = "https://web.whatsapp.com";

        private WebView2 mainWebView;

        private static Timer loadTimer;

        private static Timer checkLogoutTimer;

        private static Timer scanTimer;

        private static Timer diconnectTimer;

        public bool isLoggedIn;

        private bool isRedirected;

        private bool isScanned;

        private bool isBarcode;

        private bool isCheck = true;

        private string status = "waiting";

        private string oldHead = "";

        private WebView2 loaderWebView;

        private int barcodeHeight;

        private int barcodeWidth;

        private string loader = "\r\n                        <!DOCTYPE html>\r\n                        <html>\r\n                        <head>\r\n                            <meta charset='UTF-8'>\r\n                            <title>Loader</title>\r\n                            <style>\r\n                                body {\r\n                                    margin: 0;\r\n                                    display: flex;\r\n                                    justify-content: center;\r\n                                    align-items: center;\r\n                                    height: 100vh;\r\n                                    background: #fff;\r\n                                }\r\n\r\n                                svg {\r\n                                    width: 100px;\r\n                                    height: 100px;\r\n                        }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 200'>\r\n                                <circle fill='#000000' stroke='#000000' stroke-width='6' r='15' cx='40' cy='65'>\r\n                                    <animate attributeName='cy' calcMode='spline' dur='2' values='65;135;65;' keySplines='.5 0 .5 1;.5 0 .5 1' repeatCount='indefinite' begin='-.4'></animate>\r\n                                </circle>\r\n                                <circle fill='#000000' stroke='#000000' stroke-width='6' r='15' cx='100' cy='65'>\r\n                                    <animate attributeName='cy' calcMode='spline' dur='2' values='65;135;65;' keySplines='.5 0 .5 1;.5 0 .5 1' repeatCount='indefinite' begin='-.2'></animate>\r\n                                </circle>\r\n                                <circle fill='#000000' stroke='#000000' stroke-width='6' r='15' cx='160' cy='65'>\r\n                                    <animate attributeName='cy' calcMode='spline' dur='2' values='65;135;65;' keySplines='.5 0 .5 1;.5 0 .5 1' repeatCount='indefinite' begin='0'></animate>\r\n                                </circle>\r\n                            </svg>\r\n                        </body>\r\n                        </html>\r\n\r\n                        ";

        public WhatsDot(WebView2 userWebView)
        {
            mainWebView = userWebView ?? throw new ArgumentNullException("userWebView");
            loadWhatsapp();
        }

        public async void loadWhatsapp()
        {
            mainWebView.Visible = false;
            Control parent = mainWebView.Parent;
            loaderWebView = new WebView2();
            loaderWebView.Visible = false;
            loaderWebView.Width = mainWebView.Width;
            loaderWebView.Height = mainWebView.Height;
            loaderWebView.Location = mainWebView.Location;
            mainWebView.CoreWebView2.ContextMenuRequested += delegate (object s, CoreWebView2ContextMenuRequestedEventArgs e)
            {
                e.Handled = true;
            };
            mainWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            mainWebView.ZoomFactor = 1.0;
            mainWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            parent.Controls.Add(loaderWebView);
            await loaderWebView.EnsureCoreWebView2Async(null);
            loaderWebView.CoreWebView2.ContextMenuRequested += delegate (object s, CoreWebView2ContextMenuRequestedEventArgs e)
            {
                e.Handled = true;
            };
            loaderWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            loaderWebView.ZoomFactor = 1.0;
            loaderWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            loaderWebView.NavigateToString(loader);
            mainWebView.Source = new Uri(baseURL);
        }

        public async Task<string> checkLogin(bool onlyCheck = false)
        {

            bool loaderVisibility = true;
            if (onlyCheck)
            {
                loaderVisibility = false;
            }

            if (mainWebView.CoreWebView2 != null)
            {

                string javaScriptSpanOK = "Array.from(document.querySelectorAll('button')).some(span => span.textContent.trim() === 'OK');";
                if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptSpanOK) == "true")
                {
                    status = "waiting";
                    string javaScriptSpanOKBtn = "Array.from(document.querySelectorAll('button')).find(span => span.textContent.trim() === 'OK').click();";
                    await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptSpanOKBtn);
                }

                string javaScriptUseHere = "Array.from(document.querySelectorAll('div')).some(div => div.textContent.trim() === 'Use here');";
                if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptUseHere) == "true")
                {
                    status = "waiting";
                    string javaScriptUseHereBtn = "Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Use here').closest(\"button\").click();";
                    await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptUseHereBtn);
                }

                string javaScriptSpanUseHere = "Array.from(document.querySelectorAll('span')).some(span => span.textContent.trim() === 'Use here');";
                if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptSpanUseHere) == "true")
                {
                    status = "waiting";
                    string javaScriptSpanUseHereBtn = "Array.from(document.querySelectorAll('span')).find(span => span.textContent.trim() === 'Use here').closest(\"div\").click();";
                    await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptSpanUseHereBtn);
                }

                if (isCheck)
                {

                    loaderWebView.Visible = loaderVisibility;
                    string javaScript = "(function() {\r\n                                            const divs = document.querySelectorAll('div');\r\n                                            for (let div of divs) {\r\n                                                if (div.textContent.includes('Loading your chats')) {\r\n                                                    return true;\r\n                                                }\r\n                                            }\r\n                                            return false;\r\n                                        })();\r\n                                    ";
                    if (bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript)))
                    {
                        status = "waiting";
                    }

                    string javaScript2 = "(document.querySelector(\"#wa_web_initial_startup\") !== null)";
                    if (!bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript2)))
                    {

                        string javaScript3 = "(function() {\r\n                                            const divs = document.querySelectorAll('div');\r\n                                            for (let div of divs) {\r\n                                                if (div.textContent.includes('Scan the QR code to confirm') || div.textContent.includes('Scan QR code to confirm') || div.textContent.includes('Scan the QR') || div.textContent.includes('Scan QR')) {\r\n                                                    return true;\r\n                                                }\r\n                                            }\r\n                                            return false;\r\n                                        })();\r\n                                    ";
                        isLoggedIn = !bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript3));
                        if (!isLoggedIn)
                        {
                            string javaScript4 = "(document.querySelector('canvas') !== null)";
                            if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript4) == "true")
                            {
                                string javaScript5 = "document.querySelector('head').outerHTML";
                                oldHead = await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript5);
                                using (Graphics graphics = mainWebView.CreateGraphics())
                                {
                                    float num = graphics.DpiY / 96f;
                                    barcodeHeight = (int)((float)mainWebView.Height / num) - 25;
                                    barcodeWidth = (int)((float)mainWebView.Width / num) - 25;
                                }

                                string javaScript6 = "\r\n                        (function() {\r\n                            document.querySelector('body').append(document.querySelector('canvas'));\r\n                            document.querySelector('#app').remove();\r\n                            document.querySelector('body').style.display = 'flex';\r\n                            document.querySelector('body').style.alignItems = 'center';\r\n                            document.querySelector('body').style.justifyContent = 'center';\r\n                            document.querySelector('canvas').style.width = '" + Convert.ToString(barcodeWidth) + "px';\r\n                            document.querySelector('canvas').style.height = '" + Convert.ToString(barcodeHeight) + "px';\r\n                            document.querySelector('body').style.background = '#fff';\r\n                        })();\r\n                    ";
                                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript6);
                                isCheck = false;
                                isBarcode = true;
                                status = "disconnected";
                                loaderWebView.Visible = loaderVisibility;
                            }
                        }
                    }

                    actionControl();
                }
            }

            return status;
        }

        private async void actionControl()
        {
            if (isLoggedIn)
            {
                if (checkLogoutTimer == null)
                {
                    checkLogoutTimer = new Timer();
                    checkLogoutTimer.Interval = 1000;
                    checkLogoutTimer.Tick += delegate
                    {
                        checkLogoutAllTime();
                    };
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
            else if (checkLogoutTimer != null && checkLogoutTimer.Enabled)
            {
                checkLogoutTimer.Stop();
                checkLogoutTimer.Dispose();
                checkLogoutTimer = null;
            }
        }

        private async Task checkLogoutAllTime()
        {
            if (mainWebView.CoreWebView2 == null)
            {
                return;
            }

            string javaScript = "(document.querySelector(\"#wa_web_initial_startup\") !== null)";
            if (!bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript)))
            {
                string javaScript2 = "(function() {\r\n                                            const divs = document.querySelectorAll('div');\r\n                                            for (let div of divs) {\r\n                                                if (div.textContent.includes('Scan the QR code to confirm') || div.textContent.includes('Scan QR code to confirm') || div.textContent.includes('Scan the QR') || div.textContent.includes('Scan QR')) {\r\n                                                    return true;\r\n                                                }\r\n                                            }\r\n                                            return false;\r\n                                        })();\r\n                                    ";
                isLoggedIn = !bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript2));
                if (isLoggedIn)
                {
                    status = "connected";
                    string javaScript3 = "\r\n                            (function() {\r\n                                const btn = Array.from(document.querySelectorAll('button'))\r\n                                    .find(div => div.textContent.trim() === 'Continue');\r\n                                if (btn) btn.click();\r\n                            })();\r\n                        ";
                    await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript3);
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

        public void connectWhatsapp()
        {
            if (MessageBox.Show("Do you really want to connect?", "Message", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            mainWebView.Visible = true;
            isRedirected = false;
            if (mainWebView.CoreWebView2 != null)
            {
                scanTimer = new Timer();
                scanTimer.Interval = 1000;
                scanTimer.Tick += delegate
                {
                    checkNewScan();
                };
                scanTimer.Start();
            }
        }

        private async Task checkNewScan()
        {
            string javaScript = "Array.from(document.querySelectorAll('div')).some(div => div.textContent.trim() === 'Use here');";
            if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript) == "true")
            {
                string javaScript2 = "Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Use here').closest(\"button\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript2);
            }

            string javaScriptSpanUseHere = "Array.from(document.querySelectorAll('span')).some(span => span.textContent.trim() === 'Use here');";
            if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptSpanUseHere) == "true")
            {
                string javaScriptSpanUseHereBtn = "Array.from(document.querySelectorAll('span')).find(span => span.textContent.trim() === 'Use here').closest(\"div\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptSpanUseHereBtn);
            }

            string javaScript3 = "document.querySelector('head').outerHTML;";
            string text = await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript3);
            if (oldHead == text)
            {
                isScanned = false;
            }
            else
            {
                isScanned = true;
            }

            if (isScanned && !isRedirected)
            {
                string url = "https://web.whatsapp.com";
                await mainWebView.EnsureCoreWebView2Async(null);
                mainWebView.CoreWebView2.Navigate(url);
                status = "waiting";
                mainWebView.Visible = false;
                loaderWebView.Visible = true;
                isRedirected = true;
            }

            string javaScript4 = "(function() {\r\n                                            const divs = document.querySelectorAll('div');\r\n                                            for (let div of divs) {\r\n                                                if (div.textContent.includes('Loading your chats')) {\r\n                                                    return true;\r\n                                                }\r\n                                            }\r\n                                            return false;\r\n                                        })();\r\n                                    ";
            string loginScan = await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript4);
            string javaScript5 = "(document.querySelector(\"[aria-label = Settings]\") !== null)";
            bool num = bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript5));
            if (bool.Parse(loginScan))
            {
                mainWebView.Visible = false;
                loaderWebView.Visible = true;
                status = "waiting";
            }

            if (num)
            {
                scanTimer.Stop();
                scanTimer.Dispose();
                scanTimer = null;
                string javaScript6 = "\r\n                    (function() {\r\n                        const btn = Array.from(document.querySelectorAll('button'))\r\n                            .find(div => div.textContent.trim() === 'Continue');\r\n                        if (btn) btn.click();\r\n                    })();\r\n                ";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript6);
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
                diconnectTimer.Tick += delegate
                {
                    checkLogout();
                };
                diconnectTimer.Start();
            }
        }

        private async Task checkLogout()
        {
            string javaScript = "(document.querySelector(\"[aria-label = Settings]\") !== null)";
            if (bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript)))
            {
                string javaScript2 = "document.querySelector(\"[aria-label = Settings]\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript2);
                string javaScript3 = "Array.from(document.querySelectorAll('span')).find(span => span.textContent.trim() === 'Log out').closest(\"button\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript3);
                string javaScript4 = "Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Log out').closest(\"button\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript4);
                string javaScriptdiv = "Array.from(document.querySelectorAll('span')).find(span => span.textContent.trim() === 'Log out').closest(\"div\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptdiv);
                string javaScriptdiv1 = "Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Log out').closest(\"div\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScriptdiv1);
                return;
            }

            string javaScript5 = "(function() {\r\n                                            const divs = document.querySelectorAll('div');\r\n                                            for (let div of divs) {\r\n                                                if (div.textContent.includes('Loading your chats')) {\r\n                                                    return true;\r\n                                                }\r\n                                            }\r\n                                            return false;\r\n                                        })();\r\n                                    ";
            if (bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript5)))
            {
                string javaScript6 = "Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Log out').querySelector(\"button\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript6);
                string javaScript6div = "Array.from(document.querySelectorAll('div')).find(div => div.textContent.trim() === 'Log out').querySelector(\"div\").click();";
                await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript6div);
                status = "waiting";
                return;
            }

            string javaScript7 = "(document.querySelector(\"#wa_web_initial_startup\") !== null)";
            if (bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript7)))
            {
                return;
            }

            string javaScript8 = "(function() {\r\n                                            const divs = document.querySelectorAll('div');\r\n                                            for (let div of divs) {\r\n                                                if (div.textContent.includes('Scan the QR code to confirm') || div.textContent.includes('Scan QR code to confirm') || div.textContent.includes('Scan the QR') || div.textContent.includes('Scan QR')) {\r\n                                                    return true;\r\n                                                }\r\n                                            }\r\n                                            return false;\r\n                                        })();\r\n                                    ";
            isLoggedIn = !bool.Parse(await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript8));
            if (!isLoggedIn)
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

        public async Task<bool> sendMessage(string phoneNum, string message)
        {
            if (mainWebView.CoreWebView2 != null)
            {
                string url = "https://web.whatsapp.com/send/?phone=" + phoneNum + "&text=" + message + "&type=phone_number&app_absent=0";
                await mainWebView.EnsureCoreWebView2Async(null);
                mainWebView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                mainWebView.CoreWebView2.Navigate(url);
                Timer varTimer = new Timer
                {
                    Interval = 1000
                };
                varTimer.Tick += async delegate
                {
                    string javaScript2 = "(function() {\r\n                                        if (typeof isSendClicked === 'undefined') {\r\n                                            isSendClicked = false;\r\n                                        }\r\n\r\n                                        if (typeof isPending === 'undefined') {\r\n                                            isPending = true;\r\n                                        }\r\n\r\n                                        if (typeof isSendClicked !== 'undefined' && typeof isPending !== 'undefined') {\r\n                                            return true;\r\n                                        } else {\r\n                                            return false;\r\n                                        }\r\n                                    })();";
                    if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript2) == "true")
                    {
                        varTimer.Stop();
                    }
                };
                varTimer.Start();
                TaskCompletionSource<bool> isTaskCompleted = new TaskCompletionSource<bool>();
                Timer sendTimer = new Timer
                {
                    Interval = 1000
                };
                sendTimer.Tick += async delegate
                {
                    string javaScript = "if(document.querySelector(\"[aria-label = Settings]\") !== null){\r\n                                        if(document.querySelector(\"[aria-label ='Starting chat']\") === null){\r\n                                        (function() {\r\n\r\n                                            if(isSendClicked && isPending == false){\r\n                                                return true;\r\n                                            }\r\n\r\n                                            var pending = document.querySelector(\"span[aria-label=' Pending ']\");\r\n                                            if(pending){\r\n                                                isPending = true;console.log('false1');\r\n                                                return false;\r\n                                            }else{\r\n                                                if(isSendClicked){\r\n                                                    isPending = false;console.log('false2');\r\n                                                    return false;\r\n                                                }\r\n                                                var sendButton = document.querySelector('button[aria-label=\"Send\"]');\r\n                                                if (sendButton) {\r\n                                                    sendButton.click();\r\n                                                    isSendClicked=true;console.log('false3');\r\n                                                    return false;\r\n                                                }else{var sendButtonDiv = document.querySelector('div[aria-label=\"Send\"]');\r\n                                                if (sendButtonDiv) {\r\n                                                    sendButtonDiv.click();\r\n                                                    isSendClicked=true;console.log('false3');\r\n                                                    return false;\r\n                                                }\r\n}\r\n                                            }\r\n                                            return false;\r\n                                        })();}}";
                    if (await mainWebView.CoreWebView2.ExecuteScriptAsync(javaScript) == "true")
                    {
                        sendTimer.Stop();
                        isTaskCompleted.SetResult(result: true);
                    }
                };
                sendTimer.Start();
                return await isTaskCompleted.Task;
            }

            return false;
        }
    }

}