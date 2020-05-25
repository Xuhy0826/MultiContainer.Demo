namespace Domain.Abstractions
{
    public interface IEntity
    {
        object[] Keys { get; }
    }
    /// <summary>
    /// 实体接口
    /// </summary>
    /// <typeparam name="TKey">唯一标识的类型</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; }
    }
}
