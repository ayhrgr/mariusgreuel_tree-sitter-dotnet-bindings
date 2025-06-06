using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSitter.Tests;

[TestClass]
public class MarkdownInlineTests
{
    readonly Language _markdown = new("tree-sitter-markdown-inline", "tree_sitter_markdown_inline");
    Parser _parser = null!;
    Tree? _tree;

    [TestInitialize]
    public void TestInitialize()
    {
        _parser = new Parser(_markdown);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _tree?.Dispose();
        _parser.Dispose();
        _markdown.Dispose();
    }

    private static string RemoveWhitespace(string input)
    {
        return input.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("\t", "");
    }

}
