using System;

namespace lootboxes_db.Models
{
    public class UserInventory
    {
        public int UserInventoryId { get; set; }

        public int UserId { get; set; }

        public int ItemId { get; set; }

        public virtual Item Item { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
