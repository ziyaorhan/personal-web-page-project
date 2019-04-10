using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PersonalWebPage_MVC.Db.Abstract
{
    public class DatabaseOperations<TEntity, TContext>
       where TEntity : class, new()

       where TContext : DbContext, new()
    {
        public bool Add(TEntity entity)
        {
            bool returnValue = false;
            try
            {
                using (TContext context = new TContext())
                {
                    context.Set<TEntity>().Add(entity);
                    context.SaveChanges();
                }
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public bool Update(TEntity newEntityValue)
        {
            bool returnValue = false;
            try
            {
                using (TContext context = new TContext())
                {
                    int id = Convert.ToInt32(context.Entry(newEntityValue).Property("ID").CurrentValue);
                    var entity = context.Set<TEntity>().Find(id);
                    context.Entry(entity).CurrentValues.SetValues(newEntityValue);
                    context.SaveChanges();
                }
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public bool Delete(TEntity entity)
        {
            bool returnValue = false;
            try
            {
                using (TContext context = new TContext())
                {
                    var deletedEntity = context.Entry(entity);
                    deletedEntity.State = EntityState.Deleted;
                    context.SaveChanges();
                }
                returnValue = true;
            }
            catch (Exception)
            {

                returnValue = false;
            }
            return returnValue;
        }

        public List<TEntity> GetAll()
        {
            try
            {
                using (TContext context = new TContext())
                {
                    return context.Set<TEntity>().ToList();
                }
            }
            catch
            {
                return null;
            }
        }
        //Kontrol edilmedi.
        public TEntity GetById(object id)
        {
            try
            {
                using (TContext context = new TContext())
                {
                    return context.Set<TEntity>().Find(id);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}