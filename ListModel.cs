using System;
using System.Collections.Generic;

namespace TodoApplication
{
    public class CommandBuilder<TItem>
    {
        public ListModel<TItem> ListModel { get; }

        public CommandBuilder(ListModel<TItem> listModel)
        {
            this.ListModel = listModel;
        }

        public ICommand CreateAddItemCommand(TItem itemToAdd)
        {
            return new AddItemCommand<TItem>(ListModel.Items, itemToAdd);
        }
        public ICommand CreateRemoveItemCommand(int indexOfItemToRemove)
        {
            return new RemoveItemCommand<TItem>(ListModel.Items, indexOfItemToRemove);
        }
        public ICommand CreateUndoCommand()
        {
            return new UndoCommand(ListModel.HistoryStack);
        }
    }

    public interface ICommand
    {
        void Undo();
        bool Execute();
    }

    public class UndoCommand : ICommand
    {
        public LimitedSizeStack<ICommand> HistoryStack { get; }

        public UndoCommand(LimitedSizeStack<ICommand> historyStack)
        {
            this.HistoryStack = historyStack;
        }

        public bool Execute()
        {
            HistoryStack.Pop().Undo();
            return false;
        }

        public void Undo()
        {
            throw new NotSupportedException();
        }
    }
    public class AddItemCommand<TItem> : ICommand
    {
        public List<TItem> Items { get; }
        public TItem ItemToAdd { get; }

        public AddItemCommand(List<TItem> items, TItem itemToAdd)
        {
            this.Items = items;
            this.ItemToAdd = itemToAdd;
        }
        public bool Execute()
        {
            Items.Add(ItemToAdd);
            return true;
        }

        public void Undo()
        {
            Items.RemoveAt(Items.Count - 1);
        }
    }
    public class RemoveItemCommand<TItem> : ICommand
    {
        public List<TItem> Items { get; }
        public int IndexOfItemToRemove { get; }
        public TItem Backup { get; private set; }

        public RemoveItemCommand(List<TItem> items, int indexOfItemToRemove)
        {
            this.Items = items;
            this.IndexOfItemToRemove = indexOfItemToRemove;
        }
        public bool Execute()
        {
            Backup = Items[IndexOfItemToRemove];
            Items.RemoveAt(IndexOfItemToRemove);
            return true;
        }

        public void Undo()
        {
            Items.Insert(IndexOfItemToRemove, Backup);
        }
    }

    public class ListModel<TItem>
    {
        public List<TItem> Items { get; }
        public CommandBuilder<TItem> CommandBuilder { get; }
        public int Limit { get; }

        public LimitedSizeStack<ICommand> HistoryStack { get; private set; }

        public ListModel(int limit)
        {
            Items = new List<TItem>();
            Limit = limit;
            HistoryStack = new LimitedSizeStack<ICommand>(Limit);
            CommandBuilder = new CommandBuilder<TItem>(this);
        }

        public void AddItem(TItem item)
        {
            var command = CommandBuilder.CreateAddItemCommand(item);
            ExecuteCommand(command);
        }

        public void RemoveItem(int index)
        {
            var command = CommandBuilder.CreateRemoveItemCommand(index);
            ExecuteCommand(command);
        }

        public bool CanUndo()
        {
            return HistoryStack.Count > 0;
        }

        public void Undo()
        {
            var command = CommandBuilder.CreateUndoCommand();
            ExecuteCommand(command);
        }

        public void ExecuteCommand(ICommand command)
        {
            if (command.Execute())
                HistoryStack.Push(command);
        }
    }
}
