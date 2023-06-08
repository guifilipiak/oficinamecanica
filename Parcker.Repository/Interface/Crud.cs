using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Interface
{
    public interface Crud
    {
        T GetById<T>(int id) where T : class, new();
        System.Linq.IQueryable<T> All<T>() where T : class, new();
        void Add<T>(T item) where T : class, new();
        void Update<T>(T item) where T : class, new();
        void SaveOrUpdate<T>(T item) where T : class, new();
        T Merge<T>(T item) where T : class, new();
        void Delete<T>(T item) where T : class, new();
        void Delete<T>(ICollection<T> itens) where T : class, new();
        void Delete<T>(Expression<Func<T, bool>> expression) where T : class, new();
    }
}
