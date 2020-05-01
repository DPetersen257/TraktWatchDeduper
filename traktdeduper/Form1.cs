using System;
using System.Windows.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace traktdeduper
{
    public partial class Form1 : Form
    {
        Uri baseAddress = new Uri("https://api.trakt.tv/");

        public Form1()
        {
            InitializeComponent();
        }

        private async void MenuItem_reAuth_Click(object sender, EventArgs e)
        {
            var httpClient = new HttpClient { BaseAddress = baseAddress };
            var content = new StringContent("{  \"client_id\": \""+ Properties.Settings.Default.clientID+ "\"}", System.Text.Encoding.Default, "application/json");
            var response = await httpClient.PostAsync("oauth/device/code", content);

            if (!response.IsSuccessStatusCode)
                MessageBox.Show("Failed to get authorization code "+response.StatusCode.ToString());

            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (!string.IsNullOrEmpty(responseObject.SelectToken("user_code").ToString()))
            {
                Console.WriteLine("Successfully received user code: " + responseObject.SelectToken("user_code").ToString());
                MessageBox.Show(string.Format("Please enter code\r\n\r\n{0}\r\n\r\nat\r\n{1}\r\nbefore clicking \"OK\"", responseObject.SelectToken("user_code").ToString(), responseObject.SelectToken("verification_url").ToString()),"Reauthorization",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            PollAccessToken(responseObject.SelectToken("device_code").ToString(), Properties.Settings.Default.clientID, Properties.Settings.Default.clientSecret, int.Parse(responseObject.SelectToken("interval").ToString()), int.Parse(responseObject.SelectToken("expires_in").ToString()));
        }

        private async Task RefreshAccessToken()
        {
            var httpClient = new HttpClient { BaseAddress = baseAddress };

            var content = new StringContent("{  \"refresh_token\": \"" +Properties.Settings.Default.refreshToken+"\",  \"client_id\": \""+Properties.Settings.Default.clientID+"\",  \"client_secret\": \""+Properties.Settings.Default.clientSecret+"\",  \"redirect_uri\": \"urn:ietf:wg:oauth:2.0:oob\",  \"grant_type\": \"refresh_token\"}", System.Text.Encoding.Default, "application/json");
            var response = await httpClient.PostAsync("oauth/token", content);
            var test = Properties.Settings.Default.refreshToken;
            if (!response.IsSuccessStatusCode)
                return;

            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            Properties.Settings.Default.AccessExpiration = DateTime.Now.AddMonths(3);
            Properties.Settings.Default.AccessToken = responseObject.SelectToken("access_token").ToString();
            Properties.Settings.Default.refreshToken = responseObject.SelectToken("refresh_token").ToString();
        }

        private async void PollAccessToken(String deviceCode, String clientID, String clientSecret, int Interval, int Expiration)
        {
            var httpClient = new HttpClient { BaseAddress = baseAddress };
            int waited = 0;

            toolStripStatusLabel2.Text = "Attempting Reauth";

            while (waited< Expiration)
            {
                var content = new StringContent("{  \"code\": \"" + deviceCode + "\",  \"client_id\": \"" + clientID + "\",  \"client_secret\": \"" + clientSecret + "\"}", System.Text.Encoding.Default, "application/json");
                var response = await httpClient.PostAsync("oauth/device/token", content);
                switch ((int)response.StatusCode)
                {
                    case 200: //Success - save access token
                        {
                            JObject responseData = JObject.Parse(await response.Content.ReadAsStringAsync());
                            Properties.Settings.Default.AccessToken = responseData.SelectToken("access_token").ToString();
                            Properties.Settings.Default.refreshToken = responseData.SelectToken("refresh_token").ToString();
                            Properties.Settings.Default.AccessExpiration = DateTime.Now.AddMonths(3);

                            Properties.Settings.Default.Save();
                            Console.WriteLine(String.Format("Successfully retrieved access token: {0}", Properties.Settings.Default.AccessToken));
                            toolStripStatusLabel2.Text = "Reauth Successful";

                            return;
                        }
                    case 400: //Pending - waiting for the user to authorize online
                        {
                            System.Threading.Thread.Sleep(Interval * 1000); //convert to ms and sleep for a interval
                            waited += Interval;
                            continue;
                        }
                    case 404: // Not Found - invalid device code
                        {
                            MessageBox.Show("Device Code not found. Try Reauthing", response.StatusCode.ToString(), MessageBoxButtons.OK);
                            continue;
                        }
                    case 409: //Already Used - user already approved this code
                        {
                            MessageBox.Show("Code already used. Try again.", response.StatusCode.ToString(), MessageBoxButtons.OK);
                            return;
                        }
                    case 410: //Expired - the tokens have expired, restart the process
                        {
                            MessageBox.Show("The tokens have expired. Try again.", response.StatusCode.ToString(), MessageBoxButtons.OK);
                            return;
                        }
                    case 418: //Denied - user explicitly denied this code
                        {
                            MessageBox.Show("You denied the code online. Try again.", response.StatusCode.ToString(), MessageBoxButtons.OK);
                            return;
                        }
                    case 429: //Slow Down - your app is polling too quickly
                        {
                            System.Threading.Thread.Sleep(2*Interval * 1000); //convert to ms and sleep for a interval
                            waited += (Interval*2);
                            continue;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
            return;
        }

        private async Task<JArray> GetHistory(String Username)
        {
            var httpClient = new HttpClient { BaseAddress = baseAddress };
            int page = 1;
            int limit = 15000;

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", Properties.Settings.Default.clientID);

            var response = await httpClient.GetAsync("users/" + Username + "/history/" + listBox1.SelectedItem.ToString() + "/?page="+page+"&limit="+limit);
            string responseData = await response.Content.ReadAsStringAsync();
            toolStripStatusLabel2.Text = "History retrieval successful";
            return JArray.Parse(responseData.ToString());
        }

        private List<String> FindDupes(JArray History)
        {
            List<String> idsToRemove = new List<String>();
            Dictionary<String, JArray> HistoryDict = new Dictionary<string, JArray>();

            richTextBox1.AppendText("\r\n\rDupes Removed:");

            //Add each watched item to a dictionary by trakt slug (name+year)
            //Each dictionary item has a jarray to hold multiple watches of the same movie
            foreach (JObject item in History)
            {
                string tokenPath = "";
                if (item.SelectToken("type").ToString() == "movie")
                    tokenPath = "movie.ids.slug";
                else if (item.SelectToken("type").ToString() == "episode")
                    tokenPath = "episode.ids.trakt";


                if (!HistoryDict.ContainsKey(item.SelectToken(tokenPath).ToString())) //if itemname not already in dictionary
                    HistoryDict.Add(item.SelectToken(tokenPath).ToString(), new JArray(item)); //Add new dictionary item of <title, Jarray(watched object)>
                else //if itemname already in dictionary
                {
                    foreach (JToken value in HistoryDict[item.SelectToken(tokenPath).ToString()]) //for each item in the historydict at location [title]
                    {
                        var valuetimestamp = DateTime.Parse(value.SelectToken("watched_at").ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        var itemTimestamp = DateTime.Parse(item.SelectToken("watched_at").ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        if ( Math.Abs(value: (itemTimestamp - valuetimestamp).Days) < 2)  
                        {
                            //If watched more than once in the last 2 days, add to idstoremove list to remove the watch in a later function
                            idsToRemove.Add(item.SelectToken("id").ToString());

                            if (listBox1.SelectedItem.ToString() == "movies")
                                richTextBox1.AppendText("\r\nhttps://trakt.tv/movies/" + item.SelectToken("movie.ids.slug").ToString());
                            if (listBox1.SelectedItem.ToString() == "episodes")
                                richTextBox1.AppendText("\r\n" + item.SelectToken("show.ids.slug").ToString() + " - " + item.SelectToken("episode.title").ToString());
                            break; //If one match found, no need to compare against all items in the historydict
                        }
                    }
                    HistoryDict[item.SelectToken(tokenPath).ToString()].Add(item); //Add entry to historydict if no dupe found but already been seen once before
                }
            }
            return idsToRemove;
        }

        private async Task RemoveFromHistory(List<String> idsToRemove)
        {
            var httpClient = new HttpClient { BaseAddress = baseAddress };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", "Bearer "+ Properties.Settings.Default.AccessToken);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", Properties.Settings.Default.clientID);

            StringContent content = new StringContent("{  \"ids\": [" + string.Join(",", idsToRemove.ToArray()) + "]}", System.Text.Encoding.Default, "application/json");

            var response = await httpClient.PostAsync("sync/history/remove", content);
            if (!response.IsSuccessStatusCode)
            {
                toolStripStatusLabel2.Text = "Duplicate removal failed.   " + response.StatusCode;
                return;
            }
            var responseData = await response.Content.ReadAsStringAsync();
            toolStripStatusLabel2.Text = "Duplicate removal complete.   " + idsToRemove.Count + " items removed.";
        }

        private async void Button_traktDedupe_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.AccessToken)) //If access token missing, request one
                 MenuItem_reAuth_Click(sender, e);
            else if (Properties.Settings.Default.AccessExpiration < DateTime.Now) // if access token expired, request a new one
                await RefreshAccessToken();

            toolStripStatusLabel2.Text = "Getting history for " + Properties.Settings.Default.username;
            var WatchedHistory = await GetHistory(Properties.Settings.Default.username);

            toolStripStatusLabel2.Text = "Finding Dupes";
            List <String> WatchesToRemove = FindDupes(WatchedHistory);

            if (WatchesToRemove.Count >= 1)
            {
                toolStripStatusLabel2.Text = "Removing Dupes";
                await RemoveFromHistory(WatchesToRemove);
            }
            else
                toolStripStatusLabel2.Text = "No Dupes Found";
        }

        private void MenuItem_Settings_Click(object sender, EventArgs e)
        {
            formSettings f2 = new formSettings();
            f2.Show();
            f2.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.username)
                || string.IsNullOrWhiteSpace(Properties.Settings.Default.clientID)
                || string.IsNullOrWhiteSpace(Properties.Settings.Default.clientSecret))
                MenuItem_Settings_Click(sender,e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            await RefreshAccessToken();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}