using System;
using System.IO;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using static System.Text.Encoding;
using static Util.utils;


namespace Swap
{
    internal class Program
    {
        private static bool claimed = false;
        private static string target = null;
        private static WebProxy[] proxies = null;
        private static int attempts = 0;
        private static int method = 1;
        private static User my_account;
        

        public struct User
        {
            public string username;
            public string email;
            public string user_id;
            public string phone;
            public string session;
            public string token;

        }

        

        public static void Main()
        {
            Console.WriteLine(banner);
            login();
            if (!get_data()){Console.WriteLine("[!] Unable to gather account data!");Thread.Sleep(2000);return;}
            target = input("[+] Enter target user: ");
            string use_proxies = input("[+] Use HTTP Proxies? Y/N: ");
            if (use_proxies.ToLower() == "y"){load();}
            int thread_count = int.Parse(input("[+] Enter amount of threads: "));
            bool ready = init();
            while (!ready) { ready = init(); }
            Thread[] threads = new Thread[thread_count];
            for (int i = 0; i < thread_count; i++)
            {
                threads[i] = new Thread(() => Program.claim_user());
                threads[i].Start();
            }
            threads.ToList().ForEach(t => t.Join());
            if (!claimed) { Console.WriteLine($"[!] Spam blocked on user: {target}"); fail(target, attempts); input("[+] Press ENTER to exit....");return; }
            else { Console.WriteLine($"[+] Successfully Swapped >> @{target} | Attempts: {attempts} |  Method #{method}"); success(target,attempts); }
        }
        
        
        public static void web_claim()
        {
            
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://www.instagram.com/accounts/edit/?__d=dis");
            Uri targe = new Uri("https://www.instagram.com/accounts/edit/?__d=dis");
            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.Add(new Cookie("sessionid", $"{my_account.session}") { Domain = targe.Host });
            webRequest.Method = "POST";

            if (proxies != null) { webRequest.Proxy = proxies.ElementAt(new Random().Next(proxies.Length - 1)); };
            if (claimed) { return; }
            byte[] data = ASCII.GetBytes($"first_name=&email={my_account.email}&username={target}&phone_number={my_account.phone}&biography=sWaPPed+by+sLiMe*&external_url=discord.gg%2Funbans&chaining_enabled=on");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = data.Length;
            webRequest.Headers.Add("x-csrftoken", "JTgWH0GhGYX6pqKQkpkjEIvoRFEjkUIc");
            webRequest.Headers["Cookie"] = $"sessionid={my_account.session}";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.41 Safari/537.36 Edg/101.0.1210.32";
            webRequest.Timeout = 5000;
            using (Stream requestStream = webRequest.GetRequestStream()) { requestStream.Write(data, 0, data.Length); Interlocked.Increment(ref attempts); }
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                string content = null;
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Default))
                    {
                        content = streamReader.ReadToEnd();
                        if (content.Contains("{\"status\":\"ok\"}"))
                        {
                            claimed = true;
                        }
                    }
                }

            }
        }

        public static void api_claim()
        {

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://i.instagram.com/api/v1/accounts/edit_profile/");
            Uri targe = new Uri("https://i.instagram.com/api/v1/accounts/edit_profile/");
            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.Add(new Cookie("sessionid", $"{my_account.session}") { Domain = targe.Host });
            webRequest.Method = "POST";

            if (proxies != null) { webRequest.Proxy = proxies.ElementAt(new Random().Next(proxies.Length - 1)); };
            if (claimed) { return; }
            byte[] data = ASCII.GetBytes($"first_name=&email={my_account.email}&username={target}&phone_number={my_account.phone}&biography=sWaPPed+by+sLiMe*&external_url=discord.gg%2Funbans&_uuid={Guid.NewGuid()}&device_id=android-JDS99162");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = data.Length;
            webRequest.Headers["Cookie"] = $"sessionid={my_account.session}";
            webRequest.UserAgent = "Instagram 85.0.0.21.100 Android (28/9; 380dpi; 1080x2147; OnePlus; HWEVA; OnePlus6T; qcom; en_US; 146536611)";
            webRequest.Timeout = 5000;
            using (Stream requestStream = webRequest.GetRequestStream()) { requestStream.Write(data, 0, data.Length); Interlocked.Increment(ref attempts); }
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                string content = null;
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Default))
                    {
                        int code = (int)webResponse.StatusCode;
                        content = streamReader.ReadToEnd();
                        if (content.Contains("{\"status\":\"ok\"}") || code==200)
                        {
                            claimed = true;
                        }
                    }
                }

            }
        }


        public static void claim_user()
        {
            while (!claimed) // Try mobile api claim first
            {
               
                try {api_claim(); print(); }
                catch (WebException we)
                {
                    int Code = 0;
                    var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                    try{Code = (int)((HttpWebResponse)we.Response).StatusCode;}
                    catch(Exception){Code = 400;}
                    if (Code != 400 && (Code==429 || resp.ToLower().Contains("try again later"))){ break; }
                    if (resp.ToLower().Contains("challenge_required")) { break; }
                    
                }
                catch (Exception ex){Console.WriteLine(ex.Message);}
            }

            while (!claimed) // Try web api claim if rate-limited on mobile
            {
                method = 2;
                try { web_claim(); print(); }
                catch (WebException we)
                {
                    int Code = 0;
                    var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                    try { Code = (int)((HttpWebResponse)we.Response).StatusCode; }
                    catch (Exception) { Code = 400; }
                    if (Code != 400 && (Code == 429 || resp.Contains("edit_username_threshold_reached"))) { break; }
                    if (resp.ToLower().Contains("challenge_required")) { break; }

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }

     
        }
        
        public static void login()
        {
            while (true)
            { // Keep looping until you login successfully
                string username = input("[+] Enter username: ");
                string password = mask("[+] Enter password: ");
                using (HttpClient httpClient = new HttpClient())
                {

                    
                    httpClient.DefaultRequestHeaders.Add("user-agent", "Instagram 85.0.0.21.100 Android (28/9; 380dpi; 1080x2147; OnePlus; HWEVA; OnePlus6T; qcom; en_US; 146536611)");

                    using (StringContent content = new StringContent($"guid={Guid.NewGuid()}&username={username}&password={password}&device_id=android-JDS99162&login_attempt_count=0"))
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        using (HttpResponseMessage message = httpClient.PostAsync("https://i.instagram.com/api/v1/accounts/login/", content).Result)
                        {
                            string response = message.Content.ReadAsStringAsync().Result;
                            if (message.IsSuccessStatusCode)
                            {
                                foreach (string value in message.Headers.GetValues("Set-Cookie"))
                                {
                                    if (value.Contains("sessionid"))
                                    {
                                        string session_id = value.Split(new string[] { "sessionid=" }, StringSplitOptions.None)[1].Split(';')[0];
                                        Console.WriteLine($"[SUCCESS] Logged into user >> {username} ");
                                        my_account.session=session_id;


                                    }
                                    else if (value.Contains("token"))
                                    {
                                        string token = value.Split(new string[] { "csrftoken=" }, StringSplitOptions.None)[1].Split(';')[0];
                                        my_account.token = token;

                                    }
                                }
                                return;
                    
                            }
                            else
                            {
                                
                                Console.WriteLine(message.Content.ReadAsStringAsync().Result);
                                Console.WriteLine("\n[!] Login Failed!");
                                Thread.Sleep(5000);
                                Console.Clear();
                                Console.WriteLine(banner);

                            }
                        }
                    }
                }
            }

        }

        
        public static bool get_data() // Get relevant information to keep same settings when editing profile
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("cookie", $"sessionid={my_account.session}");
                client.DefaultRequestHeaders.Add("User-Agent", "Instagram 85.0.0.21.100 Android (28/9; 380dpi; 1080x2147; OnePlus; HWEVA; OnePlus6T; qcom; en_US; 146536611)");
                using (HttpResponseMessage message = client.GetAsync("https://i.instagram.com/api/v1/accounts/current_user/?edit=true").Result)
                {
                    if (message.IsSuccessStatusCode)
                    {
                        string response = message.Content.ReadAsStringAsync().Result;
                        dynamic json = JsonConvert.DeserializeObject(response);
                        string email = json.user.email;
                        string username = json.user.username;
                        string phone = json.user.phone_number;
                        string user_id = json.user.pk;
                        my_account.phone=phone;
                        my_account.username=username;
                        my_account.email = email;
                        my_account.user_id = user_id;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(message.Content.ReadAsStringAsync().Result);
                        return false;

                    }
                       
                }
            }
        }

        public static void load()
        {
            if (File.Exists("proxies.txt"))
            {
                string[] _proxies = File.ReadAllLines("proxies.txt");
                proxies = new WebProxy[_proxies.Length];

                for (int i = 0; i < _proxies.Length; i++)
                {
                    WebProxy _proxy = new WebProxy($"http://{_proxies[i]}");
                    proxies[i] = _proxy;
                }
            }
        }

        public static void print()
        {
            if (!claimed) { 
                Console.WriteLine($"[+] Attempts : {attempts} | Method #{method}"); 
                Console.Write("\r " + new string(' ', Console.WindowWidth - 1) + "\r");
            }
        }


    }
}