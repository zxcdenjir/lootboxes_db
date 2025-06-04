using System;
using System.Collections.Generic;

namespace lootboxes_db.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public Category(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Название: {Name}";
    }
}
