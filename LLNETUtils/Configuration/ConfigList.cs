using System.Collections;
using LLNETUtils.Utils;

namespace LLNETUtils.Configuration;

internal class ConfigList : IList<object>, IList, IReadOnlyList<object>
{
    private readonly List<object> _list = new();

    public ConfigList()
    {
    }
    
    public ConfigList(IEnumerable<object> other)
    {
        foreach (object item in other)
        {
            Add(item);
        }
    }

    public int Count => _list.Count;
    
    bool IList.IsFixedSize => ((IList) _list).IsFixedSize;
    bool IList.IsReadOnly => ((IList) _list).IsReadOnly;
    
    bool ICollection.IsSynchronized => ((IList) _list).IsSynchronized;
    bool ICollection<object>.IsReadOnly => ((ICollection<object>) _list).IsReadOnly;
    object ICollection.SyncRoot => ((IList) _list).SyncRoot;

    public object this[int index]
    {
        get => _list[index];
        set => _list[index] = ConfigUtils.GetNewConfigItem(value);
    }
    
    object? IList.this[int index]
    {
        get => _list[index];
        set 
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            _list[index] = ConfigUtils.GetNewConfigItem(value);
        }
    }

    public IEnumerator<object> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(object item)
    {
        _list.Add(ConfigUtils.GetNewConfigItem(item));
    }
    
    int IList.Add(object? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        
        return ((IList) _list).Add(ConfigUtils.GetNewConfigItem(value));
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(object item)
    {
        return _list.Contains(item);
    }
    
    bool IList.Contains(object? value)
    {
        return ((IList) _list).Contains(value);
    }

    public void CopyTo(object[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }
    
    public void CopyTo(Array array, int index)
    {
        ((IList) _list).CopyTo(array, index);
    }

    public bool Remove(object item)
    {
        return _list.Remove(item);
    }
    
    void IList.Remove(object? value)
    {
        ((IList) _list).Remove(value);
    }

    public int IndexOf(object item)
    {
        return _list.IndexOf(item);
    }
    
    int IList.IndexOf(object? value)
    {
        return ((IList) _list).IndexOf(value);
    }

    public void Insert(int index, object item)
    {
        _list.Insert(index, ConfigUtils.GetNewConfigItem(item));
    }
    
    void IList.Insert(int index, object? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        
        ((IList) _list).Insert(index, ConfigUtils.GetNewConfigItem(value));
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    public List<object> ToList()
    {
        var newList = new List<object>();
        foreach (object item in _list)
        {
            object value = item;
            if (value is ConfigSection section)
            {
                value = section.ToDictionary();
            }
            else if (value is ConfigList list)
            {
                value = list.ToList();
            }
            
            newList.Add(value);
        }

        return newList;
    }
}