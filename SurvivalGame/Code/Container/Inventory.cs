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
        public int SlotMax { get; }
        public bool Add(IItem item, int index = -1)
        {
            if (index == -1)
            {
                for (int i = 0; i < Content.Count; i++)
                {
                    if (Content[i] is EmptyItem)
                    {
                        Content[i] = item;
                        return true;
                    }
                }
                return false;
            }
            else if (index < SlotMax)
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
                //Content.Remove(item);
                Content[GetIndex(item)] = new EmptyItem();
                return true;
            }
            else
                return false;
        }
        public bool RemoveAt(int index)
        {
            try 
            {
                Content[index] = new EmptyItem();
                return true;
            }
            catch { return false; }
        }
        public IItem Get(int index)
        {
            if (index <= Content.Count)
                return Content[index];
            else
                return null;
        }
        public int GetIndex(IItem item)
        {
            try { return Content.IndexOf(item); }
            catch { return -1; }
        }
        public IEnumerator<IItem> GetEnumerator()
        {
            foreach (var item in Content)
            {
                yield return item;
            }
        }
    }
}
