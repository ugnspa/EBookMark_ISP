namespace EBookMark_ISP.Models
{
    public class Classroom
    {
        public int ID { get; set; }
        public int number { get; set; }
        public string usage { get; set; }
        public int height { get; set; }
        public int numberOfSeats { get; set; }
        public string building { get; set; }


        public Classroom()
        {

        }
        public Classroom(int id, int number, string usage, int height, int numberOfSeats, string building)
        {
            this.ID = id;
            this.number = number;
            this.usage = usage;
            this.height = height;
            this.numberOfSeats = numberOfSeats;
            this.building = building;
        }

        public string FullInfo => $"Room: {number}, {usage}, Capacity: {numberOfSeats}, Building: {building}";
    }
}
