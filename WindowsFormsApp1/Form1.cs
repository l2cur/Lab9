using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using WindowsFormsApp1.WeatherClass;
using Newtonsoft.Json;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Form1_Load();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string city = comboBox1.Text;

            Task.Run(() =>
            {
                double lat = 0;
                double lon = 0;
                StreamReader citys = new StreamReader(".\\city.txt");
                string x = citys.ReadToEnd();
                string[] y = x.Split('\n');
                foreach (string s in y)
                {
                    string[] z = s.Split('\t');
                    if (z[0] == city)
                    {
                        string[] coord = z[1].Split(',');
                        NumberFormatInfo provider = new NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                        lat = Convert.ToDouble(coord[0], provider);
                        lon = Convert.ToDouble(coord[1], provider);
                        break;
                    }
                }

                string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=7014cad18f46440ad10609dba5cc3cb4&units=metric";
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                string response;
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                    OpenWeather openWeather = JsonConvert.DeserializeObject<OpenWeather>(response);
                    Invoke((MethodInvoker)delegate
                    {
                        textBox1.Text = $"В {city} сейчас {openWeather.Weather[0].Description}, на улице {openWeather.Main.Temp}°C, ощущается как {openWeather.Main.Feels_like}°C.";
                    });
                }
            });
        }

        private void Form1_Load()
        {
            StreamReader city = new StreamReader(".\\city.txt");
            string x = city.ReadToEnd();
            string[] y = x.Split('\n');
            foreach (string s in y)
            {
                string[] z = s.Split('\t');
                comboBox1.Items.Add(z[0]);
            }
        }
    }
}
