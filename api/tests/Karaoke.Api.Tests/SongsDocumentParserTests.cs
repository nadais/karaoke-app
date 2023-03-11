using FluentAssertions;
using Karaoke.Api.Features.Songs.Upload;

namespace Karaoke.Api.Tests;

public class SongsDocumentParserTests
{
    private readonly SongsDocumentParser _sut;

    public SongsDocumentParserTests()
    {
        _sut = new SongsDocumentParser();
    }

    [Fact]
    public void ShouldReturnEmptySongs_WhenNoTablesAreProvided()
    {
        var response = _sut.ParseDocumentLines(new List<InternalTable>(), "catalog");
        response.IsT0.Should().BeTrue();
        response.AsT0.Should().BeEmpty();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldReturnError_WhenNoCatalogNameIsProvided(string catalogName)
    {
        var response = _sut.ParseDocumentLines(new List<InternalTable>(), catalogName);
        response.IsT1.Should().BeTrue();
        response.AsT1.Message.Should().Be("Catalog name cannot be empty");
    }
    
    [Fact]
    public void ShouldReturnError_WhenHeadersIsNotLength3()
    {
        var table = GetValidInternalTable() with { Headers = new List<string> { "Number" }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT1.Should().BeTrue();
        response.AsT1.Message.Should().Be("Table provided does not have 3 headers");
    }

    [Fact]
    public void ShouldReturnFaultyLines_WhenNonNumberIsPassedToNumberKey()
    {
        var table = GetValidInternalTable() with { Content = new List<List<string>>
        {
            new(){"a1", "aaa", "a"}
        }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT1.Should().BeTrue();
        var t1Response = response.AsT1;
        t1Response.Message.Should().Be("The following lines were faulty");
        
        var content = (List<string>?) t1Response.Content;
        content.Should().HaveCount(1);
        content?[0].Should().Be("Number:a1;Name:aaa;Artist:a");
    }

    [Fact]
    public void ShouldReturnMultipleFaultyLines_WhenMultipleLinesHaveErrors()
    {
        var table = GetValidInternalTable() with { Content = new List<List<string>>
        {
            new(){"a1", "aaa", "a"},
            new(){"a2", "aaa", "a"}
        }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT1.Should().BeTrue();
        var t1Response = response.AsT1;
        t1Response.Message.Should().Be("The following lines were faulty");
        
        var content = (List<string>?) t1Response.Content;
        content.Should().HaveCount(2);
        content?[0].Should().Be("Number:a1;Name:aaa;Artist:a");
        content?[1].Should().Be("Number:a2;Name:aaa;Artist:a");
    }

    [Fact]
    public void ShouldReturnFaultyLines_WhenArtistIsEmpty()
    {
        var table = GetValidInternalTable() with { Content = new List<List<string>>
        {
            new(){"1", "aaa", ""}
        }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT1.Should().BeTrue();
        var t1Response = response.AsT1;
        t1Response.Message.Should().Be("The following lines were faulty");
        
        var content = (List<string>?) t1Response.Content;
        content.Should().HaveCount(1);
        content?[0].Should().Be("Number:1;Name:aaa;Artist:");
    }
    
    [Fact]
    public void ShouldReturnFaultyLines_WhenNameIsEmpty()
    {
        var table = GetValidInternalTable() with { Content = new List<List<string>>
        {
            new(){"1", "", "aaa"}
        }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT1.Should().BeTrue();
        var t1Response = response.AsT1;
        t1Response.Message.Should().Be("The following lines were faulty");
        
        var content = (List<string>?) t1Response.Content;
        content.Should().HaveCount(1);
        content?[0].Should().Be("Number:1;Name:;Artist:aaa");
    }
    
    [Theory]
    [InlineData("aaa   ")]
    [InlineData("   aaa")]
    [InlineData("   aaa   ")]
    public void ShouldReturnArtistTrimmed_WhenArtistHasSpaces(string artistName)
    {
        var table = GetValidInternalTable() with { Content = new List<List<string>>
        {
            new(){"1", "aaa", artistName}
        }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT0.Should().BeTrue();
        var t0Response = response.AsT0;
        t0Response.Should().HaveCount(1);
        t0Response.ElementAt(0).Artist.Should().Be("aaa");
    }
    
    [Theory]
    [InlineData("aaa   ")]
    [InlineData("   aaa")]
    [InlineData("   aaa   ")]
    public void ShouldReturnNameTrimmed_WhenNameHasSpaces(string name)
    {
        var table = GetValidInternalTable() with { Content = new List<List<string>>
        {
            new(){"1", name, "aaa"}
        }};
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT0.Should().BeTrue();
        var t0Response = response.AsT0;
        t0Response.Should().HaveCount(1);
        t0Response.ElementAt(0).Artist.Should().Be("aaa");
    }
    
    [Theory]
    [InlineData("Title", "General")]
    [InlineData("Titre", "General")]
    [InlineData("Specific category", "Specific category")]
    [InlineData("   category with spaces  ", "category with spaces")]
    public void ShouldReturnExpectedCategoryName(string categoryName, string expectedCategoryName)
    {
        var table = GetValidInternalTable();
        table.Headers[1] = categoryName;
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT0.Should().BeTrue();
        var t0Response = response.AsT0;
        t0Response.Should().HaveCount(1);
        t0Response.ElementAt(0).Categories.Should().HaveCount(1);
        t0Response.ElementAt(0).Categories[0].Should().Be(expectedCategoryName);
    }
    
    [Fact]
    public void ShouldMergeCategories_WhenSameSongAppearsInMultipleTables()
    {
        var row = new List<string> { "1", "aaa", "aaa" };
        var firstTable = new InternalTable(new List<string> { "Number", "Category 1", "Artist" }, new List<List<string>>
        {
            row
        });
        var secondTable = new InternalTable(new List<string> { "Number", "Category 2", "Artist" }, new List<List<string>>
        {
            row
        });
        var response = _sut.ParseDocumentLines(new List<InternalTable>{firstTable, secondTable}, "catalog");
        response.IsT0.Should().BeTrue();
        var t0Response = response.AsT0;
        t0Response.Should().HaveCount(1);
        t0Response.ElementAt(0).Categories.Should().HaveCount(2);
        t0Response.ElementAt(0).Categories[0].Should().Be("Category 1");
        t0Response.ElementAt(0).Categories[1].Should().Be("Category 2");
    }
    
    [Fact]
    public void ShouldReturnExpectedCatalogName()
    {
        var table = GetValidInternalTable();
        var response = _sut.ParseDocumentLines(new List<InternalTable>{ table}, "catalog");
        response.IsT0.Should().BeTrue();
        var t0Response = response.AsT0;
        t0Response.Should().HaveCount(1);
        t0Response.ElementAt(0).Catalogs.Should().HaveCount(1);
        t0Response.ElementAt(0).Catalogs[0].Should().Be("catalog");
    }

    private InternalTable GetValidInternalTable()
    {
        return new InternalTable(new List<string> { "Number", "Name", "Artist" }, new List<List<string>>
        {
            new() { "1", "aaa", "aaa" }
        });
    }
}