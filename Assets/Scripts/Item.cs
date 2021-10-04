using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/DefaultItem")]
public class Item : ScriptableObject
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

}
