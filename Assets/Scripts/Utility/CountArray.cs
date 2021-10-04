using System;

///<summary>
///A much simpler "readonly" version of List<T>.
///</summary>
public class CountArray<T>
{
    public int Count {get; private set;}
    private T[] array;

    public T this[int index] => array[index];

    public CountArray(int size)
    {
        array = new T[size];
        Count = 0;
    }
    public CountArray(T[] arr)
    {
        if(typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) is null) throw new System.ArrayTypeMismatchException("Creating CountArray from an array requires the type to be nullable.");
        array = new T[arr.Length];
        Count = 0;
        for(int i = 0; i < array.Length; i++) 
            if(arr[i] is null)
            {
                array[Count] = arr[i];
                Count ++;
            }
    }
    
    public void Add(T item)
    {
        if(Count >= array.Length)
            throw new IndexOutOfRangeException("CountArray already full. Start with a larger size, or clear it.");
        array[Count] = item;
        Count++;
    }

    ///<summary>
    ///Resets all entries in the array to default(T), resets Count to 0.
    ///Works for all types.
    ///</summary>
    public void Clear()
    {
        for(int i = 0; i < array.Length; i++) //all values need to be reset. just resetting count would be funny, but keeps stuff in memory we dont need anymore.
            array[i] = default(T); //not allowed to just write null because .NET type constraints are WACKY!
        Count = 0;
    }

    ///<summary>
    ///Reduce the CountArray to a normal array with Count entries.
    ///</summary>
    public T[] ToTinyArray()
    {
        T[] x = new T[Count];
        for(int i = 0; i < Count; i++)
            x[i] = array[i];
        return x;
    }

    ///<summary>
    ///Reduce the array to one of the underlying value type with Count entries. ONLY REQUIRED FOR STRUCT?-TYPES
    ///E.g. CountArray<int?> to int[]
    ///</summary>
    public VType[] ToTinyValueTypeArray<VType>() where VType : struct //Why would anyone ever need this??? Gonna leave it in here because it's funny!
    {
        if(Nullable.GetUnderlyingType(typeof(T)) != typeof(VType))
            throw new ArrayTypeMismatchException($"Type mismatch: '{typeof(VType)}' and '{Nullable.GetUnderlyingType(typeof(T))}'");
        VType[] x = new VType[Count];
        for(int i = 0; i < Count; i++)
            x[i] = (array[i] as VType?).Value;
        return x;
    }
}
