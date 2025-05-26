namespace FS.CORE
{
    public interface ISingleton<T> where T:class ,new()
    {
        private static T _instance ;
        static T Instance { get; }
    }
}