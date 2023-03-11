using FluentAssertions;
using Karaoke.Api.Features.Songs.Upload;
using Microsoft.AspNetCore.Http;

namespace Karaoke.Api.Tests;

public class OpenXmlWrapperTests
{
    private readonly OpenXmlWrapper _openXmlWrapper;

    public OpenXmlWrapperTests()
    {
        _openXmlWrapper = new OpenXmlWrapper();
    }
    [Fact]
    public void ShouldReturnError_WhenFileFormatIsInvalid()
    {
        var stream = new MemoryStream();
        var test = new FormFile(stream, 0 , stream.Length, "name", "name.pdf");
        var response = _openXmlWrapper.ParseLines(test);
        response.IsT1.Should().BeTrue();
        response.AsT1.Message.Should().Be($"Document extension should be docx - received file with name {test.FileName}");
    }
    
    [Fact]
    public void ShouldReturnError_WhenStreamIsEmpty()
    {
        var stream = new MemoryStream();
        var test = new FormFile(stream, 0 , stream.Length, "name", "name.docx");
        var response = _openXmlWrapper.ParseLines(test);
        response.IsT1.Should().BeTrue();
        response.AsT1.Message.Should().Be("Stream cannot be empty");
    }
    
    [Fact]
    public void ShouldReturnError_WhenDocumentIsInvalid()
    {
        var stream = new MemoryStream();
        using var sw = new StreamWriter(stream);
        sw.Write("aaa");
        sw.Flush();
        var test = new FormFile(stream, 0 , stream.Length, "name", "name.docx");
        var response = _openXmlWrapper.ParseLines(test);
        response.IsT1.Should().BeTrue();
        response.AsT1.Message.Should().Be("Invalid document");
    }
}