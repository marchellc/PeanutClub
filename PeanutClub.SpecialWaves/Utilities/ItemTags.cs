using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabApi.Features.Wrappers;

using LabExtended.Events;
using LabExtended.Core.Pooling.Pools;

namespace PeanutClub.SpecialWaves.Utilities;

/// <summary>
/// An extremely simplified version of Custom Items which only tracks items.
/// </summary>
public static class ItemTags
{
    /// <summary>
    /// A list of all tracked serials and their tags.
    /// </summary>
    public static Dictionary<ushort, Dictionary<string, object[]?>> TrackedSerials { get; } = new();

    /// <summary>
    /// Sets the value of an item.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the item.</param>
    /// <param name="itemIndex">The index of the value.</param>
    /// <param name="value">The value to set.</param>
    public static void SetValue(this Item item, string itemTag, int itemIndex, object value)
    {
        if (item?.Base == null || item.Serial == 0)
            return;
        
        SetValue(item.Serial, itemTag, itemIndex, value);
    }
    
    /// <summary>
    /// Sets the value of an item.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the item.</param>
    /// <param name="itemIndex">The index of the value.</param>
    /// <param name="value">The value to set.</param>
    public static void SetValue(this Pickup item, string itemTag, int itemIndex, object value)
    {
        if (item?.Base == null || item.Serial == 0)
            return;
        
        SetValue(item.Serial, itemTag, itemIndex, value);
    }
    
    /// <summary>
    /// Sets the value of an item.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the item.</param>
    /// <param name="itemIndex">The index of the value.</param>
    /// <param name="value">The value to set.</param>
    public static void SetValue(this ItemBase item, string itemTag, int itemIndex, object value)
    {
        if (item == null || item.ItemSerial == 0)
            return;
        
        SetValue(item.ItemSerial, itemTag, itemIndex, value);
    }
    
    /// <summary>
    /// Sets the value of an item.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the item.</param>
    /// <param name="itemIndex">The index of the value.</param>
    /// <param name="value">The value to set.</param>
    public static void SetValue(this ItemPickupBase item, string itemTag, int itemIndex, object value)
    {
        if (item == null || item.Info.Serial == 0)
            return;
        
        SetValue(item.Info.Serial, itemTag, itemIndex, value);
    }
    
    /// <summary>
    /// Sets the value of an item.
    /// </summary>
    /// <param name="itemSerial">The target item's serial.</param>
    /// <param name="itemTag">The tag of the item.</param>
    /// <param name="itemIndex">The index of the value.</param>
    /// <param name="value">The value to set.</param>
    public static void SetValue(ushort itemSerial, string itemTag, int itemIndex, object value)
    {
        if (string.IsNullOrEmpty(itemTag))
            throw new ArgumentNullException(nameof(itemTag));
        
        if (itemIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(itemIndex));
        
        var dict = TrackedSerials.GetOrAdd(itemSerial, () => DictionaryPool<string, object[]?>.Shared.Rent());
        
        if (!dict.TryGetValue(itemTag, out var array))
            dict.Add(itemTag, array = new object[itemIndex + 1]);
        
        if (array!.Length < itemIndex + 1)
            Array.Resize(ref array, itemIndex + 1);
        
        array![itemIndex] = value;
    }
    
    /// <summary>
    /// Attempts to get a custom stored value.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the stored values.</param>
    /// <param name="itemIndex">The index of the stored value.</param>
    /// <param name="value">The found value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>true if the value was found</returns>
    public static bool TryGetValue<T>(this Item item, string itemTag, int itemIndex, out T value)
    {
        if (item?.Base == null || item.Serial == 0)
        {
            value = default!;
            return false;
        }

        return TryGetValue(item.Serial, itemTag, itemIndex, out value);
    }
    
    /// <summary>
    /// Attempts to get a custom stored value.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the stored values.</param>
    /// <param name="itemIndex">The index of the stored value.</param>
    /// <param name="value">The found value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>true if the value was found</returns>
    public static bool TryGetValue<T>(this Pickup item, string itemTag, int itemIndex, out T value)
    {
        if (item?.Base == null || item.Serial == 0)
        {
            value = default!;
            return false;
        }

        return TryGetValue(item.Serial, itemTag, itemIndex, out value);
    }
    
    /// <summary>
    /// Attempts to get a custom stored value.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the stored values.</param>
    /// <param name="itemIndex">The index of the stored value.</param>
    /// <param name="value">The found value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>true if the value was found</returns>
    public static bool TryGetValue<T>(this ItemBase item, string itemTag, int itemIndex, out T value)
    {
        if (item == null || item.ItemSerial == 0)
        {
            value = default!;
            return false;
        }

        return TryGetValue(item.ItemSerial, itemTag, itemIndex, out value);
    }
    
    /// <summary>
    /// Attempts to get a custom stored value.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="itemTag">The tag of the stored values.</param>
    /// <param name="itemIndex">The index of the stored value.</param>
    /// <param name="value">The found value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>true if the value was found</returns>
    public static bool TryGetValue<T>(this ItemPickupBase item, string itemTag, int itemIndex, out T value)
    {
        if (item == null || item.Info.Serial == 0)
        {
            value = default!;
            return false;
        }

        return TryGetValue(item.Info.Serial, itemTag, itemIndex, out value);
    }
    
