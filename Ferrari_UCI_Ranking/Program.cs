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
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.WriteLine("1 - Get Riders");
                Console.WriteLine("2 - Show Riders");
                Console.WriteLine("3 - Exit");
                Console.Write("Choose an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        GetRiders();
                        break;
                    case "2":
                        string json = System.IO.File.ReadAllText("riders.json");
                        List<Rider> riders = JsonConvert.DeserializeObject<List<Rider>>(json);
                        foreach (var rider in riders)
                        {
                            Console.WriteLine("Rider Name: " + rider.RiderName);
                            Console.WriteLine("Nationality: " + rider.Nationality);
                            Console.WriteLine("Team: " + rider.Team);
                            Console.WriteLine("Age: " + rider.Age);
                            Console.WriteLine("Points: " + rider.Points);
                            Console.WriteLine();
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    case "3":
                        keepGoing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            

        }

        public static List<Rider> GetRiders()
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://dataride.uci.ch/iframe/RankingDetails/1?disciplineId=10&groupId=1&momentId=178357&disciplineSeasonId=432&rankingTypeId=1&categoryId=22&raceTypeId=0");

            Thread.Sleep(2000);

            IWebElement lastPageLink = driver.FindElement(By.CssSelector("a.k-pager-last"));

            string totalPagesString = lastPageLink.GetAttribute("data-page");

            int totalPages;

            if (int.TryParse(totalPagesString, out totalPages))
            {
                Console.WriteLine("Total number of data pages: " + totalPages);
            }
            else
            {
                Console.WriteLine("Failed to parse total number of data pages.");
                Environment.Exit(1);
            }

            List<Rider> riders = new List<Rider>();

            for (int i = 0; i < totalPages; i++)
            {

                IWebElement tbodyElement = driver.FindElement(By.CssSelector("tbody[role='rowgroup']"));

                IReadOnlyCollection<IWebElement> rows = tbodyElement.FindElements(By.CssSelector("tr.k-master-row"));

                foreach (var row in rows)
                {
                    string riderName = row.FindElement(By.CssSelector("td:nth-child(4) a")).Text;
                    Console.WriteLine(riderName);
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

                if (i < totalPages - 1)
                {
                    IWebElement nextPageButton = driver.FindElement(By.CssSelector("[aria-label='Go to the next page']"));
                    nextPageButton.Click();
                    Thread.Sleep(1000);
                }
            }

            Thread.Sleep(10000);
            driver.Quit();

            string json = JsonConvert.SerializeObject(riders, Formatting.Indented);

            string filePath = @"riders.json";
            System.IO.File.WriteAllText(filePath, json);

            return riders;
        }
    }
}