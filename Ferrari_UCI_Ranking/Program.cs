using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Ferrari_Selenium
{
    internal class Program
    {
        public class Rider
        {
            public string RiderName { get; set; }
            public string Nationality { get; set; }
            public string Team { get; set; }
            public string Age { get; set; }
            public string Points { get; set; }
        }

        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://dataride.uci.ch/iframe/RankingDetails/1?disciplineId=10&groupId=1&momentId=178357&disciplineSeasonId=432&rankingTypeId=1&categoryId=22&raceTypeId=0");

            Thread.Sleep(2000);

            IWebElement tbodyElement = driver.FindElement(By.CssSelector("tbody[role='rowgroup']"));

            IReadOnlyCollection<IWebElement> rows = tbodyElement.FindElements(By.CssSelector("tr.k-master-row"));

            List<Rider> riders = new List<Rider>();

            foreach (var row in rows)
            {
                string riderName = row.FindElement(By.CssSelector("td:nth-child(4) a")).Text;
                string nationality = row.FindElement(By.CssSelector("td:nth-child(5)")).Text;
                string team = row.FindElement(By.CssSelector("td:nth-child(6)")).Text;
                string age = row.FindElement(By.CssSelector("td:nth-child(7)")).Text;
                string points = row.FindElement(By.CssSelector("td.points")).Text;

                riders.Add(new Rider
                {
                    RiderName = riderName,
                    Nationality = nationality,
                    Team = team,
                    Age = age,
                    Points = points
                });
            }

            foreach (var rider in riders)
            {
                Console.WriteLine("Rider Name: " + rider.RiderName);
                Console.WriteLine("Nationality: " + rider.Nationality);
                Console.WriteLine("Team: " + rider.Team);
                Console.WriteLine("Age: " + rider.Age);
                Console.WriteLine("Points: " + rider.Points);
                Console.WriteLine();
            }

            // Serialize the data to JSON
            string json = JsonConvert.SerializeObject(riders, Formatting.Indented);

            // Write JSON to file
            string filePath = "riders.json";
            System.IO.File.WriteAllText(filePath, json);

            Thread.Sleep(10000);
            driver.Quit();
        }
    }
}
