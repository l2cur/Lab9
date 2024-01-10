using System.Globalization;
using System.Net;

namespace Task1
{
    class Program
    {
        static async Task Main()
        {
            string pathToTickerFile = "C:\\Users\\l2cur\\Desktop\\Lab_09\\Part_01\\ticker.txt";

            string[] tickers = ReadFile(pathToTickerFile);

            long todayUNIX = DateTimeOffset.Now.ToUnixTimeSeconds();
            long todayYearAgoUNIX = DateTimeOffset.Now.AddYears(-1).ToUnixTimeSeconds();

            List<Task> tasks = new List<Task>();

            foreach (var ticker in tickers)
            {
                WebSurfer ws = new WebSurfer(ticker, todayYearAgoUNIX, todayUNIX);
                tasks.Add(Task.Run(() => ws.GetData()));
            }

            await Task.WhenAll(tasks);
        }

        static string[] ReadFile(string pathToFile)
        {
            string[] result;
            using (StreamReader streamReader = new StreamReader(pathToFile))
            {
                string content = streamReader.ReadToEnd();
                string[] lines = content.Split('\r');
                for (int i = 1; i < lines.Length; i++)
                {
                    lines[i] = lines[i][1..];
                }
                result = lines;
            }

            return result;
        }
    }

    class WebSurfer
    {
        private string Ticker;
        private long StartTime;
        private long EndTime;
        public WebSurfer(string Ticker, long StartTime, long EndTime)
        {
            this.Ticker = Ticker;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        private static object locker = new();

        public void GetData()
        {
            string url =
                $"https://query2.finance.yahoo.com/v7/finance/download/{Ticker}?period1={StartTime}&period2={EndTime}&interval=1d&events=history&includeAdjustedClose=true";

            using (WebClient client = new WebClient())
            {
                try
                {
                    string csvData = client.DownloadString(url);

                    string[] lines = csvData.Split('\n');
                    double average = 0;
                    int days = 0;

                    foreach (var line in lines[1..^1]) 
                    {
                        string[] elems = line.Split(',');

                        if (elems.Length > 5)
                        {
                            NumberFormatInfo provider = new NumberFormatInfo();
                            provider.NumberDecimalSeparator = ".";
                            provider.NumberGroupSeparator = ",";
                            double high = Convert.ToDouble(elems[2], provider);
                            double low = Convert.ToDouble(elems[3], provider);

                            average += (high + low) / 2;
                            days++;
                        }
                    }

                    average /= days;
                    string resultLine = $"{Ticker} : {average}";

                    lock (locker)
                    {
                        using (StreamWriter streamWriter =
                               new StreamWriter("C:\\Users\\l2cur\\Desktop\\Lab_09\\Part_01\\out.txt", true))
                        {
                            streamWriter.WriteLine(resultLine);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error fetching data for {Ticker}: {e.Message}");
                    return;
                }
            }
        }
    }
}