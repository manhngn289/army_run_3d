namespace Framework.ObjectPooling
{
    public interface IPoolingObject
    {
        void OnSpawned();
        void OnDespawned();
    }
}