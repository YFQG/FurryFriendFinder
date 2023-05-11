using FurryFriendFinder.Models.Data;

namespace FurryFriendFinder.Models.ViewModels
{
    public class PubliComment
    {
        public List<Comment> Comment = new();
        public List<Publication> publi = new();
        public PubliComment(List<Publication> publications, List<Comment> comments)
        {
            Comment = comments;
            publi = publications;
        }
    }
}
