using System;
using System.Collections.Generic;

namespace lootboxes_db.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<UserInventory> UserInventories { get; set; } = new List<UserInventory>();

    public User(string fullName, string login, string password)
    {
        FullName = fullName;
        Login = login;
        Password = password;
    }

    public void PrintInventory()
    {
        Console.WriteLine("Ваш инвентарь:");
        if (UserInventories.Count == 0)
        {
            Console.WriteLine("Инвентарь пуст");
        }
        else
        {
            foreach (UserInventory userInventory in UserInventories)
            {
                Item item = userInventory.Item;
                Console.ForegroundColor = (ConsoleColor)Program.context.Rarities.Find(item.RarityId)!.Color;
                Console.WriteLine(item);
                Console.ResetColor();
            }
        }
    }
}
