using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/DefaultItem")]
public class Item : ScriptableObject, System.IComparable<Item>
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private new string name;
    [SerializeField]
    private string tooltip;
    [SerializeField]
    private int stackSize;

    public Sprite Sprite => sprite;
    public string Name => name;
    public string Tooltip => tooltip;
    public int StackSize => stackSize;

    ///<summary>
    ///Comparison for sorting. By default items are sorted by name.
    ///</summary>
    public virtual int CompareTo(Item other)
    {
        return this.name.CompareTo(other.name);
    }
}
