using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.WeatherClass
{
    internal class OpenWeather
    {
        public Coord Coord { get; set; }
        public Weather[] Weather { get; set; }
        public Main Main { get; set; }
        public Sys Sys { get; set; }
        public string Name { get; set; }
    }
}
