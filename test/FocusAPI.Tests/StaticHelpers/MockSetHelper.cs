using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FocusAPI.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FocusAPI.Tests.StaticHelpers
{
    internal static class MockSetHelper
    {
        internal static void SetupEntities<T>(List<T> sourceList, Mock<FocusAPIContext> context, Expression<Func<FocusAPIContext, DbSet<T>>> setupExpression) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator);
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);

            context.Setup(setupExpression).Returns(dbSet.Object);
        }
    }
}
