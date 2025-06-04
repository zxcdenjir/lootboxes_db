using System;
using System.Collections.Generic;

namespace lootboxes_db.Models;

public partial class Rarity
{
    public int Id { get; set; }

    public decimal DropChance { get; set; }

    public int Color { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public Rarity(decimal dropChance, int color, string name)
    {
        DropChance = dropChance;
        Color = color;
        Name = name;
    }

    override public string ToString()
    {
        return $"ID: {Id}, Название: {Name}, Шанс выпадения: {DropChance * 100:F2}%, Цвет: {Color}";
    }
}