    /// <summary>
    /// Attempts to get a custom stored value.
    /// </summary>
    /// <param name="itemSerial">The serial of the item.</param>
    /// <param name="itemTag">The tag of the stored values.</param>
    /// <param name="itemIndex">The index of the stored value.</param>
    /// <param name="value">The found value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>true if the value was found</returns>
    public static bool TryGetValue<T>(ushort itemSerial, string itemTag, int itemIndex, out T value)
    {
        if (!TrackedSerials.TryGetValue(itemSerial, out var dict))
        {
            value = default!;
            return false;
        }

        if (!dict.TryGetValue(itemTag, out var values))
        {
            value = default!;
            return false;
        }

        if (values == null || itemIndex < 0 || itemIndex >= values.Length)
        {
            value = default!;
            return false;
        }
        
        value = (T)values[itemIndex];
        return true;
    }
    
    /// <summary>
    /// Whether or not a specific item has a saved tag.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>true if the tag was found</returns>
    public static bool HasTag(this Item item, string tag)
        => item?.Base != null && HasTag(item.Serial, tag);

    /// <summary>
    /// Whether or not a specific item has a saved tag.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>true if the tag was found</returns>
    public static bool HasTag(this Pickup item, string tag)
        => item?.Base != null && HasTag(item.Serial, tag);
    
    /// <summary>
    /// Whether or not a specific item has a saved tag.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>true if the tag was found</returns>
    public static bool HasTag(this ItemBase item, string tag)
        => item != null && HasTag(item.ItemSerial, tag);
    
    /// <summary>
    /// Whether or not a specific item has a saved tag.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>true if the tag was found</returns>
    public static bool HasTag(this ItemPickupBase item, string tag)
        => item != null && HasTag(item.Info.Serial, tag);
    
    /// <summary>
    /// Whether or not a specific item has a saved tag.
    /// </summary>
    /// <param name="itemSerial">The item's serial.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>true if the tag was found</returns>
    public static bool HasTag(ushort itemSerial, string tag)
        => TrackedSerials.TryGetValue(itemSerial, out var list) && list.ContainsKey(tag);

    /// <summary>
    /// Sets an item's tag.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="tag">The tag to add.</param>
    /// <param name="arraySize">The size of the array used to store custom values.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetTag(this Item item, string tag, int arraySize = -1)
    {
        if (item?.Base != null && item.Serial != 0)
        {
            SetTag(item.Serial, tag, arraySize);
        }
    }
    
    /// <summary>
    /// Sets an item's tag.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="tag">The tag to add.</param>
    /// <param name="arraySize">The size of the array used to store custom values.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetTag(this Pickup item, string tag, int arraySize = -1)
    {
        if (item?.Base != null && item.Serial != 0)
        {
            SetTag(item.Serial, tag, arraySize);
        }
    }
    
    /// <summary>
    /// Sets an item's tag.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="tag">The tag to add.</param>
    /// <param name="arraySize">The size of the array used to store custom values.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetTag(this ItemBase item, string tag, int arraySize = -1)
    {
        if (item != null && item.ItemSerial != 0)
        {
            SetTag(item.ItemSerial, tag, arraySize);
        }
    }
    
    /// <summary>
    /// Sets an item's tag.
    /// </summary>
    /// <param name="item">The target item.</param>
    /// <param name="tag">The tag to add.</param>
    /// <param name="arraySize">The size of the array used to store custom values.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetTag(this ItemPickupBase item, string tag, int arraySize = -1)
    {
        if (item != null && item.Info.Serial != 0)
        {
            SetTag(item.Info.Serial, tag, arraySize);
        }
    }

    /// <summary>
    /// Sets an item's tag.
    /// </summary>
    /// <param name="itemSerial">The target item's serial.</param>
    /// <param name="tag">The tag to add.</param>
    /// <param name="arraySize">The size of the array used to store custom values.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetTag(ushort itemSerial, string tag, int arraySize = -1)
    {
        if (string.IsNullOrEmpty(tag))
            throw new ArgumentNullException("tag");

        var tags = TrackedSerials.GetOrAdd(itemSerial, () => DictionaryPool<string, object[]?>.Shared.Rent());
        
        if (!tags.ContainsKey(tag))
            tags.Add(tag, arraySize > 0 ? new object[arraySize] : null!);
    }

    /// <summary>
    /// Removes an item's tag.
    /// </summary>
    /// <param name="itemSerial">The item's serial.</param>
    /// <param name="tag">The tag to remove.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RemoveTag(ushort itemSerial, string tag)
    {
        if (string.IsNullOrEmpty(tag))
            throw new ArgumentNullException(nameof(tag));

        if (!TrackedSerials.TryGetValue(itemSerial, out var list))
            return;
        
        list.Remove(tag);

        if (list.Count == 0)
        {
            DictionaryPool<string, object[]?>.Shared.Return(list);

            TrackedSerials.Remove(itemSerial);
        }
    }

    private static void Internal_OnRestart()
    {
        foreach (var pair in TrackedSerials)
        {
            DictionaryPool<string, object[]?>.Shared.Return(pair.Value);
        }
        
        TrackedSerials.Clear();
    }
    
    internal static void Internal_Init()
    {
        ExRoundEvents.Restarting += Internal_OnRestart;
    }
}