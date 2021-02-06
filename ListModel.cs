using System;
using System.Collections.Generic;

namespace TodoApplication
{
    public class ListModel<TItem>
    {
        public List<TItem> Items { get; }
        public int Limit;

        public ListModel(int limit)
        {
            Items = new List<TItem>();
            Limit = limit;
        }

        public void AddItem(TItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(int index)
        {
            Items.RemoveAt(index);
        }

        public bool CanUndo()
        {
            return false;
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
