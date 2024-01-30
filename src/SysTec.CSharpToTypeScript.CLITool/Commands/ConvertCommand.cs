using System;
using System.IO;
using System.Linq;
using CSharpToTypeScript.CLITool.Conventions;
using CSharpToTypeScript.CLITool.Utilities;
using CSharpToTypeScript.Core.Options;
using CSharpToTypeScript.Core.Services;
using McMaster.Extensions.CommandLineUtils;

namespace CSharpToTypeScript.CLITool.Commands;

[Command(Name = "dotnet cs2ts",
    Description = "Convert C# Models, ViewModels and DTOs into their TypeScript equivalents")]
[Subcommand(typeof(InitializeCommand))]
public class ConvertCommand : CommandBase
{
    private readonly ICodeConverter _codeConverter;
    private readonly IFileNameConverter _fileNameConverter;

    public ConvertCommand(ICodeConverter codeConverter, IFileNameConverter fileNameConverter)
    {
        _codeConverter = codeConverter;
        _fileNameConverter = fileNameConverter;
    }

    public CodeConversionOptions CodeConversionOptions
        => new(!SkipExport, UseTabs, TabSize, ConvertDatesTo, ConvertNullablesTo,
            !PreserveCasing, !PreserveInterfacePrefix,
            ImportGeneration, UseKebabCase, AppendModelSuffix, QuotationMark, AppendNewLine);

    public void OnExecute()
    {
        if (AngularMode) AngularConventions.Override(this);

        if (ClearOutputDirectory && !string.IsNullOrWhiteSpace(Output)
                                 && Directory.Exists(Output))
            Directory.Delete(Output, true);

        foreach (string input in Input)
            if (input.EndsWithFileExtension() && File.Exists(input))
                OnInputIsFile(input);
            else if (!input.EndsWithFileExtension() && Directory.Exists(input)) OnInputIsDirectory(input);
    }

    private void OnInputIsFile(string input)
    {
        string content = File.ReadAllText(input);
        string converted = _codeConverter.ConvertToTypeScript(content, CodeConversionOptions);
        string outputPath = GetOutputFilePath(input, Output, CodeConversionOptions);

        CreateOrUpdateFile(outputPath, converted, PartialOverride);
    }

    private void OnInputIsDirectory(string input)
    {
        var files = FileSystem.GetFilesWithExtension(input, "cs")
            .Select(f => new
            {
                OutputPath = GetOutputFilePath(f, Output, CodeConversionOptions),
                Content = _codeConverter.ConvertToTypeScript(File.ReadAllText(f), CodeConversionOptions)
            })
            .Where(f => !string.IsNullOrWhiteSpace(f.Content))
            .GroupBy(f => f.OutputPath)
            .Select(g => g.First());

        foreach (var file in files) CreateOrUpdateFile(file.OutputPath, file.Content, PartialOverride);
    }

    private void CreateOrUpdateFile(string path, string content, bool partialOverride)
    {
        Directory.CreateDirectory(path.ContainingDirectory());

        if (partialOverride)
            content = Marker.Update(File.Exists(path) ? File.ReadAllText(path) : string.Empty, content);

        File.WriteAllText(path, content);
    }

    private string GetOutputFilePath(string input, string output, ModuleNameConversionOptions options)
    {
        return !input.EndsWithFileExtension()
            ? throw new ArgumentException("Input should end with file extension.")
            : output?.EndsWithFileExtension() == true
                ? output
                : !string.IsNullOrWhiteSpace(output)
                    ? Path.Join(output, _fileNameConverter.ConvertToTypeScript(input, options))
                    : Path.Join(input.ContainingDirectory(), _fileNameConverter.ConvertToTypeScript(input, options));
    }
}