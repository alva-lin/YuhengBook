using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.Infrastructure.Data.Config;

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(e => e.Name)
           .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(e => e.Description)
           .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);
    }
}

public class ChapterEntityTypeConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.Property(e => e.Title)
           .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasOne(c => c.Book)
           .WithMany(b => b.Chapters)
           .HasForeignKey(c => c.BookId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Restrict)
            ;

        builder.HasOne(c => c.Detail)
           .WithOne()
           .HasForeignKey<ChapterDetail>(d => d.Id);

        builder.ToTable(nameof(Chapter).ToSnakeCase());

        builder.Property(c => c.Order)
           .HasColumnName(nameof(Chapter.Order).ToSnakeCase());

        builder.Property(c => c.Title)
           .HasColumnName(nameof(Chapter.Title).ToSnakeCase());

        builder.Navigation(c => c.Detail).IsRequired();
    }
}

public class DetailedChapterEntityTypeConfiguration : IEntityTypeConfiguration<ChapterDetail>
{
    public void Configure(EntityTypeBuilder<ChapterDetail> builder)
    {
        builder.ToTable(nameof(Chapter).ToSnakeCase());
    }
}
