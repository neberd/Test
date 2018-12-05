namespace ConsoleApp
{
    public class CityBike
    {
        public Station[] Stations { get; set; }
    }
    public class Station
    {
        public int Id { get; set; }
        public Availability Availability { get; set; }
        public string Title { get; set; }
    }
    public class Availability
    {
        public int Bikes { get; set; }
        public int Locks { get; set; }
    }
}
