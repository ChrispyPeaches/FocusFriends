using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class ShopItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageSource { get; set; }
        public ShopItemType Type { get; set; }
        public int Price { get; set; }
    }

    public enum ShopItemType
    {
        Pets,
        Sounds,
        Decor
    }
}
