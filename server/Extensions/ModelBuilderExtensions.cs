using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ViSingers.Server.Extensions;
public static class EntityTypeBuilderExtensions
{
    public static void ConfigurePropertyAsJson<TEntity, TResult>(this EntityTypeBuilder<TEntity> entityTypeBuilder, Expression<Func<TEntity, TResult>> propertyExpression)
        where TEntity : class
        where TResult : class, new()
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = false };
        var dictionaryConverter = new ValueConverter<TResult, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions) null),
            v => JsonSerializer.Deserialize<TResult>(v, (JsonSerializerOptions) null) ?? new TResult());
        entityTypeBuilder
            .Property(propertyExpression)
            .HasConversion(dictionaryConverter);
    }
}