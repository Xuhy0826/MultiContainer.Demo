using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Core
{
    public interface ITransaction
    {
        //开始事务
        Task<IDbContextTransaction> BeginTransactionAsync();

        //提交事务
        Task CommitTransactionAsync(IDbContextTransaction transaction);

        //回滚事务
        void RollbackTransaction();

        //获取当前事务
        IDbContextTransaction GetCurrentTransaction();

        //事务是否开启
        bool HasActiveTransaction { get; }
    }
}
