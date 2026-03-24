using HaskyNavLink.Models;
using MongoDB.Driver;

namespace HaskyNavLink.Common;


/// <summary>
/// 数据库操作
/// </summary>
/// <typeparam name="T"></typeparam>
internal class DB<T> where T : class
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    private static MongoDbHelper<T>? _db;

    /// <summary>
    /// 数据库操作
    /// </summary>
    public static MongoDbHelper<T> Db => _db ??= new();




    /// <summary>
    /// MongoDb连接池
    /// </summary>
    private static IMongoDatabase? mongodb;

    /// <summary>
    /// 创建Mongodb  //"mongodb://admin:Cshsj#2026@192.168.25.211:27017" //"HaskyNav"
    /// </summary>
    /// <returns></returns>
    public static IMongoDatabase CreateDB() => mongodb ??= (new MongoClient(SysCache.ConnStr))
         .GetDatabase(SysCache.DbName);


}