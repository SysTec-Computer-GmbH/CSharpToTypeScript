using System;
using CSharpToTypeScript.Core.Options;

namespace CSharpToTypeScript.Core.Models.TypeNodes;

internal class Time : TypeNode
{
    public override string WriteTypeScript(CodeConversionOptions options, Context context)
        => "string";
}