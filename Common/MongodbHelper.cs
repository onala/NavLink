using MongoDB.Driver;
using Serilog;
using System.Linq.Expressions;

namespace HaskyNavLink.Common;

/// <summary>
/// MongoDB 数据操作类
/// </summary>
/// <typeparam name="T"></typeparam>
internal class MongoDbHelper<T> where T : class
{

    //数据库
    private readonly static IMongoDatabase db = DB<T>.CreateDB();

    //数据集
    private readonly IMongoCollection<T> clt = db.GetCollection<T>(typeof(T).Name);

 

    /// <summary>
    /// 获取数据集
    /// </summary>
    /// <returns></returns>
    public IMongoCollection<T> GetCollection()
    {
        return clt ?? db.GetCollection<T>(typeof(T).Name);
    }


    /// <summary>
    /// 创建单个实体
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<T?> CreateAsync(T entity)
    {
        try
        {
            await clt.InsertOneAsync(entity);
            return entity;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            // throw ex;
            return null;
        }
    }

    /// <summary>
    /// 创建多个实体
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public async Task<IEnumerable<T>?> CreateRangeAsync(IEnumerable<T> entities)
    {
        try
        {
            await clt.InsertManyAsync(entities);
            return entities;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 读取单个实体（通过 ID）
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<List<T>?> GetByIdAsync(Expression<Func<T, bool>> func)
    {
        var rtn = await clt.FindAsync(func);
        return rtn.ToList();
    }


    /// <summary>
    /// 读取所有实体
    /// </summary>
    /// <returns></returns>
    public List<T> GetAllList()
    {
        try
        {
            var rtn = clt.Find(f => true);
            return rtn.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return [];
        }
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="func"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<long> UpdateAsync(Expression<Func<T, bool>> func, UpdateDefinition<T> entity)
    {
        var rtn = await clt.UpdateOneAsync(func, entity);

        return rtn.ModifiedCount;
    }

    /// <summary>
    ///  更新实体
    /// </summary>
    /// <param name="func"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<long> UpdateEntityAsync(Expression<Func<T, bool>> func, T entity)
    {
        var rtn = await clt.ReplaceOneAsync(func, entity);
        return rtn.ModifiedCount;
    }



    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<long> DeleteAsync(Expression<Func<T, bool>> func)
    {
        var rtn = await clt.DeleteOneAsync(func);
        return rtn.DeletedCount;
    }

    /// <summary>
    /// 向文档的数组字段中添加子文档（用于 CommList 等数组字段）
    /// </summary>
    /// <param name="filter">筛选要更新的文档的条件</param>
    /// <param name="arrayField">数组字段的选择表达式，例如 x => x.CommList</param>
    /// <param name="subDocument">要添加的子文档</param>
    /// <returns>返回受影响的行数</returns>
    public async Task<long> AddSubDocumentAsync<TSub>(
        Expression<Func<T, bool>> filter,
       string arrayFieldName,
        TSub subDocument)
    {
        try
        {
            // 使用 Push 操作符向数组字段添加子文档
            var update = Builders<T>.Update.Push(arrayFieldName, subDocument);
            var result = await clt.UpdateOneAsync(filter, update);
            return result.ModifiedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "向数组字段添加子文档失败");
            return 0;
        }
    }



}
