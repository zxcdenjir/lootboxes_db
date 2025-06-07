using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace lootboxes_db.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int RarityId { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Rarity Rarity { get; set; } = null!;

    public virtual ICollection<LootBox> LootBoxes { get; set; } = new List<LootBox>();

    public virtual ICollection<UserInventory> UserInventories { get; set; } = new List<UserInventory>();

    public Item(string name, string description, int rarityId, int categoryId)
    {
        Name = name;
        Description = description;
        RarityId = rarityId;
        CategoryId = categoryId;
    }
    public override string ToString()
    {
        return $"ID: {Id}, Название: {Name}, Редкость: {Rarity.Name}, Категория: {Category.Name}, Описание: {Description}";
    }
}
