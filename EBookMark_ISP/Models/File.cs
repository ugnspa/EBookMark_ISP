namespace EBookMark_ISP.Models
{
    public class File
    {
        public string FileType { get; set; }
        public string Name { get; set; }
        public double Size { get; set;}



        public File()
        {

        }

        public File(string filetype, string name, double size)
        {
            FileType = filetype;
            Name = name;
            Size = size;

        }


    }

    

}