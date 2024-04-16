using UploadImage.Data;

namespace UploadImage.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool Unlocked { get; set; }
        public bool IncorrectPassword { get; set; }
    }
}
