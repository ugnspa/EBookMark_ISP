namespace EBookMark_ISP.Models
{
    public class Classroom
    {
        public int number { get; set; }
        public string usage { get; set; }
        public int height { get; set; }
        public int numberOfSeats { get; set; }
        public string building { get; set; }

        public Classroom(int number, string usage, int height, int numberOfSeats, string building)
        {
            this.number = number;
            this.usage = usage;
            this.height = height;
            this.numberOfSeats = numberOfSeats;
            this.building = building;
        }
    }
}
