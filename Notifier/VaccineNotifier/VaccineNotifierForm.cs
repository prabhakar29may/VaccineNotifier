using System;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace VaccineNotifier
{
    public partial class VaccineNotifierForm : Form
    {
        public VaccineNotifierForm()
        {
            InitializeComponent();
        }
        SpeechSynthesizer speechSynthesizerObj;
        private void VaccineNotifierForm_Load(object sender, EventArgs e)
        {
            speechSynthesizerObj = new SpeechSynthesizer();
            timer1.Interval = 40000;            
        }
        private void Timer1_Tick(object sender, System.EventArgs e)
        {
            StartSearch(txtPin.Text);
        }
        private void StartSearch(string pin)
        {
            try
            {
                string result = "";
                int dt = dateTimePicker1.Value.Day;
                int month = dateTimePicker1.Value.Month;                
                for (int a = dt; a <= 31; a++)
                {
                    string url = string.Concat("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin?pincode=",pin,"&date=", a.ToString(), "-",month.ToString(),"-2021");
                    var json = new WebClient().DownloadString(url);
                    timer1.Stop();
                    timer1.Interval = 40000;
                    timer1.Start();
                    var obj = JsonConvert.DeserializeObject<JObject>(json);
                    int n = obj["centers"].Children().Count();
                    for (int i = 0; i < n; i++)
                    {
                        var obj1 = JsonConvert.DeserializeObject<JObject>(obj["centers"][i].ToString());
                        int n1 = obj1["sessions"].Children().Count();
                        string sr = obj1["name"].ToString();
                        for (int j = 0; j < n1; j++)
                        {
                            var obj2 = JsonConvert.DeserializeObject<JObject>(obj1["sessions"][j].ToString());
                            var obj3 = JsonConvert.DeserializeObject<JObject>(obj2.ToString());
                            int slot = Convert.ToInt32(obj3["available_capacity"].ToString());
                            if (slot > 0)
                            {
                                string n12 = obj3["min_age_limit"].ToString();
                                if (n12.ToString().Contains("18"))
                                {
                                    result = string.Concat("Vaccine Available on ", obj3["date"].ToString(),
                                        " at ", obj1["name"].ToString(), ". Available Slots ", obj3["available_capacity"].ToString());
                                }
                            }
                        }
                    }
                }
                if (result == "")
                {
                    //speechSynthesizerObj = new SpeechSynthesizer();
                    //speechSynthesizerObj.SpeakAsync("Vaccine not Available");
                }
                else
                {
                    speechSynthesizerObj = new SpeechSynthesizer();
                    speechSynthesizerObj.SpeakAsync(result);
                }
            }
            catch (Exception ex)
            {

                //speechSynthesizerObj = new SpeechSynthesizer();
                //speechSynthesizerObj.SpeakAsync(ex.Message.ToString());
                timer1.Stop();
                timer1.Interval = 90000;
                timer1.Start();
            }
        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            timer1.Start();            
        }
    }
}
