using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cell.Blazor._Core.Class;
using Cell.Blazor.Data.Class;

namespace Cell.Blazor._Core.Static
{
    public static class DataOperations
    {
        public static IEnumerable Execute(IEnumerable dataSource, DataManagerRequest query) => EnumerableOperation.Execute(dataSource, query);

        public static IEnumerable PerformSorting(
          IEnumerable dataSource,
          List<SortedColumn> sortedColumns)
        {
            return EnumerableOperation.PerformSorting(dataSource, sortedColumns);
        }

        public static IEnumerable PerformSorting(
          IEnumerable dataSource,
          List<Sort> sortedColumns)
        {
            return EnumerableOperation.PerformSorting(dataSource, sortedColumns);
        }

        public static IEnumerable PerformFiltering(
          IEnumerable dataSource,
          List<WhereFilter> whereFilter,
          string condition)
        {
            return EnumerableOperation.PerformFiltering(dataSource, whereFilter, condition);
        }

        public static IEnumerable PerformSearching(
          IEnumerable dataSource,
          List<SearchFilter> searchFilter)
        {
            return EnumerableOperation.PerformSearching(dataSource, searchFilter);
        }

        public static IEnumerable PerformSkip(IEnumerable dataSource, int skip) => EnumerableOperation.PerformSkip(dataSource, skip);

        public static IEnumerable PerformTake(IEnumerable dataSource, int take) => EnumerableOperation.PerformTake(dataSource, take);

        public static IEnumerable PerformGrouping(
          IEnumerable dataSource,
          List<string> grouped)
        {
            return EnumerableOperation.PerformGrouping(dataSource, grouped);
        }

        public static IEnumerable<T> Execute<T>(
          IEnumerable<T> dataSource,
          DataManagerRequest query)
        {
            return (IEnumerable<T>)QueryableOperation.Execute<T>(dataSource.AsQueryable<T>(), query);
        }

        public static IEnumerable<T> PerformSkip<T>(IEnumerable<T> dataSource, int skip) => (IEnumerable<T>)QueryableOperation.PerformSkip<T>(dataSource.AsQueryable<T>(), skip);

        public static IEnumerable<T> PerformTake<T>(IEnumerable<T> dataSource, int take) => (IEnumerable<T>)QueryableOperation.PerformTake<T>(dataSource.AsQueryable<T>(), take);

        public static IEnumerable PerformGrouping<T>(
          IEnumerable<T> dataSource,
          List<string> grouped)
        {
            return (IEnumerable)QueryableOperation.PerformGrouping<T>(dataSource.AsQueryable<T>(), grouped);
        }

        public static IEnumerable<T> PerformSorting<T>(
          IEnumerable<T> dataSource,
          List<SortedColumn> sortedColumns)
        {
            return (IEnumerable<T>)QueryableOperation.PerformSorting<T>(dataSource.AsQueryable<T>(), sortedColumns);
        }

        public static IEnumerable<T> PerformSorting<T>(
          IEnumerable<T> dataSource,
          List<Sort> sortedColumns)
        {
            return (IEnumerable<T>)QueryableOperation.PerformSorting<T>(dataSource.AsQueryable<T>(), sortedColumns);
        }

        public static IEnumerable PerformSelect(IEnumerable dataSource, List<string> select) => (IEnumerable)QueryableOperation.PerformSelect(dataSource.AsQueryable(), select);

        public static IEnumerable<T> PerformSearching<T>(
          IEnumerable<T> dataSource,
          List<SearchFilter> searchFilter)
        {
            return (IEnumerable<T>)QueryableOperation.PerformSearching<T>(dataSource.AsQueryable<T>(), searchFilter);
        }

        public static IEnumerable<T> PerformFiltering<T>(
          IEnumerable<T> dataSource,
          List<WhereFilter> whereFilter,
          string condition)
        {
            return (IEnumerable<T>)QueryableOperation.PerformFiltering<T>(dataSource.AsQueryable<T>(), whereFilter, condition);
        }

        public static IQueryable<T> Execute<T>(
          IQueryable<T> dataSource,
          DataManagerRequest query)
        {
            return QueryableOperation.Execute<T>(dataSource, query);
        }

        public static IQueryable PerformGrouping<T>(
          IQueryable<T> dataSource,
          List<string> grouped)
        {
            return QueryableOperation.PerformGrouping<T>(dataSource, grouped);
        }

        public static IQueryable<T> PerformSorting<T>(
          IQueryable<T> dataSource,
          List<SortedColumn> sortedColumns)
        {
            return QueryableOperation.PerformSorting<T>(dataSource, sortedColumns);
        }

        public static IQueryable<T> PerformSorting<T>(
          IQueryable<T> dataSource,
          List<Sort> sortedColumns)
        {
            return QueryableOperation.PerformSorting<T>(dataSource, sortedColumns);
        }

        public static IQueryable<T> PerformSkip<T>(IQueryable<T> dataSource, int skip) => QueryableOperation.PerformSkip<T>(dataSource, skip);

        public static IQueryable<T> PerformTake<T>(IQueryable<T> dataSource, int take) => QueryableOperation.PerformTake<T>(dataSource, take);

        public static IQueryable<T> PerformSearching<T>(
          IQueryable<T> dataSource,
          List<SearchFilter> searchFilter)
        {
            return QueryableOperation.PerformSearching<T>(dataSource, searchFilter);
        }

        public static IQueryable<T> PerformFiltering<T>(
          IQueryable<T> dataSource,
          List<WhereFilter> whereFilter,
          string condition)
        {
            return QueryableOperation.PerformFiltering<T>(dataSource, whereFilter, condition);
        }

        public static IQueryable PerformSelect<T>(
          IQueryable<T> dataSource,
          List<string> select)
        {
            return QueryableOperation.PerformSelect((IQueryable)dataSource, select);
        }
    }
}