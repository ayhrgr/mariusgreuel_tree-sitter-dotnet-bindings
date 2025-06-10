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
    }

    private static string RemoveWhitespace(string input)
    {
        return input.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("\t", "");
    }

    /// <summary>
    /// EXTENSION_MINUS_METADATA - https://pandoc.org/MANUAL.html#extension-yaml_metadata_block
    /// From: tree-sitter-markdown/test/corpus/extension_minus_metadata.txt
    /// </summary>
    [TestMethod("EXTENSION_MINUS_METADATA - https://pandoc.org/MANUAL.html#extension-yaml_metadata_block")]
    [TestCategory("extension_minus_metadata")]
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
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 198 - https://github.github.com/gfm/#example-198
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 198 - https://github.github.com/gfm/#example-198")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample198()
    {
        string inputText = @"| foo | bar |
| --- | --- |
| baz | bim |
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell)
        (pipe_table_delimiter_cell))
      (pipe_table_row
        (pipe_table_cell)
        (pipe_table_cell)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 199 - https://github.github.com/gfm/#example-199
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 199 - https://github.github.com/gfm/#example-199")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample199()
    {
        string inputText = @"| abc | defghi |
:-: | -----------:
bar | baz
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell
          (pipe_table_align_left)
          (pipe_table_align_right))
        (pipe_table_delimiter_cell
          (pipe_table_align_right)))
      (pipe_table_row
        (pipe_table_cell)
        (pipe_table_cell)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 200 - https://github.github.com/gfm/#example-200
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// Note: Input text uses \| for escaped pipes as per GFM spec.
    /// </summary>
    [TestMethod("Example 200 - https://github.github.com/gfm/#example-200")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample200()
    {
        string inputText = @"| f\|oo  |
| ------ |
| b `\|` az |
| b **\|** im |
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell))
      (pipe_table_row
        (pipe_table_cell))
      (pipe_table_row
        (pipe_table_cell)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 201 - https://github.github.com/gfm/#example-201
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 201 - https://github.github.com/gfm/#example-201")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample201()
    {
        string inputText = @"| abc | def |
| --- | --- |
| bar | baz |
> bar
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell)
        (pipe_table_delimiter_cell))
      (pipe_table_row
        (pipe_table_cell)
        (pipe_table_cell)))
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 202 - https://github.github.com/gfm/#example-202
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 202 - https://github.github.com/gfm/#example-202")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample202()
    {
        string inputText = @"| abc | def |
| --- | --- |
| bar | baz |
bar

bar
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell)
        (pipe_table_delimiter_cell))
      (pipe_table_row
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_row
        (pipe_table_cell)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 203 - https://github.github.com/gfm/#example-203
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 203 - https://github.github.com/gfm/#example-203")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample203()
    {
        string inputText = @"| abc | def |
| --- |
| bar |
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 204 - https://github.github.com/gfm/#example-204
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 204 - https://github.github.com/gfm/#example-204")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample204()
    {
        string inputText = @"| abc | def |
| --- | --- |
| bar |
| bar | baz | boo |
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell)
        (pipe_table_delimiter_cell))
      (pipe_table_row
        (pipe_table_cell))
      (pipe_table_row
        (pipe_table_cell)
        (pipe_table_cell)
        (pipe_table_cell)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 205 - https://github.github.com/gfm/#example-205
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Example 205 - https://github.github.com/gfm/#example-205")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableExample205()
    {
        string inputText = @"| abc | def |
| --- | --- |
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell)
        (pipe_table_delimiter_cell)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #112 - Works with table cells that only contain whitespce
    /// From: tree-sitter-markdown/test/corpus/extension_pipe_table.txt
    /// </summary>
    [TestMethod("Issue #112 - Works with table cells that only contain whitespce")]
    [TestCategory("extension_pipe_table")]
    public void TestPipeTableIssue112WhitespaceCells()
    {
        string inputText = @"| foo | bar |
| --- | --- |
|     | bim |
";
        string expectedSyntaxTree = @"(document
  (section
    (pipe_table
      (pipe_table_header
        (pipe_table_cell)
        (pipe_table_cell))
      (pipe_table_delimiter_row
        (pipe_table_delimiter_cell)
        (pipe_table_delimiter_cell))
      (pipe_table_row
        (pipe_table_cell)
        (pipe_table_cell)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// EXTENSION_PLUS_METADATA
    /// From: tree-sitter-markdown/test/corpus/extension_plus_metadata.txt
    /// </summary>
    [TestMethod("EXTENSION_PLUS_METADATA - From tree-sitter-markdown/test/corpus/extension_plus_metadata.txt")]
    [TestCategory("extension_plus_metadata")]
    public void TestExtensionPlusMetadata()
    {
        string inputText = @"+++
title:  'This is the title: it contains a colon'
author:
- Author One
- Author Two
keywords: [nothing, nothingness]
abstract: |
  This is the abstract.

  It consists of two paragraphs.
+++
";
        string expectedSyntaxTree = @"(document
  (plus_metadata))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// EXTENSION_TASK_LIST - Example 279 - https://github.github.com/gfm/#example-279
    /// From: tree-sitter-markdown/test/corpus/extension_task_list.txt
    /// </summary>
    [TestMethod("EXTENSION_TASK_LIST - Example 279 - https://github.github.com/gfm/#example-279")]
    [TestCategory("extension_task_list")]
    public void TestExtensionTaskListExample279()
    {
        string inputText = @"- [ ] foo
- [x] bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (task_list_marker_unchecked)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (task_list_marker_checked)
        (paragraph
          (inline))))))

";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// EXTENSION_TASK_LIST - Example 280 - https://github.github.com/gfm/#example-280
    /// From: tree-sitter-markdown/test/corpus/extension_task_list.txt
    /// </summary>
    [TestMethod("EXTENSION_TASK_LIST - Example 280 - https://github.github.com/gfm/#example-280")]
    [TestCategory("extension_task_list")]
    public void TestExtensionTaskListExample280()
    {
        string inputText = @"- [x] foo
  - [ ] bar
  - [x] baz
- [ ] bim
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (task_list_marker_checked)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (task_list_marker_unchecked)
            (paragraph
              (inline)
              (block_continuation)))
          (list_item
            (list_marker_minus)
            (task_list_marker_checked)
            (paragraph
              (inline)))))
      (list_item
        (list_marker_minus)
        (task_list_marker_unchecked)
        (paragraph
          (inline))))))

";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// EXTENSION_TASK_LIST - task list item marker … the letter x in either lowercase or UPPERCASE
    /// From: tree-sitter-markdown/test/corpus/extension_task_list.txt
    /// </summary>
    [TestMethod("EXTENSION_TASK_LIST - Marker x can be UPPERCASE")]
    [TestCategory("extension_task_list")]
    public void TestExtensionTaskListMarkerUpperCase()
    {
        string inputText = @"- [ ] foo
- [X] bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (task_list_marker_unchecked)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (task_list_marker_checked)
        (paragraph
          (inline))))))

