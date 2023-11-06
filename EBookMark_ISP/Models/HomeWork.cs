using System.Data;

namespace EBookMark_ISP.Models
{
    public class HomeWork
    {
        public int Id { get; set; }
        public DateTime FulfillDate { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }
        public int FileCount { get; set; }
        public string Subject { get; set; }

        public HomeWork()
        {

        }

        public HomeWork(int id, DateTime fulfilldate, DateTime setdate, DateTime uploaddate, string description, int filecount, string subject)
        {
            Id = id;
            FulfillDate = fulfilldate;
            SetDate = setdate;
            UploadDate = uploaddate;
            Description = description;
            FileCount = filecount;
            Subject = subject;

        }


    }

    

}
