
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PersonalWebPage_MVC.Models
{
    public class Award
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
    }
}