";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 107 - https://github.github.com/gfm/#example-107
    /// From: tree-sitter-markdown/test/corpus/failing.txt
    /// </summary>
    [TestMethod("Failing - Example 107 - https://github.github.com/gfm/#example-107")]
    [Ignore("Ignoring because the parser inconsistently handles a deeply indented closing code fence when a trailing newline is present. This behavior deviates from the GFM specification (e.g., Example 96) and the expectation of the original test case from tree-sitter-markdown's failing.txt (Example 107), which is also marked as ':skip'.")]
    [TestCategory("failing")]
    public void TestFailingExample107()
    {
        string inputText = @"```
aaa
    ```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #17 - Titles not detected after an empty inner list item (bullet point)
    /// From: tree-sitter-markdown/test/corpus/issues.txt
    /// </summary>
    [TestMethod("Issue #17 - Titles not detected after an empty inner list item (bullet point)")]
    [TestCategory("issues")]
    public void TestIssue17()
    {
        string inputText = @"* a
  * b
  *


# C
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_star)
            (paragraph
              (inline)
              (block_continuation)))
          (list_item
            (list_marker_star)
            (block_continuation)
            (block_continuation))))))
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))"; // <--- Fix this line
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #33 - Fenced code block attributes
    /// From: tree-sitter-markdown/test/corpus/issues.txt
    /// </summary>
    [TestMethod("Issue #33 - Fenced code block attributes")]
    [TestCategory("issues")]
    public void TestIssue33()
    {
        string inputText = @"```{R}
1 + 1
```

```{}
1 + 1
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language))
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #72 - Can't create list item after a list item with a newline and indent
    /// From: tree-sitter-markdown/test/corpus/issues.txt
    /// </summary>
    [TestMethod("Issue #72 - Can't create list item after a list item with a newline and indent")]
    [TestCategory("issues")]
    public void TestIssue72()
    {
        string inputText = @"1. a
1. b
   c
2. d
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline
            (block_continuation))))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #135 - Closing code block fence not recognized when it has trailing space
    /// From: tree-sitter-markdown/test/corpus/issues.txt
    /// </summary>
    [TestMethod("Issue #135 - Closing code block fence not recognized when it has trailing space")]
    [TestCategory("issues")]
    public void TestIssue135()
    {
        string inputText = @"```
// returns 2
globalNS.method1(5, 10);
```    

@example
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 1 - https://github.github.com/gfm/#example-1
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 1 - https://github.github.com/gfm/#example-1")]
    [TestCategory("spec")]
    public void TestSpecExample1()
    {
        string inputText = @"	foo	baz		bim
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 2 - https://github.github.com/gfm/#example-2
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 2 - https://github.github.com/gfm/#example-2")]
    [TestCategory("spec")]
    public void TestSpecExample2()
    {
        string inputText = @"  	foo	baz		bim
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 3 - https://github.github.com/gfm/#example-3
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 3 - https://github.github.com/gfm/#example-3")]
    [TestCategory("spec")]
    public void TestSpecExample3()
    {
        string inputText = @"    a	a
    ὐ	a
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 4 - https://github.github.com/gfm/#example-4
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 4 - https://github.github.com/gfm/#example-4")]
    [TestCategory("spec")]
    public void TestSpecExample4()
    {
        string inputText = @"  - foo

	bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 5 - https://github.github.com/gfm/#example-5
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 5 - https://github.github.com/gfm/#example-5")]
    [TestCategory("spec")]
    public void TestSpecExample5()
    {
        string inputText = @"- foo

		bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 6 - https://github.github.com/gfm/#example-6
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 6 - https://github.github.com/gfm/#example-6")]
    [TestCategory("spec")]
    public void TestSpecExample6()
    {
        string inputText = @">		foo
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (indented_code_block))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 7 - https://github.github.com/gfm/#example-7
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 7 - https://github.github.com/gfm/#example-7")]
    [TestCategory("spec")]
    public void TestSpecExample7()
    {
        string inputText = @"-		foo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 8 - https://github.github.com/gfm/#example-8
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 8 - https://github.github.com/gfm/#example-8")]
    [TestCategory("spec")]
    public void TestSpecExample8()
    {
        string inputText = @"    foo
	bar
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 9 - https://github.github.com/gfm/#example-9
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 9 - https://github.github.com/gfm/#example-9")]
    [TestCategory("spec")]
    public void TestSpecExample9()
    {
        string inputText = @" - foo
   - bar
	 - baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation))
            (list
              (list_item
                (list_marker_minus)
                (paragraph
                  (inline))))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 10 - https://github.github.com/gfm/#example-10
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 10 - https://github.github.com/gfm/#example-10")]
    [TestCategory("spec")]
    public void TestSpecExample10()
    {
        string inputText = @"#	Foo
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))"; // <--- Fix this line
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 11 - https://github.github.com/gfm/#example-11
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 11 - https://github.github.com/gfm/#example-11")]
    [TestCategory("spec")]
    public void TestSpecExample11()
    {
        string inputText = @"*	*	*
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 12 - https://github.github.com/gfm/#example-12
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 12 - https://github.github.com/gfm/#example-12")]
    [TestCategory("spec")]
    public void TestSpecExample12()
    {
        string inputText = @"- `one
- two`
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 13 - https://github.github.com/gfm/#example-13
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 13 - https://github.github.com/gfm/#example-13")]
    [TestCategory("spec")]
    public void TestSpecExample13()
    {
        string inputText = @"***
---
___
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)
    (thematic_break)
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 14 - https://github.github.com/gfm/#example-14
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 14 - https://github.github.com/gfm/#example-14")]
    [TestCategory("spec")]
    public void TestSpecExample14()
    {
        string inputText = @"+++
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 15 - https://github.github.com/gfm/#example-15
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 15 - https://github.github.com/gfm/#example-15")]
    [TestCategory("spec")]
    public void TestSpecExample15()
    {
        string inputText = @"=
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 16 - https://github.github.com/gfm/#example-16
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 16 - https://github.github.com/gfm/#example-16")]
    [TestCategory("spec")]
    public void TestSpecExample16()
    {
        string inputText = @"--
**
__
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 17 - https://github.github.com/gfm/#example-17
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 17 - https://github.github.com/gfm/#example-17")]
    [TestCategory("spec")]
    public void TestSpecExample17()
    {
        string inputText = @" ***
  ***
   ***
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)
    (thematic_break)
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 18 - https://github.github.com/gfm/#example-18
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 18 - https://github.github.com/gfm/#example-18")]
    [TestCategory("spec")]
    public void TestSpecExample18()
    {
        string inputText = @"    ***
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 19 - https://github.github.com/gfm/#example-19
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 19 - https://github.github.com/gfm/#example-19")]
    [TestCategory("spec")]
    public void TestSpecExample19()
    {
        string inputText = @"Foo
    ***
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 20 - https://github.github.com/gfm/#example-20
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 20 - https://github.github.com/gfm/#example-20")]
    [TestCategory("spec")]
    public void TestSpecExample20()
    {
        string inputText = @"_____________________________________
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 21 - https://github.github.com/gfm/#example-21
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 21 - https://github.github.com/gfm/#example-21")]
    [TestCategory("spec")]
    public void TestSpecExample21()
    {
        string inputText = @" - - -
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 22 - https://github.github.com/gfm/#example-22
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 22 - https://github.github.com/gfm/#example-22")]
    [TestCategory("spec")]
    public void TestSpecExample22()
    {
        string inputText = @" **  * ** * ** * **
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 23 - https://github.github.com/gfm/#example-23
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 23 - https://github.github.com/gfm/#example-23")]
    [TestCategory("spec")]
    public void TestSpecExample23()
    {
        string inputText = @"-     -      -      -
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 24 - https://github.github.com/gfm/#example-24
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 24 - https://github.github.com/gfm/#example-24")]
    [TestCategory("spec")]
    public void TestSpecExample24()
    {
        string inputText = @"- - - -
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 25 - https://github.github.com/gfm/#example-25
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 25 - https://github.github.com/gfm/#example-25")]
    [TestCategory("spec")]
    public void TestSpecExample25()
    {
        string inputText = @"_ _ _ _ a

a------

---a---
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 26 - https://github.github.com/gfm/#example-26
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 26 - https://github.github.com/gfm/#example-26")]
    [TestCategory("spec")]
    public void TestSpecExample26()
    {
        string inputText = @" *-*
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 27 - https://github.github.com/gfm/#example-27
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 27 - https://github.github.com/gfm/#example-27")]
    [TestCategory("spec")]
    public void TestSpecExample27()
    {
        string inputText = @"- foo
***
- bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))
    (thematic_break)
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 28 - https://github.github.com/gfm/#example-28
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 28 - https://github.github.com/gfm/#example-28")]
    [TestCategory("spec")]
    public void TestSpecExample28()
    {
        string inputText = @"Foo
***
bar
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (thematic_break)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 29 - https://github.github.com/gfm/#example-29
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 29 - https://github.github.com/gfm/#example-29")]
    [TestCategory("spec")]
    public void TestSpecExample29()
    {
        string inputText = @"Foo
---
bar
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (paragraph
      (inline))))";     // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 30 - https://github.github.com/gfm/#example-30
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 30 - https://github.github.com/gfm/#example-30")]
    [TestCategory("spec")]
    public void TestSpecExample30()
    {
        string inputText = @"* Foo
* * *
* Bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline))))
    (thematic_break)
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 31 - https://github.github.com/gfm/#example-31
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 31 - https://github.github.com/gfm/#example-31")]
    [TestCategory("spec")]
    public void TestSpecExample31()
    {
        string inputText = @"- Foo
- * * *
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (thematic_break)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 32 - https://github.github.com/gfm/#example-32
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 32 - https://github.github.com/gfm/#example-32")]
    [TestCategory("spec")]
    public void TestSpecExample32()
    {
        string inputText = @"# foo
## foo
### foo
#### foo
##### foo
###### foo
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))
    (section
      (atx_heading
        (atx_h2_marker)
        heading_content:(inline))
      (section
        (atx_heading
          (atx_h3_marker)
          heading_content:(inline))
        (section
          (atx_heading
            (atx_h4_marker)
            heading_content:(inline))
          (section
            (atx_heading
              (atx_h5_marker)
              heading_content:(inline))
            (section
              (atx_heading
                (atx_h6_marker)
                heading_content:(inline)))))))))";  // Added `heading_content:` labesl in six places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 33 - https://github.github.com/gfm/#example-33
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 33 - https://github.github.com/gfm/#example-33")]
    [TestCategory("spec")]
    public void TestSpecExample33()
    {
        string inputText = @"####### foo
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 34 - https://github.github.com/gfm/#example-34
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 34 - https://github.github.com/gfm/#example-34")]
    [TestCategory("spec")]
    public void TestSpecExample34()
    {
        string inputText = @"#5 bolt

#hashtag
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 35 - https://github.github.com/gfm/#example-35
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 35 - https://github.github.com/gfm/#example-35")]
    [TestCategory("spec")]
    public void TestSpecExample35()
    {
        string inputText = @"\## foo
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 36 - https://github.github.com/gfm/#example-36
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 36 - https://github.github.com/gfm/#example-36")]
    [TestCategory("spec")]
    public void TestSpecExample36()
    {
        string inputText = @"# foo *bar* \*baz\*
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 37 - https://github.github.com/gfm/#example-37
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 37 - https://github.github.com/gfm/#example-37")]
    [TestCategory("spec")]
    public void TestSpecExample37()
    {
        string inputText = @"#                  foo
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))";     // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 38 - https://github.github.com/gfm/#example-38
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 38 - https://github.github.com/gfm/#example-38")]
    [TestCategory("spec")]
    public void TestSpecExample38()
    {
        string inputText = @" ### foo
  ## foo
   # foo
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h3_marker)
      heading_content:(inline)))
  (section
    (atx_heading
      (atx_h2_marker)
      heading_content:(inline)))
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))";     // Added `heading_content:` label in three places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 39 - https://github.github.com/gfm/#example-39
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 39 - https://github.github.com/gfm/#example-39")]
    [TestCategory("spec")]
    public void TestSpecExample39()
    {
        string inputText = @"    # foo
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 40 - https://github.github.com/gfm/#example-40
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 40 - https://github.github.com/gfm/#example-40")]
    [TestCategory("spec")]
    public void TestSpecExample40()
    {
        string inputText = @"foo
    # bar
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 41 - https://github.github.com/gfm/#example-41
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 41 - https://github.github.com/gfm/#example-41")]
    [TestCategory("spec")]
    public void TestSpecExample41()
    {
        string inputText = @"## foo ##
  ###   bar    ###
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h2_marker)
      heading_content:(inline))
    (section
      (atx_heading
        (atx_h3_marker)
        heading_content:(inline)))))";  // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 42 - https://github.github.com/gfm/#example-42
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 42 - https://github.github.com/gfm/#example-42")]
    [TestCategory("spec")]
    public void TestSpecExample42()
    {
        string inputText = @"# foo ##################################
##### foo ##
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))
    (section
      (atx_heading
        (atx_h5_marker)
        heading_content:(inline)))))";  // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 43 - https://github.github.com/gfm/#example-43
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 43 - https://github.github.com/gfm/#example-43")]
    [TestCategory("spec")]
    public void TestSpecExample43()
    {
        string inputText = @"### foo ###
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h3_marker)
      heading_content:(inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 44 - https://github.github.com/gfm/#example-44
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 44 - https://github.github.com/gfm/#example-44")]
    [TestCategory("spec")]
    public void TestSpecExample44()
    {
        string inputText = @"### foo ### b
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h3_marker)
      heading_content:(inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 45 - https://github.github.com/gfm/#example-45
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 45 - https://github.github.com/gfm/#example-45")]
    [TestCategory("spec")]
    public void TestSpecExample45()
    {
        string inputText = @"# foo#
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 46 - https://github.github.com/gfm/#example-46
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 46 - https://github.github.com/gfm/#example-46")]
    [TestCategory("spec")]
    public void TestSpecExample46()
    {
        string inputText = @"### foo \###
## foo #\##
# foo \#
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h3_marker)
      heading_content:(inline)))
  (section
    (atx_heading
      (atx_h2_marker)
      heading_content:(inline)))
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))"; // Added `heading_content:` labels in three places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 47 - https://github.github.com/gfm/#example-47
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 47 - https://github.github.com/gfm/#example-47")]
    [TestCategory("spec")]
    public void TestSpecExample47()
    {
        string inputText = @"****
## foo
****
";
        string expectedSyntaxTree = @"(document
  (section
    (thematic_break))
  (section
    (atx_heading
      (atx_h2_marker)
      heading_content:(inline))
    (thematic_break)))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 48 - https://github.github.com/gfm/#example-48
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 48 - https://github.github.com/gfm/#example-48")]
    [TestCategory("spec")]
    public void TestSpecExample48()
    {
        string inputText = @"Foo bar
# baz
Bar foo
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline)))
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))
    (paragraph
      (inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 49 - https://github.github.com/gfm/#example-49
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 49 - https://github.github.com/gfm/#example-49")]
    [TestCategory("spec")]
    public void TestSpecExample49()
    {
        string inputText = @"##
#
### ###
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h2_marker)))
  (section
    (atx_heading
      (atx_h1_marker))
    (section
      (atx_heading
        (atx_h3_marker)
        heading_content:(inline)))))";  // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 50 - https://github.github.com/gfm/#example-50
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 50 - https://github.github.com/gfm/#example-50")]
    [TestCategory("spec")]
    public void TestSpecExample50()
    {
        string inputText = @"Foo *bar*
=

Foo *bar*
---------
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h1_underline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))))";    // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 51 - https://github.github.com/gfm/#example-51
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 51 - https://github.github.com/gfm/#example-51")]
    [TestCategory("spec")]
    public void TestSpecExample51()
    {
        string inputText = @"Foo *bar
baz*
=
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h1_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 52 - https://github.github.com/gfm/#example-52
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 52 - https://github.github.com/gfm/#example-52")]
    [TestCategory("spec")]
    public void TestSpecExample52()
    {
        string inputText = @"  Foo *bar
baz*
=
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h1_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 53 - https://github.github.com/gfm/#example-53
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 53 - https://github.github.com/gfm/#example-53")]
    [TestCategory("spec")]
    public void TestSpecExample53()
    {
        string inputText = @"Foo
-------------------------

Foo
=
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h1_underline))))";    // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 54 - https://github.github.com/gfm/#example-54
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 54 - https://github.github.com/gfm/#example-54")]
    [TestCategory("spec")]
    public void TestSpecExample54()
    {
        string inputText = @"   Foo
---

  Foo
-----

  Foo
  ===
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h1_underline))))";    // Added `heading_content:` labels in three places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 55 - https://github.github.com/gfm/#example-55
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 55 - https://github.github.com/gfm/#example-55")]
    [TestCategory("spec")]
    public void TestSpecExample55()
    {
        string inputText = @"    Foo
    ---

    Foo
---
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 56 - https://github.github.com/gfm/#example-56
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 56 - https://github.github.com/gfm/#example-56")]
    [TestCategory("spec")]
    public void TestSpecExample56()
    {
        string inputText = @"Foo
   ----
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 57 - https://github.github.com/gfm/#example-57
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 57 - https://github.github.com/gfm/#example-57")]
    [TestCategory("spec")]
    public void TestSpecExample57()
    {
        string inputText = @"Foo
    ---
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 58 - https://github.github.com/gfm/#example-58
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 58 - https://github.github.com/gfm/#example-58")]
    [TestCategory("spec")]
    public void TestSpecExample58()
    {
        string inputText = @"Foo
= =

Foo
--- -
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 59 - https://github.github.com/gfm/#example-59
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 59 - https://github.github.com/gfm/#example-59")]
    [TestCategory("spec")]
    public void TestSpecExample59()
    {
        string inputText = @"Foo
-----
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 60 - https://github.github.com/gfm/#example-60
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 60 - https://github.github.com/gfm/#example-60")]
    [TestCategory("spec")]
    public void TestSpecExample60()
    {
        string inputText = @"Foo\
----
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 61 - https://github.github.com/gfm/#example-61
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 61 - https://github.github.com/gfm/#example-61")]
    [TestCategory("spec")]
    public void TestSpecExample61()
    {
        string inputText = @"`Foo
----
`

<a title=""a lot
---
of dashes""/>
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (paragraph
      (inline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (paragraph
      (inline))))";   // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 62 - https://github.github.com/gfm/#example-62
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 62 - https://github.github.com/gfm/#example-62")]
    [TestCategory("spec")]
    public void TestSpecExample62()
    {
        string inputText = @"> Foo
---
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 63 - https://github.github.com/gfm/#example-63
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 63 - https://github.github.com/gfm/#example-63")]
    [TestCategory("spec")]
    public void TestSpecExample63()
    {
        string inputText = @"> foo
bar
=
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 64 - https://github.github.com/gfm/#example-64
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 64 - https://github.github.com/gfm/#example-64")]
    [TestCategory("spec")]
    public void TestSpecExample64()
    {
        string inputText = @"- Foo
---
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 65 - https://github.github.com/gfm/#example-65
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 65 - https://github.github.com/gfm/#example-65")]
    [TestCategory("spec")]
    public void TestSpecExample65()
    {
        string inputText = @"Foo
Bar
---
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 66 - https://github.github.com/gfm/#example-66
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 66 - https://github.github.com/gfm/#example-66")]
    [TestCategory("spec")]
    public void TestSpecExample66()
    {
        string inputText = @"Extra text so this is not parsed as metadata.

---
Foo
---
Bar
---
Baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (thematic_break)
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (paragraph
      (inline))))";   // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 67 - https://github.github.com/gfm/#example-67
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 67 - https://github.github.com/gfm/#example-67")]
    [TestCategory("spec")]
    public void TestSpecExample67()
    {
        string inputText = @"
=
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 68 - https://github.github.com/gfm/#example-68
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 68 - https://github.github.com/gfm/#example-68")]
    [TestCategory("spec")]
    public void TestSpecExample68()
    {
        string inputText = @"Extra text so this is not parsed as metadata.

---
---
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (thematic_break)
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 69 - https://github.github.com/gfm/#example-69
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 69 - https://github.github.com/gfm/#example-69")]
    [TestCategory("spec")]
    public void TestSpecExample69()
    {
        string inputText = @"- foo
-----
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 70 - https://github.github.com/gfm/#example-70
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 70 - https://github.github.com/gfm/#example-70")]
    [TestCategory("spec")]
    public void TestSpecExample70()
    {
        string inputText = @"    foo
---
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 71 - https://github.github.com/gfm/#example-71
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 71 - https://github.github.com/gfm/#example-71")]
    [TestCategory("spec")]
    public void TestSpecExample71()
    {
        string inputText = @"> foo
-----
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 72 - https://github.github.com/gfm/#example-72
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 72 - https://github.github.com/gfm/#example-72")]
    [TestCategory("spec")]
    public void TestSpecExample72()
    {
        string inputText = @"\> foo
------
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 73 - https://github.github.com/gfm/#example-73
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 73 - https://github.github.com/gfm/#example-73")]
    [TestCategory("spec")]
    public void TestSpecExample73()
    {
        string inputText = @"Foo

bar
---
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (paragraph
      (inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 74 - https://github.github.com/gfm/#example-74
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 74 - https://github.github.com/gfm/#example-74")]
    [TestCategory("spec")]
    public void TestSpecExample74()
    {
        string inputText = @"Foo
bar

---

baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (thematic_break)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 75 - https://github.github.com/gfm/#example-75
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 75 - https://github.github.com/gfm/#example-75")]
    [TestCategory("spec")]
    public void TestSpecExample75()
    {
        string inputText = @"Foo
bar
* * *
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (thematic_break)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 76 - https://github.github.com/gfm/#example-76
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 76 - https://github.github.com/gfm/#example-76")]
    [TestCategory("spec")]
    public void TestSpecExample76()
    {
        string inputText = @"Foo
bar
\---
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 77 - https://github.github.com/gfm/#example-77
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 77 - https://github.github.com/gfm/#example-77")]
    [TestCategory("spec")]
    public void TestSpecExample77()
    {
        string inputText = @"    a simple
      indented code block
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 78 - https://github.github.com/gfm/#example-78
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 78 - https://github.github.com/gfm/#example-78")]
    [TestCategory("spec")]
    public void TestSpecExample78()
    {
        string inputText = @"  - foo

    bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 79 - https://github.github.com/gfm/#example-79
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 79 - https://github.github.com/gfm/#example-79")]
    [TestCategory("spec")]
    public void TestSpecExample79()
    {
        string inputText = @"1.  foo

    - bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 80 - https://github.github.com/gfm/#example-80
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 80 - https://github.github.com/gfm/#example-80")]
    [TestCategory("spec")]
    public void TestSpecExample80()
    {
        string inputText = @"    <a/>
    *hi*

    - one
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 81 - https://github.github.com/gfm/#example-81
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 81 - https://github.github.com/gfm/#example-81")]
    [TestCategory("spec")]
    public void TestSpecExample81()
    {
        string inputText = @"    chunk1

    chunk2



    chunk3
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 82 - https://github.github.com/gfm/#example-82
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 82 - https://github.github.com/gfm/#example-82")]
    [TestCategory("spec")]
    public void TestSpecExample82()
    {
        string inputText = @"    chunk1

      chunk2
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 83 - https://github.github.com/gfm/#example-83
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 83 - https://github.github.com/gfm/#example-83")]
    [TestCategory("spec")]
    public void TestSpecExample83()
    {
        string inputText = @"Foo
    bar


";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 84 - https://github.github.com/gfm/#example-84
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 84 - https://github.github.com/gfm/#example-84")]
    [TestCategory("spec")]
    public void TestSpecExample84()
    {
        string inputText = @"    foo
bar
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 85 - https://github.github.com/gfm/#example-85
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 85 - https://github.github.com/gfm/#example-85")]
    [TestCategory("spec")]
    public void TestSpecExample85()
    {
        string inputText = @"# Heading
    foo
Heading
------
    foo
----
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))
    (indented_code_block)
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (indented_code_block)
    (thematic_break)))";    // Added `heading_content:` labels in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 86 - https://github.github.com/gfm/#example-86
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 86 - https://github.github.com/gfm/#example-86")]
    [TestCategory("spec")]
    public void TestSpecExample86()
    {
        string inputText = @"        foo
    bar
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 87 - https://github.github.com/gfm/#example-87
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 87 - https://github.github.com/gfm/#example-87")]
    [TestCategory("spec")]
    public void TestSpecExample87()
    {
        string inputText = @"

    foo



";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 88 - https://github.github.com/gfm/#example-88
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 88 - https://github.github.com/gfm/#example-88")]
    [TestCategory("spec")]
    public void TestSpecExample88()
    {
        string inputText = @"    foo
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 89 - https://github.github.com/gfm/#example-89
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 89 - https://github.github.com/gfm/#example-89")]
    [TestCategory("spec")]
    public void TestSpecExample89()
    {
        string inputText = @"```
<
 >
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 90 - https://github.github.com/gfm/#example-90
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 90 - https://github.github.com/gfm/#example-90")]
    [TestCategory("spec")]
    public void TestSpecExample90()
    {
        string inputText = @"~~~
<
 >
~~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 91 - https://github.github.com/gfm/#example-91
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 91 - https://github.github.com/gfm/#example-91")]
    [TestCategory("spec")]
    public void TestSpecExample91()
    {
        string inputText = @"``
foo
``
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 92 - https://github.github.com/gfm/#example-92
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 92 - https://github.github.com/gfm/#example-92")]
    [TestCategory("spec")]
    public void TestSpecExample92()
    {
        string inputText = @"```
aaa
~~~
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 93 - https://github.github.com/gfm/#example-93
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 93 - https://github.github.com/gfm/#example-93")]
    [TestCategory("spec")]
    public void TestSpecExample93()
    {
        string inputText = @"~~~
aaa
```
~~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 94 - https://github.github.com/gfm/#example-94
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 94 - https://github.github.com/gfm/#example-94")]
    [TestCategory("spec")]
    public void TestSpecExample94()
    {
        string inputText = @"````
aaa
```
``````
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 95 - https://github.github.com/gfm/#example-95
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 95 - https://github.github.com/gfm/#example-95")]
    [TestCategory("spec")]
    public void TestSpecExample95()
    {
        string inputText = @"~~~~
aaa
~~~
~~~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 96 - https://github.github.com/gfm/#example-96
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 96 - https://github.github.com/gfm/#example-96")]
    [TestCategory("spec")]
    public void TestSpecExample96()
    {
        string inputText = @"```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 97 - https://github.github.com/gfm/#example-97
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 97 - https://github.github.com/gfm/#example-97")]
    [TestCategory("spec")]
    public void TestSpecExample97()
    {
        string inputText = @"`````

```
aaa
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 98 - https://github.github.com/gfm/#example-98
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 98 - https://github.github.com/gfm/#example-98")]
    [TestCategory("spec")]
    public void TestSpecExample98()
    {
        string inputText = @"> ```
> aaa

bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (fenced_code_block
        (fenced_code_block_delimiter)
        (block_continuation)
        (code_fence_content)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 99 - https://github.github.com/gfm/#example-99
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 99 - https://github.github.com/gfm/#example-99")]
    [TestCategory("spec")]
    public void TestSpecExample99()
    {
        string inputText = @"```


```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 100 - https://github.github.com/gfm/#example-100
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 100 - https://github.github.com/gfm/#example-100")]
    [TestCategory("spec")]
    public void TestSpecExample100()
    {
        string inputText = @"```
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 101 - https://github.github.com/gfm/#example-101
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 101 - https://github.github.com/gfm/#example-101")]
    [TestCategory("spec")]
    public void TestSpecExample101()
    {
        string inputText = @" ```
 aaa
aaa
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 102 - https://github.github.com/gfm/#example-102
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 102 - https://github.github.com/gfm/#example-102")]
    [TestCategory("spec")]
    public void TestSpecExample102()
    {
        string inputText = @"  ```
aaa
  aaa
aaa
  ```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 103 - https://github.github.com/gfm/#example-103
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 103 - https://github.github.com/gfm/#example-103")]
    [TestCategory("spec")]
    public void TestSpecExample103()
    {
        string inputText = @"   ```
   aaa
    aaa
  aaa
   ```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 104 - https://github.github.com/gfm/#example-104
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 104 - https://github.github.com/gfm/#example-104")]
    [TestCategory("spec")]
    public void TestSpecExample104()
    {
        string inputText = @"    ```
    aaa
    ```
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 105 - https://github.github.com/gfm/#example-105
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 105 - https://github.github.com/gfm/#example-105")]
    [TestCategory("spec")]
    public void TestSpecExample105()
    {
        string inputText = @"```
aaa
  ```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 106 - https://github.github.com/gfm/#example-106
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 106 - https://github.github.com/gfm/#example-106")]
    [TestCategory("spec")]
    public void TestSpecExample106()
    {
        string inputText = @"   ```
aaa
  ```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 108 - https://github.github.com/gfm/#example-108
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 108 - https://github.github.com/gfm/#example-108")]
    [TestCategory("spec")]
    public void TestSpecExample108()
    {
        string inputText = @"``` ```
aaa
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 109 - https://github.github.com/gfm/#example-109
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 109 - https://github.github.com/gfm/#example-109")]
    [TestCategory("spec")]
    public void TestSpecExample109()
    {
        string inputText = @"~~~~~~
aaa
~~~ ~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 110 - https://github.github.com/gfm/#example-110
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 110 - https://github.github.com/gfm/#example-110")]
    [TestCategory("spec")]
    public void TestSpecExample110()
    {
        string inputText = @"foo
```
bar
```
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 111 - https://github.github.com/gfm/#example-111
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 111 - https://github.github.com/gfm/#example-111")]
    [TestCategory("spec")]
    public void TestSpecExample111()
    {
        string inputText = @"foo
---
~~~
bar
~~~
# baz
";
        string expectedSyntaxTree = @"(document
  (section
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h2_underline))
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter)))
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))"; // Added `heading_content:` in two places in the expectedSyntaxTree string.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 112 - https://github.github.com/gfm/#example-112
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 112 - https://github.github.com/gfm/#example-112")]
    [TestCategory("spec")]
    public void TestSpecExample112()
    {
        string inputText = @"```ruby
def foo(x)
  return 3
end
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language))
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 113 - https://github.github.com/gfm/#example-113
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 113 - https://github.github.com/gfm/#example-113")]
    [TestCategory("spec")]
    public void TestSpecExample113()
    {
        string inputText = @"~~~~    ruby startline=3 $%@#$
def foo(x)
  return 3
end
~~~~~~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language))
      (block_continuation)
      (code_fence_content
        (block_continuation)
        (block_continuation)
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 114 - https://github.github.com/gfm/#example-114
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 114 - https://github.github.com/gfm/#example-114")]
    [TestCategory("spec")]
    public void TestSpecExample114()
    {
        string inputText = @"````;
````
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language))
      (block_continuation)
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 115 - https://github.github.com/gfm/#example-115
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 115 - https://github.github.com/gfm/#example-115")]
    [TestCategory("spec")]
    public void TestSpecExample115()
    {
        string inputText = @"``` aa ```
foo
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 116 - https://github.github.com/gfm/#example-116
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 116 - https://github.github.com/gfm/#example-116")]
    [TestCategory("spec")]
    public void TestSpecExample116()
    {
        string inputText = @"~~~ aa ``` ~~~
foo
~~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language))
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 117 - https://github.github.com/gfm/#example-117
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 117 - https://github.github.com/gfm/#example-117")]
    [TestCategory("spec")]
    public void TestSpecExample117()
    {
        string inputText = @"```
``` aaa
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 118 - https://github.github.com/gfm/#example-118
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 118 - https://github.github.com/gfm/#example-118")]
    [TestCategory("spec")]
    public void TestSpecExample118()
    {
        string inputText = @"<table><tr><td>
<pre>
**Hello**,

_world_.
</pre>
</td></tr></table>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 119 - https://github.github.com/gfm/#example-119
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 119 - https://github.github.com/gfm/#example-119")]
    [TestCategory("spec")]
    public void TestSpecExample119()
    {
        string inputText = @"<table>
  <tr>
    <td>
           hi
    </td>
  </tr>
</table>

okay.
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 120 - https://github.github.com/gfm/#example-120
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 120 - https://github.github.com/gfm/#example-120")]
    [TestCategory("spec")]
    public void TestSpecExample120()
    {
        string inputText = @" <div>
  *hello*
         <foo><a>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 121 - https://github.github.com/gfm/#example-121
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 121 - https://github.github.com/gfm/#example-121")]
    [TestCategory("spec")]
    public void TestSpecExample121()
    {
        string inputText = @"</div>
*foo*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 122 - https://github.github.com/gfm/#example-122
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 122 - https://github.github.com/gfm/#example-122")]
    [TestCategory("spec")]
    public void TestSpecExample122()
    {
        string inputText = @"<DIV CLASS=""foo"">

*Markdown*

</DIV>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)) 
    (paragraph
      (inline))
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 123 - https://github.github.com/gfm/#example-123
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 123 - https://github.github.com/gfm/#example-123")]
    [TestCategory("spec")]
    public void TestSpecExample123()
    {
        string inputText = @"<div id=""foo""
  class=""bar"">
</div>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 124 - https://github.github.com/gfm/#example-124
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 124 - https://github.github.com/gfm/#example-124")]
    [TestCategory("spec")]
    public void TestSpecExample124()
    {
        string inputText = @"<div id=""foo"" class=""bar
  baz"">
</div>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 125 - https://github.github.com/gfm/#example-125
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 125 - https://github.github.com/gfm/#example-125")]
    [TestCategory("spec")]
    public void TestSpecExample125()
    {
        string inputText = @"<div>
*foo*

*bar*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 126 - https://github.github.com/gfm/#example-126
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 126 - https://github.github.com/gfm/#example-126")]
    [TestCategory("spec")]
    public void TestSpecExample126()
    {
        string inputText = @"<div id=""foo""
*hi*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 127 - https://github.github.com/gfm/#example-127
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 127 - https://github.github.com/gfm/#example-127")]
    [TestCategory("spec")]
    public void TestSpecExample127()
    {
        string inputText = @"<div class
foo
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 128 - https://github.github.com/gfm/#example-128
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 128 - https://github.github.com/gfm/#example-128")]
    [TestCategory("spec")]
    public void TestSpecExample128()
    {
        string inputText = @"<div *???-&&&-<---
*foo*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 129 - https://github.github.com/gfm/#example-129
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 129 - https://github.github.com/gfm/#example-129")]
    [TestCategory("spec")]
    public void TestSpecExample129()
    {
        string inputText = @"<div><a href=""bar"">*foo*</a></div>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 130 - https://github.github.com/gfm/#example-130
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 130 - https://github.github.com/gfm/#example-130")]
    [TestCategory("spec")]
    public void TestSpecExample130()
    {
        string inputText = @"<table><tr><td>
foo
</td></tr></table>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }




    /// <summary>
    /// Example 131 - https://github.github.com/gfm/#example-131
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 131 - https://github.github.com/gfm/#example-131")]
    [TestCategory("spec")]
    public void TestSpecExample131()
    {
        string inputText = @"<div></div>
``` c
int x = 33;
```
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 132 - https://github.github.com/gfm/#example-132
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 132 - https://github.github.com/gfm/#example-132")]
    [TestCategory("spec")]
    public void TestSpecExample132()
    {
        string inputText = @"<a href=""foo"">
*bar*
</a>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 133 - https://github.github.com/gfm/#example-133
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 133 - https://github.github.com/gfm/#example-133")]
    [TestCategory("spec")]
    public void TestSpecExample133()
    {
        string inputText = @"<Warning>
*bar*
</Warning>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 134 - https://github.github.com/gfm/#example-134
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 134 - https://github.github.com/gfm/#example-134")]
    [TestCategory("spec")]
    public void TestSpecExample134()
    {
        string inputText = @"<i class=""foo"">
*bar*
</i>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 135 - https://github.github.com/gfm/#example-135
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 135 - https://github.github.com/gfm/#example-135")]
    [TestCategory("spec")]
    public void TestSpecExample135()
    {
        string inputText = @"</ins>
*bar*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 136 - https://github.github.com/gfm/#example-136
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 136 - https://github.github.com/gfm/#example-136")]
    [TestCategory("spec")]
    public void TestSpecExample136()
    {
        string inputText = @"<del>
*foo*
</del>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 137 - https://github.github.com/gfm/#example-137
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 137 - https://github.github.com/gfm/#example-137")]
    [TestCategory("spec")]
    public void TestSpecExample137()
    {
        string inputText = @"<del>

*foo*

</del>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))
    (paragraph
      (inline))
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 138 - https://github.github.com/gfm/#example-138
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 138 - https://github.github.com/gfm/#example-138")]
    [TestCategory("spec")]
    public void TestSpecExample138()
    {
        string inputText = @"<del>*foo*</del>
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 139 - https://github.github.com/gfm/#example-139
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 139 - https://github.github.com/gfm/#example-139")]
    [TestCategory("spec")]
    public void TestSpecExample139()
    {
        string inputText = @"<pre language=""haskell""><code>
import Text.HTML.TagSoup

main :: IO ()
main = print $ parseTags tags
</code></pre>
okay
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 140 - https://github.github.com/gfm/#example-140
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 140 - https://github.github.com/gfm/#example-140")]
    [TestCategory("spec")]
    public void TestSpecExample140()
    {
        string inputText = @"<script type=""text/javascript"">
// JavaScript example

document.getElementById(""demo"").innerHTML = ""Hello JavaScript!"";
</script>
okay
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 141 - https://github.github.com/gfm/#example-141
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 141 - https://github.github.com/gfm/#example-141")]
    [TestCategory("spec")]
    public void TestSpecExample141()
    {
        string inputText = @"<style
  type=""text/css"">
h1 {color:red;}

p {color:blue;}
</style>
okay
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 142 - https://github.github.com/gfm/#example-142
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 142 - https://github.github.com/gfm/#example-142")]
    [TestCategory("spec")]
    public void TestSpecExample142()
    {
        string inputText = @"<style
  type=""text/css"">

foo
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 143 - https://github.github.com/gfm/#example-143
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 143 - https://github.github.com/gfm/#example-143")]
    [TestCategory("spec")]
    public void TestSpecExample143()
    {
        string inputText = @"> <div>
> foo

bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (html_block
        (block_continuation)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 144 - https://github.github.com/gfm/#example-144
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 144 - https://github.github.com/gfm/#example-144")]
    [TestCategory("spec")]
    public void TestSpecExample144()
    {
        string inputText = @"- <div>
- foo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (html_block))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 145 - https://github.github.com/gfm/#example-145
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 145 - https://github.github.com/gfm/#example-145")]
    [TestCategory("spec")]
    public void TestSpecExample145()
    {
        string inputText = @"<style>p{color:red;}</style>
*foo*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 146 - https://github.github.com/gfm/#example-146
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 146 - https://github.github.com/gfm/#example-146")]
    [TestCategory("spec")]
    public void TestSpecExample146()
    {
        string inputText = @"<!-- foo -->*bar*
*baz*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 147 - https://github.github.com/gfm/#example-147
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 147 - https://github.github.com/gfm/#example-147")]
    [TestCategory("spec")]
    public void TestSpecExample147()
    {
        string inputText = @"<script>
foo
</script>1. *bar*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 148 - https://github.github.com/gfm/#example-148
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 148 - https://github.github.com/gfm/#example-148")]
    [TestCategory("spec")]
    public void TestSpecExample148()
    {
        string inputText = @"<!-- Foo

bar
   baz -->
okay
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 149 - https://github.github.com/gfm/#example-149
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 149 - https://github.github.com/gfm/#example-149")]
    [TestCategory("spec")]
    public void TestSpecExample149()
    {
        string inputText = @"<?php

  echo '>';

?>
okay
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 150 - https://github.github.com/gfm/#example-150
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 150 - https://github.github.com/gfm/#example-150")]
    [TestCategory("spec")]
    public void TestSpecExample150()
    {
        string inputText = @"<!DOCTYPE html>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 151 - https://github.github.com/gfm/#example-151
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 151 - https://github.github.com/gfm/#example-151")]
    [TestCategory("spec")]
    public void TestSpecExample151()
    {
        string inputText = @"<![CDATA[
function matchwo(a,b)
{
  if (a < b && a < 0) then {
    return 1;

  } else {

    return 0;
  }
}
]]>
okay
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 152 - https://github.github.com/gfm/#example-152
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 152 - https://github.github.com/gfm/#example-152")]
    [TestCategory("spec")]
    public void TestSpecExample152()
    {
        string inputText = @"  <!-- foo -->

    <!-- foo -->
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block)
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 153 - https://github.github.com/gfm/#example-153
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 153 - https://github.github.com/gfm/#example-153")]
    [TestCategory("spec")]
    public void TestSpecExample153()
    {
        string inputText = @"  <div>

    <div>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 154 - https://github.github.com/gfm/#example-154
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 154 - https://github.github.com/gfm/#example-154")]
    [TestCategory("spec")]
    public void TestSpecExample154()
    {
        string inputText = @"Foo
<div>
bar
</div>
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 155 - https://github.github.com/gfm/#example-155
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 155 - https://github.github.com/gfm/#example-155")]
    [TestCategory("spec")]
    public void TestSpecExample155()
    {
        string inputText = @"<div>
bar
</div>
*foo*
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 156 - https://github.github.com/gfm/#example-156
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 156 - https://github.github.com/gfm/#example-156")]
    [TestCategory("spec")]
    public void TestSpecExample156()
    {
        string inputText = @"Foo
<a href=""bar"">
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 157 - https://github.github.com/gfm/#example-157
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 157 - https://github.github.com/gfm/#example-157")]
    [TestCategory("spec")]
    public void TestSpecExample157()
    {
        string inputText = @"<div>

*Emphasized* text.

</div>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))
    (paragraph
      (inline))
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 158 - https://github.github.com/gfm/#example-158
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 158 - https://github.github.com/gfm/#example-158")]
    [TestCategory("spec")]
    public void TestSpecExample158()
    {
        string inputText = @"<div>
*Emphasized* text.
</div>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 159 - https://github.github.com/gfm/#example-159
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 159 - https://github.github.com/gfm/#example-159")]
    [TestCategory("spec")]
    public void TestSpecExample159()
    {
        string inputText = @"<table>

<tr>

<td>
Hi
</td>

</tr>

</table>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))
    (html_block
      (block_continuation))
    (html_block
      (block_continuation)
      (block_continuation)
      (block_continuation))
    (html_block
      (block_continuation))
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 160 - https://github.github.com/gfm/#example-160
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 160 - https://github.github.com/gfm/#example-160")]
    [TestCategory("spec")]
    public void TestSpecExample160()
    {
        string inputText = @"<table>

  <tr>

    <td>
      Hi
    </td>

  </tr>

</table>
";
        string expectedSyntaxTree = @"(document
  (section
    (html_block
      (block_continuation))
    (html_block
      (block_continuation))
    (indented_code_block
      (block_continuation)
      (block_continuation))
    (html_block
      (block_continuation))
    (html_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 161 - https://github.github.com/gfm/#example-161
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 161 - https://github.github.com/gfm/#example-161")]
    [TestCategory("spec")]
    public void TestSpecExample161()
    {
        string inputText = @"[foo]: /url ""title""

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination)
      (link_title))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 162 - https://github.github.com/gfm/#example-162
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 162 - https://github.github.com/gfm/#example-162")]
    [TestCategory("spec")]
    public void TestSpecExample162()
    {
        string inputText = @"   [foo]:
      /url
           'the title'

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination)
      (link_title))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 163 - https://github.github.com/gfm/#example-163
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 163 - https://github.github.com/gfm/#example-163")]
    [TestCategory("spec")]
    public void TestSpecExample163()
    {
        string inputText = @"[Foo*bar\]]:my_(url) 'title (with parens)'

[Foo*bar\]]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label
        (backslash_escape))
      (link_destination)
      (link_title))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 164 - https://github.github.com/gfm/#example-164
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 164 - https://github.github.com/gfm/#example-164")]
    [TestCategory("spec")]
    public void TestSpecExample164()
    {
        string inputText = @"[Foo bar]:
<my url>
'title'

[Foo bar]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination)
      (link_title))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 165 - https://github.github.com/gfm/#example-165
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 165 - https://github.github.com/gfm/#example-165")]
    [TestCategory("spec")]
    public void TestSpecExample165()
    {
        string inputText = @"[foo]: /url '
title
line1
line2
'

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination)
      (link_title))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 166 - https://github.github.com/gfm/#example-166
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 166 - https://github.github.com/gfm/#example-166")]
    [TestCategory("spec")]
    public void TestSpecExample166()
    {
        string inputText = @"[foo]: /url 'title

with blank line'

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 167 - https://github.github.com/gfm/#example-167
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 167 - https://github.github.com/gfm/#example-167")]
    [TestCategory("spec")]
    public void TestSpecExample167()
    {
        string inputText = @"[foo]:
/url

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 168 - https://github.github.com/gfm/#example-168
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 168 - https://github.github.com/gfm/#example-168")]
    [TestCategory("spec")]
    public void TestSpecExample168()
    {
        string inputText = @"[foo]:

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 169 - https://github.github.com/gfm/#example-169
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 169 - https://github.github.com/gfm/#example-169")]
    [TestCategory("spec")]
    public void TestSpecExample169()
    {
        string inputText = @"[foo]: <>

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 170 - https://github.github.com/gfm/#example-170
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 170 - https://github.github.com/gfm/#example-170")]
    [TestCategory("spec")]
    public void TestSpecExample170()
    {
        string inputText = @"[foo]: <bar>(baz)

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 171 - https://github.github.com/gfm/#example-171
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 171 - https://github.github.com/gfm/#example-171")]
    [TestCategory("spec")]
    public void TestSpecExample171()
    {
        string inputText = @"[foo]: /url\bar\*baz ""foo\""bar\baz""

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination
        (backslash_escape))
      (link_title
        (backslash_escape)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 172 - https://github.github.com/gfm/#example-172
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 172 - https://github.github.com/gfm/#example-172")]
    [TestCategory("spec")]
    public void TestSpecExample172()
    {
        string inputText = @"[foo]

[foo]: url
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (link_reference_definition
      (link_label)
      (link_destination))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 173 - https://github.github.com/gfm/#example-173
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 173 - https://github.github.com/gfm/#example-173")]
    [TestCategory("spec")]
    public void TestSpecExample173()
    {
        string inputText = @"[foo]

[foo]: first
[foo]: second
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (link_reference_definition
      (link_label)
      (link_destination))
    (link_reference_definition
      (link_label)
      (link_destination))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 174 - https://github.github.com/gfm/#example-174
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 174 - https://github.github.com/gfm/#example-174")]
    [TestCategory("spec")]
    public void TestSpecExample174()
    {
        string inputText = @"[FOO]: /url

[Foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 175 - https://github.github.com/gfm/#example-175
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 175 - https://github.github.com/gfm/#example-175")]
    [TestCategory("spec")]
    public void TestSpecExample175()
    {
        string inputText = @"[ΑΓΩ]: /φου

[αγω]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 176 - https://github.github.com/gfm/#example-176
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 176 - https://github.github.com/gfm/#example-176")]
    [TestCategory("spec")]
    public void TestSpecExample176()
    {
        string inputText = @"[foo]: /url
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 177 - https://github.github.com/gfm/#example-177
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 177 - https://github.github.com/gfm/#example-177")]
    [TestCategory("spec")]
    public void TestSpecExample177()
    {
        string inputText = @"[
foo
]: /url
bar
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 178 - https://github.github.com/gfm/#example-178
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 178 - https://github.github.com/gfm/#example-178")]
    [TestCategory("spec")]
    public void TestSpecExample178()
    {
        string inputText = @"[foo]: /url ""title"" ok
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 179 - https://github.github.com/gfm/#example-179
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 179 - https://github.github.com/gfm/#example-179")]
    [TestCategory("spec")]
    public void TestSpecExample179()
    {
        string inputText = @"[foo]: /url
""title"" ok
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 180 - https://github.github.com/gfm/#example-180
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 180 - https://github.github.com/gfm/#example-180")]
    [TestCategory("spec")]
    public void TestSpecExample180()
    {
        string inputText = @"    [foo]: /url ""title""

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 181 - https://github.github.com/gfm/#example-181
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 181 - https://github.github.com/gfm/#example-181")]
    [TestCategory("spec")]
    public void TestSpecExample181()
    {
        string inputText = @"```
[foo]: /url
```

[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 182 - https://github.github.com/gfm/#example-182
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 182 - https://github.github.com/gfm/#example-182")]
    [TestCategory("spec")]
    public void TestSpecExample182()
    {
        string inputText = @"Foo
[bar]: /baz

[bar]
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 183 - https://github.github.com/gfm/#example-183
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 183 - https://github.github.com/gfm/#example-183")]
    [TestCategory("spec")]
    public void TestSpecExample183()
    {
        string inputText = @"# [Foo]
[foo]: /url
> bar
";
        string expectedSyntaxTree = @"(document
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))
    (link_reference_definition
      (link_label)
      (link_destination))
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";  // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 184 - https://github.github.com/gfm/#example-184
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 184 - https://github.github.com/gfm/#example-184")]
    [TestCategory("spec")]
    public void TestSpecExample184()
    {
        string inputText = @"[foo]: /url
bar
=
[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (setext_heading
      heading_content:(paragraph
        (inline))
      (setext_h1_underline))
    (paragraph
      (inline))))"; // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 185 - https://github.github.com/gfm/#example-185
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 185 - https://github.github.com/gfm/#example-185")]
    [TestCategory("spec")]
    public void TestSpecExample185()
    {
        string inputText = @"[foo]: /url
=
[foo]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 186 - https://github.github.com/gfm/#example-186
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 186 - https://github.github.com/gfm/#example-186")]
    [TestCategory("spec")]
    public void TestSpecExample186()
    {
        string inputText = @"[foo]: /foo-url ""foo""
[bar]: /bar-url
  ""bar""
[baz]: /baz-url

[foo],
[bar],
[baz]
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination)
      (link_title))
    (link_reference_definition
      (link_label)
      (link_destination)
      (link_title))
    (link_reference_definition
      (link_label)
      (link_destination))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 187 - https://github.github.com/gfm/#example-187
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 187 - https://github.github.com/gfm/#example-187")]
    [TestCategory("spec")]
    public void TestSpecExample187()
    {
        string inputText = @"[foo]

> [foo]: /url
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (block_quote
      (block_quote_marker)
      (link_reference_definition
        (link_label)
        (link_destination)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 188 - https://github.github.com/gfm/#example-188
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 188 - https://github.github.com/gfm/#example-188")]
    [TestCategory("spec")]
    public void TestSpecExample188()
    {
        string inputText = @"[foo]: /url
";
        string expectedSyntaxTree = @"(document
  (section
    (link_reference_definition
      (link_label)
      (link_destination))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 189 - https://github.github.com/gfm/#example-189
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 189 - https://github.github.com/gfm/#example-189")]
    [TestCategory("spec")]
    public void TestSpecExample189()
    {
        string inputText = @"aaa

bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 190 - https://github.github.com/gfm/#example-190
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 190 - https://github.github.com/gfm/#example-190")]
    [TestCategory("spec")]
    public void TestSpecExample190()
    {
        string inputText = @"aaa
bbb

ccc
ddd
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 191 - https://github.github.com/gfm/#example-191
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 191 - https://github.github.com/gfm/#example-191")]
    [TestCategory("spec")]
    public void TestSpecExample191()
    {
        string inputText = @"aaa


bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 192 - https://github.github.com/gfm/#example-192
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 192 - https://github.github.com/gfm/#example-192")]
    [TestCategory("spec")]
    public void TestSpecExample192()
    {
        string inputText = @"  aaa
 bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 193 - https://github.github.com/gfm/#example-193
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 193 - https://github.github.com/gfm/#example-193")]
    [TestCategory("spec")]
    public void TestSpecExample193()
    {
        string inputText = @"aaa
             bbb
                                       ccc
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 194 - https://github.github.com/gfm/#example-194
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 194 - https://github.github.com/gfm/#example-194")]
    [TestCategory("spec")]
    public void TestSpecExample194()
    {
        string inputText = @"   aaa
bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 195 - https://github.github.com/gfm/#example-195
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 195 - https://github.github.com/gfm/#example-195")]
    [TestCategory("spec")]
    public void TestSpecExample195()
    {
        string inputText = @"    aaa
bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 196 - https://github.github.com/gfm/#example-196
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 196 - https://github.github.com/gfm/#example-196")]
    [TestCategory("spec")]
    public void TestSpecExample196()
    {
        string inputText = @"aaa  
bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 197 - https://github.github.com/gfm/#example-197
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 197 - https://github.github.com/gfm/#example-197")]
    [TestCategory("spec")]
    public void TestSpecExample197()
    {
        string inputText = @"

aaa


# aaa


";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline)))
  (section
    (atx_heading
      (atx_h1_marker)
      heading_content:(inline))))";     // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 206 - https://github.github.com/gfm/#example-206
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 206 - https://github.github.com/gfm/#example-206")]
    [TestCategory("spec")]
    public void TestSpecExample206()
    {
        string inputText = @"> # Foo
> bar
> baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (section
        (atx_heading
          (atx_h1_marker)
          heading_content:(inline)
          (block_continuation))
        (paragraph
          (inline
            (block_continuation)))))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 207 - https://github.github.com/gfm/#example-207
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 207 - https://github.github.com/gfm/#example-207")]
    [TestCategory("spec")]
    public void TestSpecExample207()
    {
        string inputText = @"># Foo
>bar
> baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (section
        (atx_heading
          (atx_h1_marker)
          heading_content:(inline)
          (block_continuation))
        (paragraph
          (inline
            (block_continuation)))))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 208 - https://github.github.com/gfm/#example-208
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 208 - https://github.github.com/gfm/#example-208")]
    [TestCategory("spec")]
    public void TestSpecExample208()
    {
        string inputText = @"   > # Foo
   > bar
 > baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (section
        (atx_heading
          (atx_h1_marker)
          heading_content:(inline)
          (block_continuation))
        (paragraph
          (inline
            (block_continuation)))))))";    // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 209 - https://github.github.com/gfm/#example-209
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 209 - https://github.github.com/gfm/#example-209")]
    [TestCategory("spec")]
    public void TestSpecExample209()
    {
        string inputText = @"    > # Foo
    > bar
    > baz
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 210 - https://github.github.com/gfm/#example-210
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 210 - https://github.github.com/gfm/#example-210")]
    [TestCategory("spec")]
    public void TestSpecExample210()
    {
        string inputText = @"> # Foo
> bar
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (section
        (atx_heading
          (atx_h1_marker)
          heading_content:(inline)
          (block_continuation))
        (paragraph
          (inline))))))";   // Added `heading_content:` label.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 211 - https://github.github.com/gfm/#example-211
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 211 - https://github.github.com/gfm/#example-211")]
    [TestCategory("spec")]
    public void TestSpecExample211()
    {
        string inputText = @"> bar
baz
> foo
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline
          (block_continuation))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 212 - https://github.github.com/gfm/#example-212
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 212 - https://github.github.com/gfm/#example-212")]
    [TestCategory("spec")]
    public void TestSpecExample212()
    {
        string inputText = @"> foo
---
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))
    (thematic_break)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 213 - https://github.github.com/gfm/#example-213
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 213 - https://github.github.com/gfm/#example-213")]
    [TestCategory("spec")]
    public void TestSpecExample213()
    {
        string inputText = @"> - foo
- bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (list
        (list_item
          (list_marker_minus)
          (paragraph
            (inline)))))
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 214 - https://github.github.com/gfm/#example-214
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 214 - https://github.github.com/gfm/#example-214")]
    [TestCategory("spec")]
    public void TestSpecExample214()
    {
        string inputText = @">     foo
    bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (indented_code_block))
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 215 - https://github.github.com/gfm/#example-215
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 215 - https://github.github.com/gfm/#example-215")]
    [TestCategory("spec")]
    public void TestSpecExample215()
    {
        string inputText = @"> ```
foo
```
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (fenced_code_block
        (fenced_code_block_delimiter)))
    (paragraph
      (inline))
    (fenced_code_block
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 216 - https://github.github.com/gfm/#example-216
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 216 - https://github.github.com/gfm/#example-216")]
    [TestCategory("spec")]
    public void TestSpecExample216()
    {
        string inputText = @"> foo
    - bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 217 - https://github.github.com/gfm/#example-217
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 217 - https://github.github.com/gfm/#example-217")]
    [TestCategory("spec")]
    public void TestSpecExample217()
    {
        string inputText = @">
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 218 - https://github.github.com/gfm/#example-218
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 218 - https://github.github.com/gfm/#example-218")]
    [TestCategory("spec")]
    public void TestSpecExample218()
    {
        string inputText = @">
>
>
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (block_continuation)
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 219 - https://github.github.com/gfm/#example-219
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 219 - https://github.github.com/gfm/#example-219")]
    [TestCategory("spec")]
    public void TestSpecExample219()
    {
        string inputText = @">
> foo
>
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (block_continuation)
      (paragraph
        (inline)
        (block_continuation)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 220 - https://github.github.com/gfm/#example-220
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 220 - https://github.github.com/gfm/#example-220")]
    [TestCategory("spec")]
    public void TestSpecExample220()
    {
        string inputText = @"> foo

> bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 221 - https://github.github.com/gfm/#example-221
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 221 - https://github.github.com/gfm/#example-221")]
    [TestCategory("spec")]
    public void TestSpecExample221()
    {
        string inputText = @"> foo
> bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline
          (block_continuation))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 222 - https://github.github.com/gfm/#example-222
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 222 - https://github.github.com/gfm/#example-222")]
    [TestCategory("spec")]
    public void TestSpecExample222()
    {
        string inputText = @"> foo
>
> bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)
        (block_continuation))
      (block_continuation)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 223 - https://github.github.com/gfm/#example-223
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 223 - https://github.github.com/gfm/#example-223")]
    [TestCategory("spec")]
    public void TestSpecExample223()
    {
        string inputText = @"foo
> bar
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 224 - https://github.github.com/gfm/#example-224
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 224 - https://github.github.com/gfm/#example-224")]
    [TestCategory("spec")]
    public void TestSpecExample224()
    {
        string inputText = @"> aaa
***
> bbb
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))
    (thematic_break)
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 225 - https://github.github.com/gfm/#example-225
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 225 - https://github.github.com/gfm/#example-225")]
    [TestCategory("spec")]
    public void TestSpecExample225()
    {
        string inputText = @"> bar
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 226 - https://github.github.com/gfm/#example-226
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 226 - https://github.github.com/gfm/#example-226")]
    [TestCategory("spec")]
    public void TestSpecExample226()
    {
        string inputText = @"> bar

baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 227 - https://github.github.com/gfm/#example-227
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 227 - https://github.github.com/gfm/#example-227")]
    [TestCategory("spec")]
    public void TestSpecExample227()
    {
        string inputText = @"> bar
>
baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)
        (block_continuation)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 228 - https://github.github.com/gfm/#example-228
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 228 - https://github.github.com/gfm/#example-228")]
    [TestCategory("spec")]
    public void TestSpecExample228()
    {
        string inputText = @"> > > foo
bar
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (block_quote
        (block_quote_marker)
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 229 - https://github.github.com/gfm/#example-229
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 229 - https://github.github.com/gfm/#example-229")]
    [TestCategory("spec")]
    public void TestSpecExample229()
    {
        string inputText = @">>> foo
> bar
>>baz
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (block_quote
        (block_quote_marker)
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline
              (block_continuation)
              (block_continuation))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 230 - https://github.github.com/gfm/#example-230
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 230 - https://github.github.com/gfm/#example-230")]
    [TestCategory("spec")]
    public void TestSpecExample230()
    {
        string inputText = @">     code

>    not code
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (indented_code_block))
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 231 - https://github.github.com/gfm/#example-231
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 231 - https://github.github.com/gfm/#example-231")]
    [TestCategory("spec")]
    public void TestSpecExample231()
    {
        string inputText = @"A paragraph
with two lines.

    indented code

> A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (indented_code_block)
    (block_quote
      (block_quote_marker)
      (paragraph
        (inline)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 232 - https://github.github.com/gfm/#example-232
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 232 - https://github.github.com/gfm/#example-232")]
    [TestCategory("spec")]
    public void TestSpecExample232()
    {
        string inputText = @"1.  A paragraph
    with two lines.

        indented code

    > A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline
            (block_continuation))
          (block_continuation))
        (block_continuation)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 233 - https://github.github.com/gfm/#example-233
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 233 - https://github.github.com/gfm/#example-233")]
    [TestCategory("spec")]
    public void TestSpecExample233()
    {
        string inputText = @"- one

 two
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 234 - https://github.github.com/gfm/#example-234
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 234 - https://github.github.com/gfm/#example-234")]
    [TestCategory("spec")]
    public void TestSpecExample234()
    {
        string inputText = @"- one

  two
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 235 - https://github.github.com/gfm/#example-235
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 235 - https://github.github.com/gfm/#example-235")]
    [TestCategory("spec")]
    public void TestSpecExample235()
    {
        string inputText = @" -    one

     two
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))))
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 236 - https://github.github.com/gfm/#example-236
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 236 - https://github.github.com/gfm/#example-236")]
    [TestCategory("spec")]
    public void TestSpecExample236()
    {
        string inputText = @" -    one

      two
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 237 - https://github.github.com/gfm/#example-237
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 237 - https://github.github.com/gfm/#example-237")]
    [TestCategory("spec")]
    public void TestSpecExample237()
    {
        string inputText = @"   > > 1.  one
>>
>>     two
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (block_quote
        (block_quote_marker)
        (list
          (list_item
            (list_marker_dot)
            (paragraph
              (inline)
              (block_continuation))
            (block_continuation)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 238 - https://github.github.com/gfm/#example-238
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 238 - https://github.github.com/gfm/#example-238")]
    [TestCategory("spec")]
    public void TestSpecExample238()
    {
        string inputText = @">>- one
>>
  >  > two
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (block_quote
        (block_quote_marker)
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation))
            (block_continuation)))
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 239 - https://github.github.com/gfm/#example-239
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 239 - https://github.github.com/gfm/#example-239")]
    [TestCategory("spec")]
    public void TestSpecExample239()
    {
        string inputText = @"-one

2.two
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 240 - https://github.github.com/gfm/#example-240
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 240 - https://github.github.com/gfm/#example-240")]
    [TestCategory("spec")]
    public void TestSpecExample240()
    {
        string inputText = @"- foo


  bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 241 - https://github.github.com/gfm/#example-241
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 241 - https://github.github.com/gfm/#example-241")]
    [TestCategory("spec")]
    public void TestSpecExample241()
    {
        string inputText = @"1.  foo

    ```
    bar
    ```

    baz

    > bam
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (fenced_code_block
          (fenced_code_block_delimiter)
          (block_continuation)
          (code_fence_content
            (block_continuation))
          (fenced_code_block_delimiter)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 242 - https://github.github.com/gfm/#example-242
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 242 - https://github.github.com/gfm/#example-242")]
    [TestCategory("spec")]
    public void TestSpecExample242()
    {
        string inputText = @"- Foo

      bar


      baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block
          (block_continuation)
          (block_continuation)
          (block_continuation))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 243 - https://github.github.com/gfm/#example-243
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 243 - https://github.github.com/gfm/#example-243")]
    [TestCategory("spec")]
    public void TestSpecExample243()
    {
        string inputText = @"123456789. ok
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 244 - https://github.github.com/gfm/#example-244
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 244 - https://github.github.com/gfm/#example-244")]
    [TestCategory("spec")]
    public void TestSpecExample244()
    {
        string inputText = @"1234567890. not ok
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 245 - https://github.github.com/gfm/#example-245
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 245 - https://github.github.com/gfm/#example-245")]
    [TestCategory("spec")]
    public void TestSpecExample245()
    {
        string inputText = @"0. ok
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 246 - https://github.github.com/gfm/#example-246
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 246 - https://github.github.com/gfm/#example-246")]
    [TestCategory("spec")]
    public void TestSpecExample246()
    {
        string inputText = @"003. ok
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 247 - https://github.github.com/gfm/#example-247
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 247 - https://github.github.com/gfm/#example-247")]
    [TestCategory("spec")]
    public void TestSpecExample247()
    {
        string inputText = @"-1. not ok
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 248 - https://github.github.com/gfm/#example-248
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 248 - https://github.github.com/gfm/#example-248")]
    [TestCategory("spec")]
    public void TestSpecExample248()
    {
        string inputText = @"- foo

      bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 249 - https://github.github.com/gfm/#example-249
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 249 - https://github.github.com/gfm/#example-249")]
    [TestCategory("spec")]
    public void TestSpecExample249()
    {
        string inputText = @"  10.  foo

           bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 250 - https://github.github.com/gfm/#example-250
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 250 - https://github.github.com/gfm/#example-250")]
    [TestCategory("spec")]
    public void TestSpecExample250()
    {
        string inputText = @"    indented code

paragraph

    more code
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)
    (paragraph
      (inline))
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 251 - https://github.github.com/gfm/#example-251
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 251 - https://github.github.com/gfm/#example-251")]
    [TestCategory("spec")]
    public void TestSpecExample251()
    {
        string inputText = @"1.     indented code

   paragraph

       more code
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 252 - https://github.github.com/gfm/#example-252
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 252 - https://github.github.com/gfm/#example-252")]
    [TestCategory("spec")]
    public void TestSpecExample252()
    {
        string inputText = @"1.      indented code

   paragraph

       more code
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 253 - https://github.github.com/gfm/#example-253
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 253 - https://github.github.com/gfm/#example-253")]
    [TestCategory("spec")]
    public void TestSpecExample253()
    {
        string inputText = @"   foo

bar
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 254 - https://github.github.com/gfm/#example-254
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 254 - https://github.github.com/gfm/#example-254")]
    [TestCategory("spec")]
    public void TestSpecExample254()
    {
        string inputText = @"-    foo

  bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 255 - https://github.github.com/gfm/#example-255
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 255 - https://github.github.com/gfm/#example-255")]
    [TestCategory("spec")]
    public void TestSpecExample255()
    {
        string inputText = @"-  foo

   bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 256 - https://github.github.com/gfm/#example-256
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 256 - https://github.github.com/gfm/#example-256")]
    [TestCategory("spec")]
    public void TestSpecExample256()
    {
        string inputText = @"-
  foo
-
  ```
  bar
  ```
-
      baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (block_continuation)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (block_continuation)
        (fenced_code_block
          (fenced_code_block_delimiter)
          (block_continuation)
          (code_fence_content
            (block_continuation))
          (fenced_code_block_delimiter)))
      (list_item
        (list_marker_minus)
        (block_continuation)
        (indented_code_block)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 257 - https://github.github.com/gfm/#example-257
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 257 - https://github.github.com/gfm/#example-257")]
    [TestCategory("spec")]
    public void TestSpecExample257()
    {
        string inputText = @"-
  foo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 258 - https://github.github.com/gfm/#example-258
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 258 - https://github.github.com/gfm/#example-258")]
    [TestCategory("spec")]
    public void TestSpecExample258()
    {
        string inputText = @"-

  foo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (block_continuation)))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 259 - https://github.github.com/gfm/#example-259
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 259 - https://github.github.com/gfm/#example-259")]
    [TestCategory("spec")]
    public void TestSpecExample259()
    {
        string inputText = @"- foo
-
- bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 260 - https://github.github.com/gfm/#example-260
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 260 - https://github.github.com/gfm/#example-260")]
    [TestCategory("spec")]
    public void TestSpecExample260()
    {
        string inputText = @"- foo
-
- bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 261 - https://github.github.com/gfm/#example-261
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 261 - https://github.github.com/gfm/#example-261")]
    [TestCategory("spec")]
    public void TestSpecExample261()
    {
        string inputText = @"1. foo
2.
3. bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)))
      (list_item
        (list_marker_dot))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 262 - https://github.github.com/gfm/#example-262
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 262 - https://github.github.com/gfm/#example-262")]
    [TestCategory("spec")]
    public void TestSpecExample262()
    {
        string inputText = @"*
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_star)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 263 - https://github.github.com/gfm/#example-263
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 263 - https://github.github.com/gfm/#example-263")]
    [TestCategory("spec")]
    public void TestSpecExample263()
    {
        string inputText = @"foo
*

foo
1.
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 264 - https://github.github.com/gfm/#example-264
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 264 - https://github.github.com/gfm/#example-264")]
    [TestCategory("spec")]
    public void TestSpecExample264()
    {
        string inputText = @" 1.  A paragraph
     with two lines.

         indented code

     > A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline
            (block_continuation))
          (block_continuation))
        (block_continuation)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 265 - https://github.github.com/gfm/#example-265
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 265 - https://github.github.com/gfm/#example-265")]
    [TestCategory("spec")]
    public void TestSpecExample265()
    {
        string inputText = @"  1.  A paragraph
      with two lines.

          indented code

      > A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline
            (block_continuation))
          (block_continuation))
        (block_continuation)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 266 - https://github.github.com/gfm/#example-266
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 266 - https://github.github.com/gfm/#example-266")]
    [TestCategory("spec")]
    public void TestSpecExample266()
    {
        string inputText = @"   1.  A paragraph
       with two lines.

           indented code

       > A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline
            (block_continuation))
          (block_continuation))
        (block_continuation)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 267 - https://github.github.com/gfm/#example-267
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 267 - https://github.github.com/gfm/#example-267")]
    [TestCategory("spec")]
    public void TestSpecExample267()
    {
        string inputText = @"    1.  A paragraph
        with two lines.

            indented code

        > A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block
      (block_continuation))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 268 - https://github.github.com/gfm/#example-268
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 268 - https://github.github.com/gfm/#example-268")]
    [TestCategory("spec")]
    public void TestSpecExample268()
    {
        string inputText = @"  1.  A paragraph
with two lines.

          indented code

      > A block quote.
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (indented_code_block
          (block_continuation)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 269 - https://github.github.com/gfm/#example-269
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 269 - https://github.github.com/gfm/#example-269")]
    [TestCategory("spec")]
    public void TestSpecExample269()
    {
        string inputText = @"  1.  A paragraph
    with two lines.
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 270 - https://github.github.com/gfm/#example-270
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 270 - https://github.github.com/gfm/#example-270")]
    [TestCategory("spec")]
    public void TestSpecExample270()
    {
        string inputText = @"> 1. > Blockquote
continued here.
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (list
        (list_item
          (list_marker_dot)
          (block_quote
            (block_quote_marker)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 271 - https://github.github.com/gfm/#example-271
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 271 - https://github.github.com/gfm/#example-271")]
    [TestCategory("spec")]
    public void TestSpecExample271()
    {
        string inputText = @"> 1. > Blockquote
> continued here.
";
        string expectedSyntaxTree = @"(document
  (section
    (block_quote
      (block_quote_marker)
      (list
        (list_item
          (list_marker_dot)
          (block_quote
            (block_quote_marker)
            (paragraph
              (inline
                (block_continuation)))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 272 - https://github.github.com/gfm/#example-272
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 272 - https://github.github.com/gfm/#example-272")]
    [TestCategory("spec")]
    public void TestSpecExample272()
    {
        string inputText = @"- foo
  - bar
    - baz
      - boo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation))
            (list
              (list_item
                (list_marker_minus)
                (paragraph
                  (inline)
                  (block_continuation))
                (list
                  (list_item
                    (list_marker_minus)
                    (paragraph
                      (inline))))))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 273 - https://github.github.com/gfm/#example-273
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 273 - https://github.github.com/gfm/#example-273")]
    [TestCategory("spec")]
    public void TestSpecExample273()
    {
        string inputText = @"- foo
 - bar
  - baz
   - boo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 274 - https://github.github.com/gfm/#example-274
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 274 - https://github.github.com/gfm/#example-274")]
    [TestCategory("spec")]
    public void TestSpecExample274()
    {
        string inputText = @"10) foo
    - bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_parenthesis)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 275 - https://github.github.com/gfm/#example-275
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 275 - https://github.github.com/gfm/#example-275")]
    [TestCategory("spec")]
    public void TestSpecExample275()
    {
        string inputText = @"10) foo
   - bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_parenthesis)
        (paragraph
          (inline))))
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 276 - https://github.github.com/gfm/#example-276
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 276 - https://github.github.com/gfm/#example-276")]
    [TestCategory("spec")]
    public void TestSpecExample276()
    {
        string inputText = @"- - foo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 277 - https://github.github.com/gfm/#example-277
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 277 - https://github.github.com/gfm/#example-277")]
    [TestCategory("spec")]
    public void TestSpecExample277()
    {
        string inputText = @"1. - 2. foo
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (list
          (list_item
            (list_marker_minus)
            (list
              (list_item
                (list_marker_dot)
                (paragraph
                  (inline))))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 278 - https://github.github.com/gfm/#example-278
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 278 - https://github.github.com/gfm/#example-278")]
    [TestCategory("spec")]
    public void TestSpecExample278()
    {
        string inputText = @"- # Foo
- Bar
  ---
  baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (section
          (atx_heading
            (atx_h1_marker)
            heading_content:(inline))))
      (list_item
        (list_marker_minus)
        (setext_heading
          heading_content:(paragraph
            (inline)
            (block_continuation))
          (setext_h2_underline)
          (block_continuation))
        (paragraph
          (inline))))))";   // Added `heading_content:` label in two places.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 281 - https://github.github.com/gfm/#example-281
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 281 - https://github.github.com/gfm/#example-281")]
    [TestCategory("spec")]
    public void TestSpecExample281()
    {
        string inputText = @"- foo
- bar
+ baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))
    (list
      (list_item
        (list_marker_plus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 282 - https://github.github.com/gfm/#example-282
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 282 - https://github.github.com/gfm/#example-282")]
    [TestCategory("spec")]
    public void TestSpecExample282()
    {
        string inputText = @"1. foo
2. bar
3) baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))
    (list
      (list_item
        (list_marker_parenthesis)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 283 - https://github.github.com/gfm/#example-283
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 283 - https://github.github.com/gfm/#example-283")]
    [TestCategory("spec")]
    public void TestSpecExample283()
    {
        string inputText = @"Foo
- bar
- baz
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 284 - https://github.github.com/gfm/#example-284
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 284 - https://github.github.com/gfm/#example-284")]
    [TestCategory("spec")]
    public void TestSpecExample284()
    {
        string inputText = @"The number of windows in my house is
14.  The number of doors is 6.
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 285 - https://github.github.com/gfm/#example-285
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 285 - https://github.github.com/gfm/#example-285")]
    [TestCategory("spec")]
    public void TestSpecExample285()
    {
        string inputText = @"The number of windows in my house is
1.  The number of doors is 6.
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 286 - https://github.github.com/gfm/#example-286
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 286 - https://github.github.com/gfm/#example-286")]
    [TestCategory("spec")]
    public void TestSpecExample286()
    {
        string inputText = @"- foo

- bar


- baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 287 - https://github.github.com/gfm/#example-287
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 287 - https://github.github.com/gfm/#example-287")]
    [TestCategory("spec")]
    public void TestSpecExample287()
    {
        string inputText = @"- foo
  - bar
    - baz


      bim
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation))
            (list
              (list_item
                (list_marker_minus)
                (paragraph
                  (inline)
                  (block_continuation))
                (block_continuation)
                (block_continuation)
                (paragraph
                  (inline))))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 288 - https://github.github.com/gfm/#example-288
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 288 - https://github.github.com/gfm/#example-288")]
    [TestCategory("spec")]
    public void TestSpecExample288()
    {
        string inputText = @"- foo
- bar

<!-- -->

- baz
- bim
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))))
    (html_block)
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 289 - https://github.github.com/gfm/#example-289
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 289 - https://github.github.com/gfm/#example-289")]
    [TestCategory("spec")]
    public void TestSpecExample289()
    {
        string inputText = @"-   foo

    notcode

-   foo

<!-- -->

    code
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline)
          (block_continuation)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))))
    (html_block)
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 290 - https://github.github.com/gfm/#example-290
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 290 - https://github.github.com/gfm/#example-290")]
    [TestCategory("spec")]
    public void TestSpecExample290()
    {
        string inputText = @"- a
 - b
  - c
   - d
  - e
 - f
- g
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 291 - https://github.github.com/gfm/#example-291
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 291 - https://github.github.com/gfm/#example-291")]
    [TestCategory("spec")]
    public void TestSpecExample291()
    {
        string inputText = @"1. a

  2. b

   3. c
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation)))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation)))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 292 - https://github.github.com/gfm/#example-292
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 292 - https://github.github.com/gfm/#example-292")]
    [TestCategory("spec")]
    public void TestSpecExample292()
    {
        string inputText = @"- a
 - b
  - c
   - d
    - e
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))"; // Note: spec implies deeper nesting, but parser flattens beyond a certain depth for this input.
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 293 - https://github.github.com/gfm/#example-293
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 293 - https://github.github.com/gfm/#example-293")]
    [TestCategory("spec")]
    public void TestSpecExample293()
    {
        string inputText = @"1. a

  2. b

    3. c
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation)))
      (list_item
        (list_marker_dot)
        (paragraph
          (inline)
          (block_continuation))))
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 294 - https://github.github.com/gfm/#example-294
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 294 - https://github.github.com/gfm/#example-294")]
    [TestCategory("spec")]
    public void TestSpecExample294()
    {
        string inputText = @"- a
- b

- c
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 295 - https://github.github.com/gfm/#example-295
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 295 - https://github.github.com/gfm/#example-295")]
    [TestCategory("spec")]
    public void TestSpecExample295()
    {
        string inputText = @"* a
*

* c
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline)))
      (list_item
        (list_marker_star)
        (block_continuation))
      (list_item
        (list_marker_star)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 296 - https://github.github.com/gfm/#example-296
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 296 - https://github.github.com/gfm/#example-296")]
    [TestCategory("spec")]
    public void TestSpecExample296()
    {
        string inputText = @"- a
- b

  c
- d
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 297 - https://github.github.com/gfm/#example-297
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 297 - https://github.github.com/gfm/#example-297")]
    [TestCategory("spec")]
    public void TestSpecExample297()
    {
        string inputText = @"- a
- b

  [ref]: /url
- d
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_continuation)
        (link_reference_definition
          (link_label)
          (link_destination)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 298 - https://github.github.com/gfm/#example-298
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 298 - https://github.github.com/gfm/#example-298")]
    [TestCategory("spec")]
    public void TestSpecExample298()
    {
        string inputText = @"- a
- ```
  b


  ```
- c
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)))
      (list_item
        (list_marker_minus)
        (fenced_code_block
          (fenced_code_block_delimiter)
          (block_continuation)
          (code_fence_content
            (block_continuation)
            (block_continuation)
            (block_continuation))
          (fenced_code_block_delimiter)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 299 - https://github.github.com/gfm/#example-299
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 299 - https://github.github.com/gfm/#example-299")]
    [TestCategory("spec")]
    public void TestSpecExample299()
    {
        string inputText = @"- a
  - b

    c
- d
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation))
            (block_continuation)
            (paragraph
              (inline)))))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 300 - https://github.github.com/gfm/#example-300
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 300 - https://github.github.com/gfm/#example-300")]
    [TestCategory("spec")]
    public void TestSpecExample300()
    {
        string inputText = @"* a
  > b
  >
* c
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)
            (block_continuation))))
      (list_item
        (list_marker_star)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 301 - https://github.github.com/gfm/#example-301
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 301 - https://github.github.com/gfm/#example-301")]
    [TestCategory("spec")]
    public void TestSpecExample301()
    {
        string inputText = @"- a
  > b
  ```
  c
  ```
- d
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (block_quote
          (block_quote_marker)
          (paragraph
            (inline)
            (block_continuation)))
        (fenced_code_block
          (fenced_code_block_delimiter)
          (block_continuation)
          (code_fence_content
            (block_continuation))
          (fenced_code_block_delimiter)))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 302 - https://github.github.com/gfm/#example-302
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 302 - https://github.github.com/gfm/#example-302")]
    [TestCategory("spec")]
    public void TestSpecExample302()
    {
        string inputText = @"- a
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 303 - https://github.github.com/gfm/#example-303
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 303 - https://github.github.com/gfm/#example-303")]
    [TestCategory("spec")]
    public void TestSpecExample303()
    {
        string inputText = @"- a
  - b
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 304 - https://github.github.com/gfm/#example-304
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 304 - https://github.github.com/gfm/#example-304")]
    [TestCategory("spec")]
    public void TestSpecExample304()
    {
        string inputText = @"1. ```
   foo
   ```

   bar
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_dot)
        (fenced_code_block
          (fenced_code_block_delimiter)
          (block_continuation)
          (code_fence_content
            (block_continuation))
          (fenced_code_block_delimiter)
          (block_continuation))
        (block_continuation)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 305 - https://github.github.com/gfm/#example-305
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 305 - https://github.github.com/gfm/#example-305")]
    [TestCategory("spec")]
    public void TestSpecExample305()
    {
        string inputText = @"* foo
  * bar

  baz
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_star)
            (paragraph
              (inline)
              (block_continuation))
            (block_continuation)))
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 306 - https://github.github.com/gfm/#example-306
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 306 - https://github.github.com/gfm/#example-306")]
    [TestCategory("spec")]
    public void TestSpecExample306()
    {
        string inputText = @"- a
  - b
  - c

- d
  - e
  - f
";
        string expectedSyntaxTree = @"(document
  (section
    (list
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation)))
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation)))))
      (list_item
        (list_marker_minus)
        (paragraph
          (inline)
          (block_continuation))
        (list
          (list_item
            (list_marker_minus)
            (paragraph
              (inline)
              (block_continuation)))
          (list_item
            (list_marker_minus)
            (paragraph
              (inline))))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 314 - https://github.github.com/gfm/#example-314
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 314 - https://github.github.com/gfm/#example-314")]
    [TestCategory("spec")]
    public void TestSpecExample314()
    {
        string inputText = @"    \[\]
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 315 - https://github.github.com/gfm/#example-315
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 315 - https://github.github.com/gfm/#example-315")]
    [TestCategory("spec")]
    public void TestSpecExample315()
    {
        string inputText = @"~~~
\[\]
~~~
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 319 - https://github.github.com/gfm/#example-319
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 319 - https://github.github.com/gfm/#example-319")]
    [TestCategory("spec")]
    public void TestSpecExample319()
    {
        string inputText = @"[foo]

[foo]: /bar\* ""ti\*tle""
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (link_reference_definition
      (link_label)
      (link_destination
        (backslash_escape))
      (link_title
        (backslash_escape)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 320 - https://github.github.com/gfm/#example-320
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 320 - https://github.github.com/gfm/#example-320")]
    [TestCategory("spec")]
    public void TestSpecExample320()
    {
        string inputText = @"``` foo\+bar
foo
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language
          (backslash_escape)))
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 329 - https://github.github.com/gfm/#example-329
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 329 - https://github.github.com/gfm/#example-329")]
    [TestCategory("spec")]
    public void TestSpecExample329()
    {
        string inputText = @"[foo]

[foo]: /f&ouml;&ouml; ""f&ouml;&ouml;""
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (link_reference_definition
      (link_label)
      (link_destination
        (entity_reference)
        (entity_reference))
      (link_title
        (entity_reference)
        (entity_reference)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 330 - https://github.github.com/gfm/#example-330
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 330 - https://github.github.com/gfm/#example-330")]
    [TestCategory("spec")]
    public void TestSpecExample330()
    {
        string inputText = @"``` f&ouml;&ouml;
foo
```
";
        string expectedSyntaxTree = @"(document
  (section
    (fenced_code_block
      (fenced_code_block_delimiter)
      (info_string
        (language
          (entity_reference)
          (entity_reference)))
      (block_continuation)
      (code_fence_content
        (block_continuation))
      (fenced_code_block_delimiter))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 332 - https://github.github.com/gfm/#example-332
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 332 - https://github.github.com/gfm/#example-332")]
    [TestCategory("spec")]
    public void TestSpecExample332()
    {
        string inputText = @"    f&ouml;f&ouml;
";
        string expectedSyntaxTree = @"(document
  (section
    (indented_code_block)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 334 - https://github.github.com/gfm/#example-334
    /// From: tree-sitter-markdown/test/corpus/spec.txt
    /// </summary>
    [TestMethod("Example 334 - https://github.github.com/gfm/#example-334")]
    [TestCategory("spec")]
    public void TestSpecExample334()
    {
        string inputText = @"&#42; foo

* foo
";
        string expectedSyntaxTree = @"(document
  (section
    (paragraph
      (inline))
    (list
      (list_item
        (list_marker_star)
        (paragraph
          (inline))))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


}
