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
        array = new T[arr.Length];
        for(int i = 0; i < array.Length; i++) 
            if(arr[i].Equals(default(T)) == false)
            {
                array[Count] = arr[i];
                Count ++;
            }
    }
    
    public void Add(T item)
    {
        array[Count] = item;
        Count++;
    }
}
