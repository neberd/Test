using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleTables;

namespace ConsoleApp
{
    public class Program
    {
        public static readonly string ApiKey = "Legg inn din API-nøkkel her";
        public static readonly string StationsData = "stations";
        public static readonly string AvailabilityData = "stations/availability";

        private static readonly IList<Station> StationList = new List<Station>();
        private static readonly IList<Station> AvailabilityList = new List<Station>();

        public static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                Console.Out.WriteLine("Mangler API-nøkkel!");
            }
            else
            {
                RunAsync(StationsData, StationList).Wait();
                RunAsync(AvailabilityData, AvailabilityList).Wait();
                Display();
            }
        }

        private static void Display()
        {
            if (StationList.Count == 0 || AvailabilityList.Count == 0)
            {
                Console.WriteLine("Fant ingen data");
                return;
            }
            var data = (from s in StationList
                        join a in AvailabilityList on s.Id equals a.Id
                        select new { s.Id, s.Title, a.Availability.Bikes, a.Availability.Locks }).ToList();

            var table = new ConsoleTable("Id", "Navn", "Ledige sykler", "Tilgjengelige låser");
            foreach (var d in data)
            {
                table.AddRow(d.Id, d.Title, d.Bikes, d.Locks);
            }

            Console.BufferHeight = 500;
            Console.WriteLine(table);
            Console.ReadLine();
        }

        private static async Task RunAsync(string apiMethod, ICollection<Station> stationList)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://oslobysykkel.no/api/v1/");
                    client.DefaultRequestHeaders.Add("Client-Identifier", ApiKey);
                    HttpResponseMessage response = await client.GetAsync(apiMethod);

                    if (response.IsSuccessStatusCode)
                    {
                        CityBike cityBike = await response.Content.ReadAsAsync<CityBike>();
                        foreach (Station station in cityBike.Stations)
                        {
                            stationList.Add(station);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
