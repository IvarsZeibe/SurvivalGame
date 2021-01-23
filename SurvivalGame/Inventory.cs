using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Inventory
    {
        private List<IItem> Content = new List<IItem>();
        public Inventory(int slotMax)
        {
            SlotMax = slotMax;
            for(int i = 0; i < slotMax; i++)
            {
                Content.Add(new EmptyItem());
            }
        }
        private int SlotMax { get; }
        public bool Add(IItem item, int index)
        {
            if (index < SlotMax)
            {
                Content[index] = item;
                return true;
            }
            else
                return false;
        }
        public bool Remove(IItem item)
        {
            if (Content.Contains(item))
            {
                Content.Remove(item);
                return true;
            }
            else
                return false;
        }
        public IItem Get(int index)
        {
            if (index <= Content.Count)
                return Content[index];
            else
                return null;
        }
    }
}
