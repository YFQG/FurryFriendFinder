using FurryFriendFinder.Models.Data;
namespace FurryFriendFinder.Models.ViewModels
{
    public class ProductInvent
    {
        public Product product { get; set; }
        public Inventory inventory { get; set; }
        public Brand brand { get; set; }
        public Packing packing { get; set; }
        public AnimalType type { get; set; }
        public Movement movement { get; set; }
    }
}
