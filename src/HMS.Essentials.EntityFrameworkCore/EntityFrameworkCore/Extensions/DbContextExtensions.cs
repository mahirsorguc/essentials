using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace HMS.Essentials.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for Entity Framework Core DbContext configuration.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Configures all string properties in the model with a maximum length.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="maxLength">The maximum length for string properties.</param>
    /// <param name="includeNavigations">Whether to include navigation properties.</param>
    public static void ConfigureStringMaxLength(
        this ModelBuilder modelBuilder,
        int maxLength = 256,
        bool includeNavigations = false)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(string));

            foreach (var property in properties)
            {
                if (!includeNavigations && property.IsForeignKey())
                {
                    continue;
                }

                if (property.GetMaxLength() == null)
                {
                    property.SetMaxLength(maxLength);
                }
            }
        }
    }

    /// <summary>
    /// Configures all decimal properties in the model with precision and scale.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="precision">The precision for decimal properties.</param>
    /// <param name="scale">The scale for decimal properties.</param>
    public static void ConfigureDecimalPrecision(
        this ModelBuilder modelBuilder,
        int precision = 18,
        int scale = 2)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in properties)
            {
                property.SetPrecision(precision);
                property.SetScale(scale);
            }
        }
    }

    /// <summary>
    /// Configures table names to use a specific naming convention.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="nameTransform">Function to transform entity type name to table name.</param>
    public static void ConfigureTableNames(
        this ModelBuilder modelBuilder,
        Func<string, string> nameTransform)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.BaseType == null)
            {
                var tableName = nameTransform(entityType.ClrType.Name);
                entityType.SetTableName(tableName);
            }
        }
    }

    /// <summary>
    /// Configures table names to use plural form.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void ConfigurePluralTableNames(this ModelBuilder modelBuilder)
    {
        ConfigureTableNames(modelBuilder, name =>
        {
            // Simple pluralization - can be extended with a proper pluralization library
            if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(0, name.Length - 1) + "ies";
            }
            else if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return name + "es";
            }
            else
            {
                return name + "s";
            }
        });
    }

    /// <summary>
    /// Configures cascade delete behavior for all foreign keys.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="deleteBehavior">The delete behavior to apply.</param>
    public static void ConfigureCascadeDelete(
        this ModelBuilder modelBuilder,
        DeleteBehavior deleteBehavior = DeleteBehavior.Restrict)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = deleteBehavior;
        }
    }

    /// <summary>
    /// Applies a query filter to all entities of a specific type.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to filter by.</typeparam>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="filterExpression">The filter expression.</param>
    public static void ApplyGlobalFilter<TInterface>(
        this ModelBuilder modelBuilder,
        Expression<Func<TInterface, bool>> filterExpression)
    {
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(TInterface).IsAssignableFrom(e.ClrType))
            .ToList();

        foreach (var entityType in entities)
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var visitor = new ParameterReplacerVisitor(filterExpression.Parameters[0], parameter);
            var body = visitor.Visit(filterExpression.Body);
            
            if (body != null)
            {
                var lambdaExpression = Expression.Lambda(body, parameter);
                entityType.SetQueryFilter(lambdaExpression);
            }
        }
    }

    /// <summary>
    /// Helper class for replacing expression parameters.
    /// </summary>
    private class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacerVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }

    /// <summary>
    /// Configures all DateTime properties to use UTC.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void ConfigureDateTimeAsUtc(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in properties)
            {
                property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
            }
        }
    }

    /// <summary>
    /// Gets all entity types that implement a specific interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>Collection of entity types.</returns>
    public static IEnumerable<IMutableEntityType> GetEntitiesOfType<TInterface>(this ModelBuilder modelBuilder)
    {
        return modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(TInterface).IsAssignableFrom(e.ClrType));
    }

    /// <summary>
    /// Disables cascade delete for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="modelBuilder">The model builder.</param>
    public static void DisableCascadeDelete<TEntity>(this ModelBuilder modelBuilder)
        where TEntity : class
    {
        var entityType = modelBuilder.Model.FindEntityType(typeof(TEntity));
        
        if (entityType != null)
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
