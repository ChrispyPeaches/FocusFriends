using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Shared.Models
{
    public class ShopItem
    {
        public string Name { get; set; }
        public byte[] ImageSource { get; set; }
        public ShopItemType Type { get; set; }
        public int Price { get; set; }
    }

    public enum ShopItemType
    {
        Pets,
        Sounds,
        Furniture
    }
}
