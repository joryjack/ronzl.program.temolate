﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Ronzl.Framework.Contract;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;

namespace Ronzl.Framework.DAL
{
    /// <summary>
    /// DAL基类，实现Repository通用泛型数据访问模式
    /// </summary>
    public class DbContextBase : DbContext, IDataRepository, IDisposable
    {
        public DbContextBase(string connectionString)
        {
            //var objectContext = (this as IObjectContextAdapter).ObjectContext;
            //objectContext.CommandTimeout = 500;

            this.Database.Connection.ConnectionString = connectionString;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbContextBase(string connectionString, IAuditable auditLogger)
            : this(connectionString)
        {
            this.AuditLogger = auditLogger;
        }

        public IAuditable AuditLogger { get; set; }

        public T UpdateEntityFields<T>(T entity, List<string> fileds) where T : ModelBase
        {
            if (entity != null && fileds != null)
            {
                this.Set<T>().Attach(entity);
                var SetEntry = ((IObjectContextAdapter)this).ObjectContext.
                    ObjectStateManager.GetObjectStateEntry(entity);
                foreach (var t in fileds)
                {
                    SetEntry.SetModifiedProperty(t);
                }
            }
            this.SaveChanges();
            return entity;
        }

        public T Update<T>(T entity) where T : ModelBase
        {
            var set = this.Set<T>();
            set.Attach(entity);
            this.Entry<T>(entity).State = EntityState.Modified;
            this.SaveChanges();

            return entity;
        }

        public T Insert<T>(T entity) where T : ModelBase
        {
            this.Set<T>().Add(entity);
            this.SaveChanges();
            return entity;
        }

        public void Delete<T>(T entity) where T : ModelBase
        {
            this.Entry<T>(entity).State = EntityState.Deleted;
            this.SaveChanges();
        }

        public T Find<T>(params object[] keyValues) where T : ModelBase
        {
            return this.Set<T>().Find(keyValues);
        }

        public List<T> FindAll<T>(Expression<Func<T, bool>> conditions = null) where T : ModelBase
        {
            if (conditions == null)
                return this.Set<T>().ToList();
            else
                return this.Set<T>().Where(conditions).ToList();
        }

        public PagedList<T> FindAllByPage<T, S>(Expression<Func<T, bool>> conditions, Expression<Func<T, S>> orderBy, int pageSize, int pageIndex) where T : ModelBase
        {
            var queryList = conditions == null ? this.Set<T>() : this.Set<T>().Where(conditions) as IQueryable<T>;

            return queryList.OrderByDescending(orderBy).ToPagedList(pageIndex, pageSize);
        }

        public override int SaveChanges()
        {
            // this.WriteAuditLog();

            var result = base.SaveChanges();
            return result;
        }

        internal void WriteAuditLog()
        {
            if (this.AuditLogger == null)
                return;

            foreach (var dbEntry in this.ChangeTracker.Entries<ModelBase>().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                var auditableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(AuditableAttribute), false).SingleOrDefault() as AuditableAttribute;
                if (auditableAttr == null)
                    continue;

                var operaterName = WCFContext.Current.Operater.Name;

                Task.Factory.StartNew(() =>
                {
                    var tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
                    string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
                    var moduleName = dbEntry.Entity.GetType().FullName.Split('.').Skip(1).FirstOrDefault();

                    this.AuditLogger.WriteLog(dbEntry.Entity.id, operaterName, moduleName, tableName, dbEntry.State.ToString(), dbEntry.Entity);
                });
            }

        }
    }
}
