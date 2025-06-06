using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSitter.Tests;

[TestClass]
public class MarkdownTests
{
    readonly Language _markdown = new("markdown");
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

    /// <summary>
    /// tree-sitter-native/tree-sitter-markdown/tree-sitter-markdown/test/corpus/extension_minus_metadata.txt
    /// </summary>
    [TestMethod]
    public void TestExtensionMinusMetadata()
    {
        string inputText = @"---
title:  'This is the title: it contains a colon'
author:
- Author One
- Author Two
keywords: [nothing, nothingness]
abstract: |
  This is the abstract.

  It consists of two paragraphs.
---
";
        string expectedSyntaxTree = "(document (minus_metadata))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(expectedSyntaxTree, actualSyntaxTree);
    }

}
