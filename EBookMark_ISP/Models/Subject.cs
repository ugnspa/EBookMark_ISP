namespace EBookMark_ISP.Models
{
    public class Subject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public Subject(string code, string name, string desc, string lang)
        { 
            Code = code;
            Name= name;
            Description= desc;
            Language = lang;

        } 

    }
}
