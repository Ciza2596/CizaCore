using System.Linq;

namespace CizaCore
{
    public static class ArrayExtension
    {
        public static T[] ToArrayWithoutSameItems<T>(this T[] source, T[] removedItems)
        {
            var newSource = source.ToHashSet();
            
            foreach (T item in removedItems)
                newSource.Remove(item);
            
            return newSource.ToArray();
        }
    }
}