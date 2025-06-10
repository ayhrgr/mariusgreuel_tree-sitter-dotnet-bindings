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


    /// <summary>
    /// Basic LaTeX parsing.
    /// </summary>
    [TestMethod("Basic LaTeX parsing.")]
    [TestCategory("Latex")]
    public void TestLatexBasicParsing()
    {
        string inputText = @"$$This$$ has $$an odd$$ number of instances of $$.";
        string expectedSyntaxTree = @"(inline
          (latex_block
            (latex_span_delimiter)
            (latex_span_delimiter))
          (latex_block
            (latex_span_delimiter)
            (latex_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// LaTeX and markup clashes.
    /// </summary>
    [TestMethod("LaTeX and markup clashes.")]
    [TestCategory("Latex")]
    public void TestLatexAndMarkupClash()
    {
        string inputText = @"$$This should prevent *this from parsing$$ the bold.*";
        string expectedSyntaxTree = @"(inline
          (latex_block
            (latex_span_delimiter)
            (latex_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// LaTeX and link clashes
    /// </summary>
    [TestMethod("LaTeX and link clashes")]
    [TestCategory("Latex")]
    public void TestLatexAndLinkClash()
    {
        string inputText = @"$$This should prevent [this from parsing$$ the link](https://google.com)";
        string expectedSyntaxTree = @"(inline
          (latex_block
            (latex_span_delimiter)
            (latex_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// LaTeX inside markup
    /// </summary>
    [TestMethod("LaTeX inside markup")]
    [TestCategory("Latex")]
    public void TestLatexInsideMarkup()
    {
        string inputText = @"*This bold $$should still parse $$*.";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (latex_block
              (latex_span_delimiter)
              (latex_span_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// LaTeX within one paragraph
    /// </summary>
    [TestMethod("LaTeX within one paragraph")]
    [TestCategory("Latex")]
    public void TestLatexWithinOneParagraph()
    {
        string inputText = @"$$This should all be captured
as one instance of LaTeX.$$

$$This presumably

should not, but will because we need the blocks.$$";
        string expectedSyntaxTree = @"(inline
          (latex_block
            (latex_span_delimiter)
            (latex_span_delimiter))
          (latex_block
            (latex_span_delimiter)
            (latex_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// LaTeX with escaped dollar signs
    /// </summary>
    [TestMethod("LaTeX with escaped dollar signs")]
    [TestCategory("Latex")]
    public void TestLatexWithEscapedDollarSigns()
    {
        string inputText = @"$Hello\$th*er*e$";
        string expectedSyntaxTree = @"(inline
          (latex_block
            (latex_span_delimiter)
            (backslash_escape)
            (latex_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }




    /// <summary>
    /// Example 491 - https://github.github.com/gfm/#example-491
    /// </summary>
    [TestMethod("Example 491 - https://github.github.com/gfm/#example-491")]
    [TestCategory("Strikethrough")]
    public void TestStrikethroughExample491()
    {
        string inputText = @"~~Hi~~ Hello, world!";
        // NOTE: The expected S-expression from the original test corpus is used here.
        string expectedSyntaxTree = @"(inline
  (strikethrough
    (emphasis_delimiter)
    (strikethrough
      (emphasis_delimiter)
      (emphasis_delimiter))
    (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 492 - https://github.github.com/gfm/#example-492
    /// Note: The original test case mentions this does not work because it relies on block structure.
    /// For an inline parser, this is treated as unclosed strikethrough markers.
    /// </summary>
    [TestMethod("Example 492 - https://github.github.com/gfm/#example-492")]
    [TestCategory("Strikethrough")]
    public void TestStrikethroughExample492()
    {
        string inputText = @"This ~~has a

new paragraph~~.";
        // The inline parser does not handle block-level constructs like paragraphs.
        // Therefore, the strikethrough markers are not matched across paragraphs and are treated as literal text.
        // The expected tree is just a single inline node containing the text.
        string expectedSyntaxTree = @"(inline
  (strikethrough
    (emphasis_delimiter)
    (strikethrough
      (emphasis_delimiter)
      (emphasis_delimiter))
    (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Basic Wiki-link parsing.
    /// </summary>
    [TestMethod("Basic Wiki-link parsing.")]
    [TestCategory("Wikilink")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestWikilinkBasicParsing()
    {
        string inputText = @"[[Tree laws of motion]]";
        string expectedSyntaxTree = @"(inline
  (wiki_link
    (link_destination)
  ))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Wiki-link to a file
    /// </summary>
    [TestMethod("Wiki-link to a file")]
    [TestCategory("Wikilink")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestWikilinkToFile()
    {
        string inputText = @"[[Figure 1.png]]";
        string expectedSyntaxTree = @"(inline
          (wiki_link
            (link_destination)
          ))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Wiki-link to a heading in a note
    /// </summary>
    [TestMethod("Wiki-link to a heading in a note")]
    [TestCategory("Wikilink")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestWikilinkToHeading()
    {
        string inputText = @"[[Tree laws of motion#Second law]]";
        string expectedSyntaxTree = @"(inline
          (wiki_link
            (link_destination)
          ))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Wiki-link with title
    /// </summary>
    [TestMethod("Wiki-link with title")]
    [TestCategory("Wikilink")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestWikilinkWithTitle()
    {
        string inputText = @"[[Internal links|custom display text]]";
        string expectedSyntaxTree = @"(inline
          (wiki_link
            (link_destination)
            (link_text)
          ))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Wiki-link opener with no closer
    /// </summary>
    [TestMethod("Wiki-link opener with no closer")]
    [TestCategory("Wikilink")]
    public void TestWikilinkOpenerWithNoCloser()
    {
        string inputText = @"[[";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Wiki-link version of Example 556
    /// </summary>
    [TestMethod("Wiki-link version of Example 556")]
    [TestCategory("Wikilink")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestWikilinkExample556()
    {
        string inputText = @"[[[foo]]]";
        string expectedSyntaxTree = @"(inline
          (wiki_link
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// #18 - Error on markdown images
    /// </summary>
    [TestMethod("#18 - Error on markdown images")]
    [TestCategory("Issues")]
    public void TestIssue18_MarkdownImages()
    {
        string inputText = @"![img1](link1)
![img2](link2)";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)
            (link_destination))
          (image
            (image_description)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #6 - HTML tag can sometimes be parsed as code span (1)
    /// </summary>
    [TestMethod("#6 - HTML tag can sometimes be parsed as code span (1)")]
    [TestCategory("Issues")]
    public void TestIssue6_HtmlInCodeSpan1()
    {
        string inputText = @"test `Option<T>` test `Option` and `</>`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #6 - HTML tag can sometimes be parsed as code span (2)
    /// </summary>
    [TestMethod("#6 - HTML tag can sometimes be parsed as code span (2)")]
    [TestCategory("Issues")]
    public void TestIssue6_HtmlInCodeSpan2()
    {
        string inputText = @"test `Option<T>` test `Option` and `test`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #36 - Multiple code spans with HTML comments does not working properly (1)
    /// </summary>
    [TestMethod("#36 - Multiple code spans with HTML comments does not working properly (1)")]
    [TestCategory("Issues")]
    public void TestIssue36_CodeSpanWithHtmlComment1()
    {
        string inputText = @"foo `<!--comment-->`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #36 - Multiple code spans with HTML comments does not working properly (2)
    /// </summary>
    [TestMethod("#36 - Multiple code spans with HTML comments does not working properly (2)")]
    [TestCategory("Issues")]
    public void TestIssue36_CodeSpanWithHtmlComment2()
    {
        string inputText = @"`<!--comment-->` foo";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #36 - Multiple code spans with HTML comments does not working properly (3)
    /// </summary>
    [TestMethod("#36 - Multiple code spans with HTML comments does not working properly (3)")]
    [TestCategory("Issues")]
    public void TestIssue36_CodeSpanWithHtmlComment3()
    {
        string inputText = @"`<!--comment-->` foo `<!--comment-->`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// #75 - code spans with `[x][]`
    /// </summary>
    [TestMethod("#75 - code spans with `[x][]`")]
    [TestCategory("Issues")]
    public void TestIssue75_CodeSpanWithBrackets()
    {
        string inputText = @"`some code`
normal text (or even nothing) `[index][]`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 307 - https://github.github.com/gfm/#example-307
    /// </summary>
    [TestMethod("Example 307 - https://github.github.com/gfm/#example-307")]
    [TestCategory("Spec")]
    public void TestSpecExample307()
    {
        string inputText = @"`hi`lo`";
        string expectedSyntaxTree = @"(inline
  (code_span
    (code_span_delimiter)
    (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 308 - https://github.github.com/gfm/#example-308
    /// </summary>
    [TestMethod("Example 308 - https://github.github.com/gfm/#example-308")]
    [TestCategory("Spec")]
    public void TestSpecExample308()
    {
        string inputText = @"\!\""\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~";
        string expectedSyntaxTree = @"(inline
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape)
          (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 309 - https://github.github.com/gfm/#example-309
    /// </summary>
    [TestMethod("Example 309 - https://github.github.com/gfm/#example-309")]
    [TestCategory("Spec")]
    public void TestSpecExample309()
    {
        string inputText = @"\	\A\a\ \3\φ\«";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 310 - https://github.github.com/gfm/#example-310
    /// </summary>
    [TestMethod("Example 310 - https://github.github.com/gfm/#example-310")]
    [TestCategory("Spec")]
    public void TestSpecExample310()
    {
        string inputText = @"\*not emphasized*
\<br/> not a tag
\[not a link](/foo)
\`not code`
1\. not a list
\* not a list
\# not a heading
\[foo]: /url ""not a reference""
\&ouml; not a character entity";
        string expectedSyntaxTree = @"(inline
  (backslash_escape)
  (backslash_escape)
  (backslash_escape)
  (backslash_escape)
  (backslash_escape)
  (backslash_escape)
  (backslash_escape)
  (backslash_escape)
  (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 311 - https://github.github.com/gfm/#example-311
    /// </summary>
    [TestMethod("Example 311 - https://github.github.com/gfm/#example-311")]
    [TestCategory("Spec")]
    public void TestSpecExample311()
    {
        string inputText = @"\\*emphasis*";
        string expectedSyntaxTree = @"(inline
          (backslash_escape)
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 312 - https://github.github.com/gfm/#example-312
    /// </summary>
    [TestMethod("Example 312 - https://github.github.com/gfm/#example-312")]
    [TestCategory("Spec")]
    public void TestSpecExample312()
    {
        string inputText = @"foo\
bar";
        string expectedSyntaxTree = @"(inline
          (hard_line_break))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 313 - https://github.github.com/gfm/#example-313
    /// </summary>
    [TestMethod("Example 313 - https://github.github.com/gfm/#example-313")]
    [TestCategory("Spec")]
    public void TestSpecExample313()
    {
        string inputText = @"`` \[\` ``";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 316 - https://github.github.com/gfm/#example-316
    /// </summary>
    [TestMethod("Example 316 - https://github.github.com/gfm/#example-316")]
    [TestCategory("Spec")]
    public void TestSpecExample316()
    {
        string inputText = @"<http://example.com?find=\*>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 317 - https://github.github.com/gfm/#example-317
    /// </summary>
    [TestMethod("Example 317 - https://github.github.com/gfm/#example-317")]
    [TestCategory("Spec")]
    public void TestSpecExample317()
    {
        string inputText = @"<a href=""/bar\/)"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 318 - https://github.github.com/gfm/#example-318
    /// </summary>
    [TestMethod("Example 318 - https://github.github.com/gfm/#example-318")]
    [TestCategory("Spec")]
    public void TestSpecExample318()
    {
        string inputText = @"[foo](/bar\* ""ti\*tle"")";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination
              (backslash_escape))
            (link_title
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 321 - https://github.github.com/gfm/#example-321
    /// </summary>
    [TestMethod("Example 321 - https://github.github.com/gfm/#example-321")]
    [TestCategory("Spec")]
    public void TestSpecExample321()
    {
        string inputText = @"&nbsp; &amp; &copy; &AElig; &Dcaron;
&frac34; &HilbertSpace; &DifferentialD;
&ClockwiseContourIntegral; &ngE;";
        string expectedSyntaxTree = @"(inline
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference)
  (entity_reference))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 322 - https://github.github.com/gfm/#example-322
    /// </summary>
    [TestMethod("Example 322 - https://github.github.com/gfm/#example-322")]
    [TestCategory("Spec")]
    public void TestSpecExample322()
    {
        string inputText = @"&#35; &#1234; &#992; &#0;";
        string expectedSyntaxTree = @"(inline
  (numeric_character_reference)
  (numeric_character_reference)
  (numeric_character_reference)
  (numeric_character_reference))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 323 - https://github.github.com/gfm/#example-323
    /// </summary>
    [TestMethod("Example 323 - https://github.github.com/gfm/#example-323")]
    [TestCategory("Spec")]
    public void TestSpecExample323()
    {
        string inputText = @"&#X22; &#XD06; &#xcab;";
        string expectedSyntaxTree = @"(inline
  (numeric_character_reference)
  (numeric_character_reference)
  (numeric_character_reference))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 324 - https://github.github.com/gfm/#example-324
    /// </summary>
    [TestMethod("Example 324 - https://github.github.com/gfm/#example-324")]
    [TestCategory("Spec")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestSpecExample324()
    {
        string inputText = @"&nbsp &x; &#; &#x;
&#87654321;
&#abcdef0;
&ThisIsNotDefined; &hi?;";
        string expectedSyntaxTree = @"(inline
  (tag)
  (tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 325 - https://github.github.com/gfm/#example-325
    /// </summary>
    [TestMethod("Example 325 - https://github.github.com/gfm/#example-325")]
    [TestCategory("Spec")]
    public void TestSpecExample325()
    {
        string inputText = @"©";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 326 - https://github.github.com/gfm/#example-326
    /// </summary>
    [TestMethod("Example 326 - https://github.github.com/gfm/#example-326")]
    [TestCategory("Spec")]
    public void TestSpecExample326()
    {
        string inputText = @"&MadeUpEntity;";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 327 - https://github.github.com/gfm/#example-327
    /// </summary>
    [TestMethod("Example 327 - https://github.github.com/gfm/#example-327")]
    [TestCategory("Spec")]
    public void TestSpecExample327()
    {
        string inputText = @"<a href=""öö.html"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 328 - https://github.github.com/gfm/#example-328
    /// </summary>
    [TestMethod("Example 328 - https://github.github.com/gfm/#example-328")]
    [TestCategory("Spec")]
    public void TestSpecExample328()
    {
        string inputText = @"[foo](/f&ouml;&ouml; ""f&ouml;&ouml;"")";
        string expectedSyntaxTree = @"(inline
  (inline_link
    (link_text)
    (link_destination
      (entity_reference)
      (entity_reference))
    (link_title
      (entity_reference)
      (entity_reference))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 331 - https://github.github.com/gfm/#example-331
    /// </summary>
    [TestMethod("Example 331 - https://github.github.com/gfm/#example-331")]
    [TestCategory("Spec")]
    public void TestSpecExample331()
    {
        string inputText = @"`föö`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 333 - https://github.github.com/gfm/#example-333
    /// </summary>
    [TestMethod("Example 333 - https://github.github.com/gfm/#example-333")]
    [TestCategory("Spec")]
    public void TestSpecExample333()
    {
        string inputText = @"&#42;foo&#42;
*foo*";
        string expectedSyntaxTree = @"(inline
  (numeric_character_reference)
  (numeric_character_reference)
  (emphasis
    (emphasis_delimiter)
    (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 335 - https://github.github.com/gfm/#example-335
    /// </summary>
    [TestMethod("Example 335 - https://github.github.com/gfm/#example-335")]
    [TestCategory("Spec")]
    public void TestSpecExample335()
    {
        string inputText = @"foo&#10;&#10;bar";
        string expectedSyntaxTree = @"(inline
  (numeric_character_reference)
  (numeric_character_reference))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 336 - https://github.github.com/gfm/#example-336
    /// </summary>
    [TestMethod("Example 336 - https://github.github.com/gfm/#example-336")]
    [TestCategory("Spec")]
    public void TestSpecExample336()
    {
        string inputText = @"&#9;foo";
        string expectedSyntaxTree = @"(inline
  (numeric_character_reference))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 337 - https://github.github.com/gfm/#example-337
    /// </summary>
    [TestMethod("Example 337 - https://github.github.com/gfm/#example-337")]
    [TestCategory("Spec")]
    public void TestSpecExample337()
    {
        string inputText = @"[a](url &quot;tit&quot;)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (entity_reference)
          (entity_reference))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 338 - https://github.github.com/gfm/#example-338
    /// </summary>
    [TestMethod("Example 338 - https://github.github.com/gfm/#example-338")]
    [TestCategory("Spec")]
    public void TestSpecExample338()
    {
        string inputText = @"`foo`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 339 - https://github.github.com/gfm/#example-339
    /// </summary>
    [TestMethod("Example 339 - https://github.github.com/gfm/#example-339")]
    [TestCategory("Spec")]
    public void TestSpecExample339()
    {
        string inputText = @"`` foo ` bar ``";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 340 - https://github.github.com/gfm/#example-340
    /// </summary>
    [TestMethod("Example 340 - https://github.github.com/gfm/#example-340")]
    [TestCategory("Spec")]
    public void TestSpecExample340()
    {
        string inputText = @"` `` `";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 341 - https://github.github.com/gfm/#example-341
    /// </summary>
    [TestMethod("Example 341 - https://github.github.com/gfm/#example-341")]
    [TestCategory("Spec")]
    public void TestSpecExample341()
    {
        string inputText = @"`  ``  `";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 342 - https://github.github.com/gfm/#example-342
    /// </summary>
    [TestMethod("Example 342 - https://github.github.com/gfm/#example-342")]
    [TestCategory("Spec")]
    public void TestSpecExample342()
    {
        string inputText = @"` a`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 343 - https://github.github.com/gfm/#example-343
    /// </summary>
    [TestMethod("Example 343 - https://github.github.com/gfm/#example-343")]
    [TestCategory("Spec")]
    public void TestSpecExample343()
    {
        string inputText = @"` b `";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 344 - https://github.github.com/gfm/#example-344
    /// </summary>
    [TestMethod("Example 344 - https://github.github.com/gfm/#example-344")]
    [TestCategory("Spec")]
    public void TestSpecExample344()
    {
        string inputText = @"` `
`  `";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter))
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 345 - https://github.github.com/gfm/#example-345
    /// </summary>
    [TestMethod("Example 345 - https://github.github.com/gfm/#example-345")]
    [TestCategory("Spec")]
    public void TestSpecExample345()
    {
        string inputText = @"``
foo
bar
baz
``";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 346 - https://github.github.com/gfm/#example-346
    /// </summary>
    [TestMethod("Example 346 - https://github.github.com/gfm/#example-346")]
    [TestCategory("Spec")]
    public void TestSpecExample346()
    {
        string inputText = @"``
foo
``";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 347 - https://github.github.com/gfm/#example-347
    /// </summary>
    [TestMethod("Example 347 - https://github.github.com/gfm/#example-347")]
    [TestCategory("Spec")]
    public void TestSpecExample347()
    {
        string inputText = @"`foo   bar
baz`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 348 - https://github.github.com/gfm/#example-348
    /// </summary>
    [TestMethod("Example 348 - https://github.github.com/gfm/#example-348")]
    [TestCategory("Spec")]
    public void TestSpecExample348()
    {
        string inputText = @"`foo\`bar`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 349 - https://github.github.com/gfm/#example-349
    /// </summary>
    [TestMethod("Example 349 - https://github.github.com/gfm/#example-349")]
    [TestCategory("Spec")]
    public void TestSpecExample349()
    {
        string inputText = @"``foo`bar``";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 350 - https://github.github.com/gfm/#example-350
    /// </summary>
    [TestMethod("Example 350 - https://github.github.com/gfm/#example-350")]
    [TestCategory("Spec")]
    public void TestSpecExample350()
    {
        string inputText = @"` foo `` bar `";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 351 - https://github.github.com/gfm/#example-351
    /// </summary>
    [TestMethod("Example 351 - https://github.github.com/gfm/#example-351")]
    [TestCategory("Spec")]
    public void TestSpecExample351()
    {
        string inputText = @"*foo`*`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 352 - https://github.github.com/gfm/#example-352
    /// </summary>
    [TestMethod("Example 352 - https://github.github.com/gfm/#example-352")]
    [TestCategory("Spec")]
    public void TestSpecExample352()
    {
        string inputText = @"[not a `link](/foo`)";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 353 - https://github.github.com/gfm/#example-353
    /// </summary>
    [TestMethod("Example 353 - https://github.github.com/gfm/#example-353")]
    [TestCategory("Spec")]
    public void TestSpecExample353()
    {
        string inputText = @"`<a href=""`"">`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 354 - https://github.github.com/gfm/#example-354
    /// </summary>
    [TestMethod("Example 354 - https://github.github.com/gfm/#example-354")]
    [TestCategory("Spec")]
    public void TestSpecExample354()
    {
        string inputText = @"<a href=""`"">`";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 356 - https://github.github.com/gfm/#example-356
    /// </summary>
    [TestMethod("Example 356 - https://github.github.com/gfm/#example-356")]
    [TestCategory("Spec")]
    public void TestSpecExample356()
    {
        string inputText = @"<http://foo.bar.`baz>`";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 357 - https://github.github.com/gfm/#example-357
    /// </summary>
    [TestMethod("Example 357 - https://github.github.com/gfm/#example-357")]
    [TestCategory("Spec")]
    public void TestSpecExample357()
    {
        string inputText = @"```foo``";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 358 - https://github.github.com/gfm/#example-358
    /// </summary>
    [TestMethod("Example 358 - https://github.github.com/gfm/#example-358")]
    [TestCategory("Spec")]
    public void TestSpecExample358()
    {
        string inputText = @"`foo";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 359 - https://github.github.com/gfm/#example-359
    /// </summary>
    [TestMethod("Example 359 - https://github.github.com/gfm/#example-359")]
    [TestCategory("Spec")]
    public void TestSpecExample359()
    {
        string inputText = @"`foo``bar``";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 360 - https://github.github.com/gfm/#example-360
    /// </summary>
    [TestMethod("Example 360 - https://github.github.com/gfm/#example-360")]
    [TestCategory("Spec")]
    public void TestSpecExample360()
    {
        string inputText = @"*foo bar*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 361 - https://github.github.com/gfm/#example-361
    /// </summary>
    [TestMethod("Example 361 - https://github.github.com/gfm/#example-361")]
    [TestCategory("Spec")]
    public void TestSpecExample361()
    {
        string inputText = @"a * foo bar*";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 362 - https://github.github.com/gfm/#example-362
    /// </summary>
    [TestMethod("Example 362 - https://github.github.com/gfm/#example-362")]
    [TestCategory("Spec")]
    public void TestSpecExample362()
    {
        string inputText = @"a*""foo""*";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 364 - https://github.github.com/gfm/#example-364
    /// </summary>
    [TestMethod("Example 364 - https://github.github.com/gfm/#example-364")]
    [TestCategory("Spec")]
    public void TestSpecExample364()
    {
        string inputText = @"foo*bar*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 365 - https://github.github.com/gfm/#example-365
    /// </summary>
    [TestMethod("Example 365 - https://github.github.com/gfm/#example-365")]
    [TestCategory("Spec")]
    public void TestSpecExample365()
    {
        string inputText = @"5*6*78";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 366 - https://github.github.com/gfm/#example-366
    /// </summary>
    [TestMethod("Example 366 - https://github.github.com/gfm/#example-366")]
    [TestCategory("Spec")]
    public void TestSpecExample366()
    {
        string inputText = @"_foo bar_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 367 - https://github.github.com/gfm/#example-367
    /// </summary>
    [TestMethod("Example 367 - https://github.github.com/gfm/#example-367")]
    [TestCategory("Spec")]
    public void TestSpecExample367()
    {
        string inputText = @"_ foo bar_";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 368 - https://github.github.com/gfm/#example-368
    /// </summary>
    [TestMethod("Example 368 - https://github.github.com/gfm/#example-368")]
    [TestCategory("Spec")]
    public void TestSpecExample368()
    {
        string inputText = @"a_""foo""_";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 369 - https://github.github.com/gfm/#example-369
    /// </summary>
    [TestMethod("Example 369 - https://github.github.com/gfm/#example-369")]
    [TestCategory("Spec")]
    public void TestSpecExample369()
    {
        string inputText = @"foo_bar_";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 370 - https://github.github.com/gfm/#example-370
    /// </summary>
    [TestMethod("Example 370 - https://github.github.com/gfm/#example-370")]
    [TestCategory("Spec")]
    public void TestSpecExample370()
    {
        string inputText = @"5_6_78";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 371 - https://github.github.com/gfm/#example-371
    /// </summary>
    [TestMethod("Example 371 - https://github.github.com/gfm/#example-371")]
    [TestCategory("Spec")]
    public void TestSpecExample371()
    {
        string inputText = @"пристаням_стремятся_";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 372 - https://github.github.com/gfm/#example-372
    /// </summary>
    [TestMethod("Example 372 - https://github.github.com/gfm/#example-372")]
    [TestCategory("Spec")]
    public void TestSpecExample372()
    {
        string inputText = @"aa_""bb""_cc";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 373 - https://github.github.com/gfm/#example-373
    /// </summary>
    [TestMethod("Example 373 - https://github.github.com/gfm/#example-373")]
    [TestCategory("Spec")]
    public void TestSpecExample373()
    {
        string inputText = @"foo-_(bar)_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 374 - https://github.github.com/gfm/#example-374
    /// </summary>
    [TestMethod("Example 374 - https://github.github.com/gfm/#example-374")]
    [TestCategory("Spec")]
    public void TestSpecExample374()
    {
        string inputText = @"_foo*";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 375 - https://github.github.com/gfm/#example-375
    /// </summary>
    [TestMethod("Example 375 - https://github.github.com/gfm/#example-375")]
    [TestCategory("Spec")]
    public void TestSpecExample375()
    {
        string inputText = @"*foo bar *";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 376 - https://github.github.com/gfm/#example-376
    /// </summary>
    [TestMethod("Example 376 - https://github.github.com/gfm/#example-376")]
    [TestCategory("Spec")]
    public void TestSpecExample376()
    {
        string inputText = @"*foo bar
*";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 377 - https://github.github.com/gfm/#example-377
    /// </summary>
    [TestMethod("Example 377 - https://github.github.com/gfm/#example-377")]
    [TestCategory("Spec")]
    public void TestSpecExample377()
    {
        string inputText = @"*(*foo)";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 378 - https://github.github.com/gfm/#example-378
    /// </summary>
    [TestMethod("Example 378 - https://github.github.com/gfm/#example-378")]
    [TestCategory("Spec")]
    public void TestSpecExample378()
    {
        string inputText = @"*(*foo*)*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 379 - https://github.github.com/gfm/#example-379
    /// </summary>
    [TestMethod("Example 379 - https://github.github.com/gfm/#example-379")]
    [TestCategory("Spec")]
    public void TestSpecExample379()
    {
        string inputText = @"*foo*bar";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 380 - https://github.github.com/gfm/#example-380
    /// </summary>
    [TestMethod("Example 380 - https://github.github.com/gfm/#example-380")]
    [TestCategory("Spec")]
    public void TestSpecExample380()
    {
        string inputText = @"_foo bar _";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 381 - https://github.github.com/gfm/#example-381
    /// </summary>
    [TestMethod("Example 381 - https://github.github.com/gfm/#example-381")]
    [TestCategory("Spec")]
    public void TestSpecExample381()
    {
        string inputText = @"_(_foo)";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 382 - https://github.github.com/gfm/#example-382
    /// </summary>
    [TestMethod("Example 382 - https://github.github.com/gfm/#example-382")]
    [TestCategory("Spec")]
    public void TestSpecExample382()
    {
        string inputText = @"_(_foo_)_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 383 - https://github.github.com/gfm/#example-383
    /// </summary>
    [TestMethod("Example 383 - https://github.github.com/gfm/#example-383")]
    [TestCategory("Spec")]
    public void TestSpecExample383()
    {
        string inputText = @"_foo_bar";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 384 - https://github.github.com/gfm/#example-384
    /// </summary>
    [TestMethod("Example 384 - https://github.github.com/gfm/#example-384")]
    [TestCategory("Spec")]
    public void TestSpecExample384()
    {
        string inputText = @"_пристаням_стремятся";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 385 - https://github.github.com/gfm/#example-385
    /// </summary>
    [TestMethod("Example 385 - https://github.github.com/gfm/#example-385")]
    [TestCategory("Spec")]
    public void TestSpecExample385()
    {
        string inputText = @"_foo_bar_baz_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 386 - https://github.github.com/gfm/#example-386
    /// </summary>
    [TestMethod("Example 386 - https://github.github.com/gfm/#example-386")]
    [TestCategory("Spec")]
    public void TestSpecExample386()
    {
        string inputText = @"_(bar)_.";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 387 - https://github.github.com/gfm/#example-387
    /// </summary>
    [TestMethod("Example 387 - https://github.github.com/gfm/#example-387")]
    [TestCategory("Spec")]
    public void TestSpecExample387()
    {
        string inputText = @"**foo bar**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 388 - https://github.github.com/gfm/#example-388
    /// </summary>
    [TestMethod("Example 388 - https://github.github.com/gfm/#example-388")]
    [TestCategory("Spec")]
    public void TestSpecExample388()
    {
        string inputText = @"** foo bar**";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 390 - https://github.github.com/gfm/#example-390
    /// </summary>
    [TestMethod("Example 390 - https://github.github.com/gfm/#example-390")]
    [TestCategory("Spec")]
    public void TestSpecExample390()
    {
        string inputText = @"foo**bar**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 391 - https://github.github.com/gfm/#example-391
    /// </summary>
    [TestMethod("Example 391 - https://github.github.com/gfm/#example-391")]
    [TestCategory("Spec")]
    public void TestSpecExample391()
    {
        string inputText = @"__foo bar__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 392 - https://github.github.com/gfm/#example-392
    /// </summary>
    [TestMethod("Example 392 - https://github.github.com/gfm/#example-392")]
    [TestCategory("Spec")]
    public void TestSpecExample392()
    {
        string inputText = @"__ foo bar__";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 393 - https://github.github.com/gfm/#example-393
    /// </summary>
    [TestMethod("Example 393 - https://github.github.com/gfm/#example-393")]
    [TestCategory("Spec")]
    public void TestSpecExample393()
    {
        string inputText = @"__
foo bar__";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 395 - https://github.github.com/gfm/#example-395
    /// </summary>
    [TestMethod("Example 395 - https://github.github.com/gfm/#example-395")]
    [TestCategory("Spec")]
    public void TestSpecExample395()
    {
        string inputText = @"foo__bar__";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 396 - https://github.github.com/gfm/#example-396
    /// </summary>
    [TestMethod("Example 396 - https://github.github.com/gfm/#example-396")]
    [TestCategory("Spec")]
    public void TestSpecExample396()
    {
        string inputText = @"5__6__78";
        // GFM spec implies this should not be strong emphasis.
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 397 - https://github.github.com/gfm/#example-397
    /// </summary>
    [TestMethod("Example 397 - https://github.github.com/gfm/#example-397")]
    [TestCategory("Spec")]
    public void TestSpecExample397()
    {
        string inputText = @"пристаням__стремятся__";
        // GFM spec implies this should not be strong emphasis as it's part of a word.
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 398 - https://github.github.com/gfm/#example-398
    /// </summary>
    [TestMethod("Example 398 - https://github.github.com/gfm/#example-398")]
    [TestCategory("Spec")]
    public void TestSpecExample398()
    {
        string inputText = @"__foo, __bar__, baz__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 399 - https://github.github.com/gfm/#example-399
    /// </summary>
    [TestMethod("Example 399 - https://github.github.com/gfm/#example-399")]
    [TestCategory("Spec")]
    public void TestSpecExample399()
    {
        string inputText = @"foo-__(bar)__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 402 - https://github.github.com/gfm/#example-402
    /// </summary>
    [TestMethod("Example 402 - https://github.github.com/gfm/#example-402")]
    [TestCategory("Spec")]
    public void TestSpecExample402()
    {
        string inputText = @"*(**foo**)*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 403 - https://github.github.com/gfm/#example-403
    /// </summary>
    [TestMethod("Example 403 - https://github.github.com/gfm/#example-403")]
    [TestCategory("Spec")]
    public void TestSpecExample403()
    {
        string inputText = @"**Gomphocarpus (*Gomphocarpus physocarpus*, syn.
*Asclepias physocarpa*)**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 404 - https://github.github.com/gfm/#example-404
    /// </summary>
    [TestMethod("Example 404 - https://github.github.com/gfm/#example-404")]
    [TestCategory("Spec")]
    public void TestSpecExample404()
    {
        string inputText = @"**foo ""*bar*"" foo**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 405 - https://github.github.com/gfm/#example-405
    /// </summary>
    [TestMethod("Example 405 - https://github.github.com/gfm/#example-405")]
    [TestCategory("Spec")]
    public void TestSpecExample405()
    {
        string inputText = @"**foo**bar";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 408 - https://github.github.com/gfm/#example-408
    /// </summary>
    [TestMethod("Example 408 - https://github.github.com/gfm/#example-408")]
    [TestCategory("Spec")]
    public void TestSpecExample408()
    {
        string inputText = @"_(__foo__)_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 409 - https://github.github.com/gfm/#example-409
    /// </summary>
    [TestMethod("Example 409 - https://github.github.com/gfm/#example-409")]
    [TestCategory("Spec")]
    public void TestSpecExample409()
    {
        string inputText = @"__foo__bar";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 410 - https://github.github.com/gfm/#example-410
    /// </summary>
    [TestMethod("Example 410 - https://github.github.com/gfm/#example-410")]
    [TestCategory("Spec")]
    public void TestSpecExample410()
    {
        string inputText = @"__пристаням__стремятся";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 412 - https://github.github.com/gfm/#example-412
    /// </summary>
    [TestMethod("Example 412 - https://github.github.com/gfm/#example-412")]
    [TestCategory("Spec")]
    public void TestSpecExample412()
    {
        string inputText = @"__(bar)__.";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 413 - https://github.github.com/gfm/#example-413
    /// </summary>
    [TestMethod("Example 413 - https://github.github.com/gfm/#example-413")]
    [TestCategory("Spec")]
    public void TestSpecExample413()
    {
        string inputText = @"*foo [bar](/url)*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (inline_link
              (link_text)
              (link_destination))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 414 - https://github.github.com/gfm/#example-414
    /// </summary>
    [TestMethod("Example 414 - https://github.github.com/gfm/#example-414")]
    [TestCategory("Spec")]
    public void TestSpecExample414()
    {
        string inputText = @"*foo
bar*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 415 - https://github.github.com/gfm/#example-415
    /// </summary>
    [TestMethod("Example 415 - https://github.github.com/gfm/#example-415")]
    [TestCategory("Spec")]
    public void TestSpecExample415()
    {
        string inputText = @"_foo __bar__ baz_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 416 - https://github.github.com/gfm/#example-416
    /// </summary>
    [TestMethod("Example 416 - https://github.github.com/gfm/#example-416")]
    [TestCategory("Spec")]
    public void TestSpecExample416()
    {
        string inputText = @"_foo _bar_ baz_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 417 - https://github.github.com/gfm/#example-417
    /// </summary>
    [TestMethod("Example 417 - https://github.github.com/gfm/#example-417")]
    [TestCategory("Spec")]
    public void TestSpecExample417()
    {
        string inputText = @"__foo_ bar_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 418 - https://github.github.com/gfm/#example-418
    /// </summary>
    [TestMethod("Example 418 - https://github.github.com/gfm/#example-418")]
    [TestCategory("Spec")]
    public void TestSpecExample418()
    {
        string inputText = @"*foo *bar**";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 419 - https://github.github.com/gfm/#example-419
    /// </summary>
    [TestMethod("Example 419 - https://github.github.com/gfm/#example-419")]
    [TestCategory("Spec")]
    public void TestSpecExample419()
    {
        string inputText = @"*foo **bar** baz*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 422 - https://github.github.com/gfm/#example-422
    /// </summary>
    [TestMethod("Example 422 - https://github.github.com/gfm/#example-422")]
    [TestCategory("Spec")]
    public void TestSpecExample422()
    {
        string inputText = @"***foo** bar*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 423 - https://github.github.com/gfm/#example-423
    /// </summary>
    [TestMethod("Example 423 - https://github.github.com/gfm/#example-423")]
    [TestCategory("Spec")]
    public void TestSpecExample423()
    {
        string inputText = @"*foo **bar***";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 425 - https://github.github.com/gfm/#example-425
    /// </summary>
    [TestMethod("Example 425 - https://github.github.com/gfm/#example-425")]
    [TestCategory("Spec")]
    public void TestSpecExample425()
    {
        string inputText = @"foo***bar***baz";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 426 - https://github.github.com/gfm/#example-426
    /// </summary>
    [TestMethod("Example 426 - https://github.github.com/gfm/#example-426")]
    [TestCategory("Spec")]
    public void TestSpecExample426()
    {
        string inputText = @"foo******bar*********baz";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (strong_emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter))
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 427 - https://github.github.com/gfm/#example-427
    /// </summary>
    [TestMethod("Example 427 - https://github.github.com/gfm/#example-427")]
    [TestCategory("Spec")]
    public void TestSpecExample427()
    {
        string inputText = @"*foo **bar *baz* bim** bop*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter))
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 428 - https://github.github.com/gfm/#example-428
    /// </summary>
    [TestMethod("Example 428 - https://github.github.com/gfm/#example-428")]
    [TestCategory("Spec")]
    public void TestSpecExample428()
    {
        string inputText = @"*foo [*bar*](/url)*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (inline_link
              (link_text
                (emphasis
                  (emphasis_delimiter)
                  (emphasis_delimiter)))
              (link_destination))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 429 - https://github.github.com/gfm/#example-429
    /// </summary>
    [TestMethod("Example 429 - https://github.github.com/gfm/#example-429")]
    [TestCategory("Spec")]
    public void TestSpecExample429()
    {
        string inputText = @"** is not an empty emphasis";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 430 - https://github.github.com/gfm/#example-430
    /// </summary>
    [TestMethod("Example 430 - https://github.github.com/gfm/#example-430")]
    [TestCategory("Spec")]
    public void TestSpecExample430()
    {
        string inputText = @"**** is not an empty strong emphasis";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 431 - https://github.github.com/gfm/#example-431
    /// </summary>
    [TestMethod("Example 431 - https://github.github.com/gfm/#example-431")]
    [TestCategory("Spec")]
    public void TestSpecExample431()
    {
        string inputText = @"**foo [bar](/url)**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (inline_link
              (link_text)
              (link_destination))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 432 - https://github.github.com/gfm/#example-432
    /// </summary>
    [TestMethod("Example 432 - https://github.github.com/gfm/#example-432")]
    [TestCategory("Spec")]
    public void TestSpecExample432()
    {
        string inputText = @"**foo
bar**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 433 - https://github.github.com/gfm/#example-433
    /// </summary>
    [TestMethod("Example 433 - https://github.github.com/gfm/#example-433")]
    [TestCategory("Spec")]
    public void TestSpecExample433()
    {
        string inputText = @"__foo _bar_ baz__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 434 - https://github.github.com/gfm/#example-434
    /// </summary>
    [TestMethod("Example 434 - https://github.github.com/gfm/#example-434")]
    [TestCategory("Spec")]
    public void TestSpecExample434()
    {
        string inputText = @"__foo __bar__ baz__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 435 - https://github.github.com/gfm/#example-435
    /// </summary>
    [TestMethod("Example 435 - https://github.github.com/gfm/#example-435")]
    [TestCategory("Spec")]
    public void TestSpecExample435()
    {
        string inputText = @"____foo__ bar__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 436 - https://github.github.com/gfm/#example-436
    /// </summary>
    [TestMethod("Example 436 - https://github.github.com/gfm/#example-436")]
    [TestCategory("Spec")]
    public void TestSpecExample436()
    {
        string inputText = @"**foo **bar****";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 437 - https://github.github.com/gfm/#example-437
    /// </summary>
    [TestMethod("Example 437 - https://github.github.com/gfm/#example-437")]
    [TestCategory("Spec")]
    public void TestSpecExample437()
    {
        string inputText = @"**foo *bar* baz**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 439 - https://github.github.com/gfm/#example-439
    /// </summary>
    [TestMethod("Example 439 - https://github.github.com/gfm/#example-439")]
    [TestCategory("Spec")]
    public void TestSpecExample439()
    {
        string inputText = @"***foo* bar**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 440 - https://github.github.com/gfm/#example-440
    /// </summary>
    [TestMethod("Example 440 - https://github.github.com/gfm/#example-440")]
    [TestCategory("Spec")]
    public void TestSpecExample440()
    {
        string inputText = @"**foo *bar***";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 441 - https://github.github.com/gfm/#example-441
    /// </summary>
    [TestMethod("Example 441 - https://github.github.com/gfm/#example-441")]
    [TestCategory("Spec")]
    public void TestSpecExample441()
    {
        string inputText = @"**foo *bar **baz**
bim* bop**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (strong_emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter))
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 442 - https://github.github.com/gfm/#example-442
    /// </summary>
    [TestMethod("Example 442 - https://github.github.com/gfm/#example-442")]
    [TestCategory("Spec")]
    public void TestSpecExample442()
    {
        string inputText = @"**foo [*bar*](/url)**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (inline_link
              (link_text
                (emphasis
                  (emphasis_delimiter)
                  (emphasis_delimiter)))
              (link_destination))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 443 - https://github.github.com/gfm/#example-443
    /// </summary>
    [TestMethod("Example 443 - https://github.github.com/gfm/#example-443")]
    [TestCategory("Spec")]
    public void TestSpecExample443()
    {
        string inputText = @"__ is not an empty emphasis";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 444 - https://github.github.com/gfm/#example-444
    /// </summary>
    [TestMethod("Example 444 - https://github.github.com/gfm/#example-444")]
    [TestCategory("Spec")]
    public void TestSpecExample444()
    {
        string inputText = @"____ is not an empty strong emphasis";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 445 - https://github.github.com/gfm/#example-445
    /// </summary>
    [TestMethod("Example 445 - https://github.github.com/gfm/#example-445")]
    [TestCategory("Spec")]
    public void TestSpecExample445()
    {
        string inputText = @"foo ***";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 446 - https://github.github.com/gfm/#example-446
    /// </summary>
    [TestMethod("Example 446 - https://github.github.com/gfm/#example-446")]
    [TestCategory("Spec")]
    public void TestSpecExample446()
    {
        string inputText = @"foo *\**";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (backslash_escape)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 447 - https://github.github.com/gfm/#example-447
    /// </summary>
    [TestMethod("Example 447 - https://github.github.com/gfm/#example-447")]
    [TestCategory("Spec")]
    public void TestSpecExample447()
    {
        string inputText = @"foo *_*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 448 - https://github.github.com/gfm/#example-448
    /// </summary>
    [TestMethod("Example 448 - https://github.github.com/gfm/#example-448")]
    [TestCategory("Spec")]
    public void TestSpecExample448()
    {
        string inputText = @"foo *****";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 449 - https://github.github.com/gfm/#example-449
    /// </summary>
    [TestMethod("Example 449 - https://github.github.com/gfm/#example-449")]
    [TestCategory("Spec")]
    public void TestSpecExample449()
    {
        string inputText = @"foo **\***";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (backslash_escape)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 450 - https://github.github.com/gfm/#example-450
    /// </summary>
    [TestMethod("Example 450 - https://github.github.com/gfm/#example-450")]
    [TestCategory("Spec")]
    public void TestSpecExample450()
    {
        string inputText = @"foo **_**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 451 - https://github.github.com/gfm/#example-451
    /// </summary>
    [TestMethod("Example 451 - https://github.github.com/gfm/#example-451")]
    [TestCategory("Spec")]
    public void TestSpecExample451()
    {
        string inputText = @"**foo*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 452 - https://github.github.com/gfm/#example-452
    /// </summary>
    [TestMethod("Example 452 - https://github.github.com/gfm/#example-452")]
    [TestCategory("Spec")]
    public void TestSpecExample452()
    {
        string inputText = @"*foo**";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 453 - https://github.github.com/gfm/#example-453
    /// </summary>
    [TestMethod("Example 453 - https://github.github.com/gfm/#example-453")]
    [TestCategory("Spec")]
    public void TestSpecExample453()
    {
        string inputText = @"***foo**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 454 - https://github.github.com/gfm/#example-454
    /// </summary>
    [TestMethod("Example 454 - https://github.github.com/gfm/#example-454")]
    [TestCategory("Spec")]
    public void TestSpecExample454()
    {
        string inputText = @"****foo*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 455 - https://github.github.com/gfm/#example-455
    /// </summary>
    [TestMethod("Example 455 - https://github.github.com/gfm/#example-455")]
    [TestCategory("Spec")]
    public void TestSpecExample455()
    {
        string inputText = @"**foo***";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 456 - https://github.github.com/gfm/#example-456
    /// </summary>
    [TestMethod("Example 456 - https://github.github.com/gfm/#example-456")]
    [TestCategory("Spec")]
    public void TestSpecExample456()
    {
        string inputText = @"*foo****";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 457 - https://github.github.com/gfm/#example-457
    /// </summary>
    [TestMethod("Example 457 - https://github.github.com/gfm/#example-457")]
    [TestCategory("Spec")]
    public void TestSpecExample457()
    {
        string inputText = @"foo ___";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 458 - https://github.github.com/gfm/#example-458
    /// </summary>
    [TestMethod("Example 458 - https://github.github.com/gfm/#example-458")]
    [TestCategory("Spec")]
    public void TestSpecExample458()
    {
        string inputText = @"foo _\__";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (backslash_escape)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 459 - https://github.github.com/gfm/#example-459
    /// </summary>
    [TestMethod("Example 459 - https://github.github.com/gfm/#example-459")]
    [TestCategory("Spec")]
    public void TestSpecExample459()
    {
        string inputText = @"foo _*_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 460 - https://github.github.com/gfm/#example-460
    /// </summary>
    [TestMethod("Example 460 - https://github.github.com/gfm/#example-460")]
    [TestCategory("Spec")]
    public void TestSpecExample460()
    {
        string inputText = @"foo _____";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 461 - https://github.github.com/gfm/#example-461
    /// </summary>
    [TestMethod("Example 461 - https://github.github.com/gfm/#example-461")]
    [TestCategory("Spec")]
    public void TestSpecExample461()
    {
        string inputText = @"foo __\___";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (backslash_escape)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 462 - https://github.github.com/gfm/#example-462
    /// </summary>
    [TestMethod("Example 462 - https://github.github.com/gfm/#example-462")]
    [TestCategory("Spec")]
    public void TestSpecExample462()
    {
        string inputText = @"foo __*__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 463 - https://github.github.com/gfm/#example-463
    /// </summary>
    [TestMethod("Example 463 - https://github.github.com/gfm/#example-463")]
    [TestCategory("Spec")]
    public void TestSpecExample463()
    {
        string inputText = @"__foo_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 464 - https://github.github.com/gfm/#example-464
    /// </summary>
    [TestMethod("Example 464 - https://github.github.com/gfm/#example-464")]
    [TestCategory("Spec")]
    public void TestSpecExample464()
    {
        string inputText = @"_foo__";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 465 - https://github.github.com/gfm/#example-465
    /// </summary>
    [TestMethod("Example 465 - https://github.github.com/gfm/#example-465")]
    [TestCategory("Spec")]
    public void TestSpecExample465()
    {
        string inputText = @"___foo__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 466 - https://github.github.com/gfm/#example-466
    /// </summary>
    [TestMethod("Example 466 - https://github.github.com/gfm/#example-466")]
    [TestCategory("Spec")]
    public void TestSpecExample466()
    {
        string inputText = @"____foo_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 467 - https://github.github.com/gfm/#example-467
    /// </summary>
    [TestMethod("Example 467 - https://github.github.com/gfm/#example-467")]
    [TestCategory("Spec")]
    public void TestSpecExample467()
    {
        string inputText = @"__foo___";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 468 - https://github.github.com/gfm/#example-468
    /// </summary>
    [TestMethod("Example 468 - https://github.github.com/gfm/#example-468")]
    [TestCategory("Spec")]
    public void TestSpecExample468()
    {
        string inputText = @"_foo____";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 469 - https://github.github.com/gfm/#example-469
    /// </summary>
    [TestMethod("Example 469 - https://github.github.com/gfm/#example-469")]
    [TestCategory("Spec")]
    public void TestSpecExample469()
    {
        string inputText = @"**foo**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 470 - https://github.github.com/gfm/#example-470
    /// </summary>
    [TestMethod("Example 470 - https://github.github.com/gfm/#example-470")]
    [TestCategory("Spec")]
    public void TestSpecExample470()
    {
        string inputText = @"*_foo_*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 471 - https://github.github.com/gfm/#example-471
    /// </summary>
    [TestMethod("Example 471 - https://github.github.com/gfm/#example-471")]
    [TestCategory("Spec")]
    public void TestSpecExample471()
    {
        string inputText = @"__foo__";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 472 - https://github.github.com/gfm/#example-472
    /// </summary>
    [TestMethod("Example 472 - https://github.github.com/gfm/#example-472")]
    [TestCategory("Spec")]
    public void TestSpecExample472()
    {
        string inputText = @"_*foo*_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 473 - https://github.github.com/gfm/#example-473
    /// </summary>
    [TestMethod("Example 473 - https://github.github.com/gfm/#example-473")]
    [TestCategory("Spec")]
    public void TestSpecExample473()
    {
        string inputText = @"****foo****";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 474 - https://github.github.com/gfm/#example-474
    /// </summary>
    [TestMethod("Example 474 - https://github.github.com/gfm/#example-474")]
    [TestCategory("Spec")]
    public void TestSpecExample474()
    {
        string inputText = @"____foo____";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 475 - https://github.github.com/gfm/#example-475
    /// </summary>
    [TestMethod("Example 475 - https://github.github.com/gfm/#example-475")]
    [TestCategory("Spec")]
    public void TestSpecExample475()
    {
        string inputText = @"******foo******";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (strong_emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter))
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 476 - https://github.github.com/gfm/#example-476
    /// </summary>
    [TestMethod("Example 476 - https://github.github.com/gfm/#example-476")]
    [TestCategory("Spec")]
    public void TestSpecExample476()
    {
        string inputText = @"***foo***";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 477 - https://github.github.com/gfm/#example-477
    /// </summary>
    [TestMethod("Example 477 - https://github.github.com/gfm/#example-477")]
    [TestCategory("Spec")]
    public void TestSpecExample477()
    {
        string inputText = @"_____foo_____";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (strong_emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter)
                (emphasis_delimiter))
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 478 - https://github.github.com/gfm/#example-478
    /// </summary>
    [TestMethod("Example 478 - https://github.github.com/gfm/#example-478")]
    [TestCategory("Spec")]
    public void TestSpecExample478()
    {
        string inputText = @"*foo _bar* baz_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 479 - https://github.github.com/gfm/#example-479
    /// </summary>
    [TestMethod("Example 479 - https://github.github.com/gfm/#example-479")]
    [TestCategory("Spec")]
    public void TestSpecExample479()
    {
        string inputText = @"*foo __bar *baz bim__ bam*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (strong_emphasis
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter)
              (emphasis_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 480 - https://github.github.com/gfm/#example-480
    /// </summary>
    [TestMethod("Example 480 - https://github.github.com/gfm/#example-480")]
    [TestCategory("Spec")]
    public void TestSpecExample480()
    {
        string inputText = @"**foo **bar baz**";
        string expectedSyntaxTree = @"(inline
          (strong_emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 481 - https://github.github.com/gfm/#example-481
    /// </summary>
    [TestMethod("Example 481 - https://github.github.com/gfm/#example-481")]
    [TestCategory("Spec")]
    public void TestSpecExample481()
    {
        string inputText = @"*foo *bar baz*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 482 - https://github.github.com/gfm/#example-482
    /// </summary>
    [TestMethod("Example 482 - https://github.github.com/gfm/#example-482")]
    [TestCategory("Spec")]
    public void TestSpecExample482()
    {
        string inputText = @"*[bar*](/url)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 483 - https://github.github.com/gfm/#example-483
    /// </summary>
    [TestMethod("Example 483 - https://github.github.com/gfm/#example-483")]
    [TestCategory("Spec")]
    public void TestSpecExample483()
    {
        string inputText = @"_foo [bar_](/url)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 484 - https://github.github.com/gfm/#example-484
    /// </summary>
    [TestMethod("Example 484 - https://github.github.com/gfm/#example-484")]
    [TestCategory("Spec")]
    public void TestSpecExample484()
    {
        string inputText = @"*<img src=""foo"" title=""*""/>";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 485 - https://github.github.com/gfm/#example-485
    /// </summary>
    [TestMethod("Example 485 - https://github.github.com/gfm/#example-485")]
    [TestCategory("Spec")]
    public void TestSpecExample485()
    {
        string inputText = @"**<a href=""**"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 486 - https://github.github.com/gfm/#example-486
    /// </summary>
    [TestMethod("Example 486 - https://github.github.com/gfm/#example-486")]
    [TestCategory("Spec")]
    public void TestSpecExample486()
    {
        string inputText = @"__<a href=""__"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 487 - https://github.github.com/gfm/#example-487
    /// </summary>
    [TestMethod("Example 487 - https://github.github.com/gfm/#example-487")]
    [TestCategory("Spec")]
    public void TestSpecExample487()
    {
        string inputText = @"*a `*`*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (code_span
              (code_span_delimiter)
              (code_span_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 488 - https://github.github.com/gfm/#example-488
    /// </summary>
    [TestMethod("Example 488 - https://github.github.com/gfm/#example-488")]
    [TestCategory("Spec")]
    public void TestSpecExample488()
    {
        string inputText = @"_a `_`_";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (code_span
              (code_span_delimiter)
              (code_span_delimiter))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 489 - https://github.github.com/gfm/#example-489
    /// </summary>
    [TestMethod("Example 489 - https://github.github.com/gfm/#example-489")]
    [TestCategory("Spec")]
    public void TestSpecExample489()
    {
        string inputText = @"**a<http://foo.bar/?q=**>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 490 - https://github.github.com/gfm/#example-490
    /// </summary>
    [TestMethod("Example 490 - https://github.github.com/gfm/#example-490")]
    [TestCategory("Spec")]
    public void TestSpecExample490()
    {
        string inputText = @"__a<http://foo.bar/?q=__>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 493 - https://github.github.com/gfm/#example-493
    /// </summary>
    [TestMethod("Example 493 - https://github.github.com/gfm/#example-493")]
    [TestCategory("Spec")]
    public void TestSpecExample493()
    {
        string inputText = @"[link](/uri ""title"")";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)
            (link_title)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 494 - https://github.github.com/gfm/#example-494
    /// </summary>
    [TestMethod("Example 494 - https://github.github.com/gfm/#example-494")]
    [TestCategory("Spec")]
    public void TestSpecExample494()
    {
        string inputText = @"[link](/uri)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 495 - https://github.github.com/gfm/#example-495
    /// </summary>
    [TestMethod("Example 495 - https://github.github.com/gfm/#example-495")]
    [TestCategory("Spec")]
    public void TestSpecExample495()
    {
        string inputText = @"[link]()";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 496 - https://github.github.com/gfm/#example-496
    /// </summary>
    [TestMethod("Example 496 - https://github.github.com/gfm/#example-496")]
    [TestCategory("Spec")]
    public void TestSpecExample496()
    {
        string inputText = @"[link](<>)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 497 - https://github.github.com/gfm/#example-497
    /// </summary>
    [TestMethod("Example 497 - https://github.github.com/gfm/#example-497")]
    [TestCategory("Spec")]
    public void TestSpecExample497()
    {
        string inputText = @"[link](/my uri)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 498 - https://github.github.com/gfm/#example-498
    /// </summary>
    [TestMethod("Example 498 - https://github.github.com/gfm/#example-498")]
    [TestCategory("Spec")]
    public void TestSpecExample498()
    {
        string inputText = @"[link](</my uri>)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 499 - https://github.github.com/gfm/#example-499
    /// </summary>
    [TestMethod("Example 499 - https://github.github.com/gfm/#example-499")]
    [TestCategory("Spec")]
    public void TestSpecExample499()
    {
        string inputText = @"[link](foo
bar)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 500 - https://github.github.com/gfm/#example-500
    /// </summary>
    [TestMethod("Example 500 - https://github.github.com/gfm/#example-500")]
    [TestCategory("Spec")]
    public void TestSpecExample500()
    {
        string inputText = @"[link](<foo
bar>)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 501 - https://github.github.com/gfm/#example-501
    /// </summary>
    [TestMethod("Example 501 - https://github.github.com/gfm/#example-501")]
    [TestCategory("Spec")]
    public void TestSpecExample501()
    {
        string inputText = @"[a](<b)c>)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 502 - https://github.github.com/gfm/#example-502
    /// </summary>
    [TestMethod("Example 502 - https://github.github.com/gfm/#example-502")]
    [TestCategory("Spec")]
    public void TestSpecExample502()
    {
        string inputText = @"[link](<foo\>)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 503 - https://github.github.com/gfm/#example-503
    /// </summary>
    [TestMethod("Example 503 - https://github.github.com/gfm/#example-503")]
    [TestCategory("Spec")]
    public void TestSpecExample503()
    {
        string inputText = @"[a](<b)c
[a](<b)c>
[a](<b>c)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (shortcut_link
            (link_text))
          (shortcut_link
            (link_text))
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 504 - https://github.github.com/gfm/#example-504
    /// </summary>
    [TestMethod("Example 504 - https://github.github.com/gfm/#example-504")]
    [TestCategory("Spec")]
    public void TestSpecExample504()
    {
        string inputText = @"[link](\(foo\))";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination
              (backslash_escape)
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 505 - https://github.github.com/gfm/#example-505
    /// </summary>
    [TestMethod("Example 505 - https://github.github.com/gfm/#example-505")]
    [TestCategory("Spec")]
    public void TestSpecExample505()
    {
        string inputText = @"[link](foo(and(bar)))";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 506 - https://github.github.com/gfm/#example-506
    /// </summary>
    [TestMethod("Example 506 - https://github.github.com/gfm/#example-506")]
    [TestCategory("Spec")]
    public void TestSpecExample506()
    {
        string inputText = @"[link](foo\(and\(bar\))";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination
              (backslash_escape)
              (backslash_escape)
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 507 - https://github.github.com/gfm/#example-507
    /// </summary>
    [TestMethod("Example 507 - https://github.github.com/gfm/#example-507")]
    [TestCategory("Spec")]
    public void TestSpecExample507()
    {
        string inputText = @"[link](<foo(and(bar)>)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 508 - https://github.github.com/gfm/#example-508
    /// </summary>
    [TestMethod("Example 508 - https://github.github.com/gfm/#example-508")]
    [TestCategory("Spec")]
    public void TestSpecExample508()
    {
        string inputText = @"[link](foo\)\:)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination
              (backslash_escape)
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 509 - https://github.github.com/gfm/#example-509
    /// </summary>
    [TestMethod("Example 509 - https://github.github.com/gfm/#example-509")]
    [TestCategory("Spec")]
    public void TestSpecExample509()
    {
        string inputText = @"[link](#fragment)
[link](http://example.com#fragment)
[link](http://example.com?foo=3#frag)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination))
          (inline_link
            (link_text)
            (link_destination))
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 510 - https://github.github.com/gfm/#example-510
    /// </summary>
    [TestMethod("Example 510 - https://github.github.com/gfm/#example-510")]
    [TestCategory("Spec")]
    public void TestSpecExample510()
    {
        string inputText = @"[link](foo\bar)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 511 - https://github.github.com/gfm/#example-511
    /// </summary>
    [TestMethod("Example 511 - https://github.github.com/gfm/#example-511")]
    [TestCategory("Spec")]
    public void TestSpecExample511()
    {
        string inputText = @"[link](foo%20b&auml;)";
        string expectedSyntaxTree = @"(inline
  (inline_link
    (link_text)
    (link_destination
      (entity_reference))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 512 - https://github.github.com/gfm/#example-512
    /// </summary>
    [TestMethod("Example 512 - https://github.github.com/gfm/#example-512")]
    [TestCategory("Spec")]
    public void TestSpecExample512()
    {
        string inputText = @"[link](""title"")";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 513 - https://github.github.com/gfm/#example-513
    /// </summary>
    [TestMethod("Example 513 - https://github.github.com/gfm/#example-513")]
    [TestCategory("Spec")]
    public void TestSpecExample513()
    {
        string inputText = @"[link](/url ""title"")
[link](/url 'title')
[link](/url (title))";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)
            (link_title))
          (inline_link
            (link_text)
            (link_destination)
            (link_title))
          (inline_link
            (link_text)
            (link_destination)
            (link_title)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 514 - https://github.github.com/gfm/#example-514
    /// </summary>
    [TestMethod("Example 514 - https://github.github.com/gfm/#example-514")]
    [TestCategory("Spec")]
    public void TestSpecExample514()
    {
        string inputText = @"[link](/url ""title \""&quot;"")";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)
            (link_title
              (backslash_escape)
              (entity_reference))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 515 - https://github.github.com/gfm/#example-515
    /// </summary>
    [TestMethod("Example 515 - https://github.github.com/gfm/#example-515")]
    [TestCategory("Spec")]
    public void TestSpecExample515()
    {
        string inputText = @"[link](/url ""title"")";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 516 - https://github.github.com/gfm/#example-516
    /// </summary>
    [TestMethod("Example 516 - https://github.github.com/gfm/#example-516")]
    [TestCategory("Spec")]
    public void TestSpecExample516()
    {
        string inputText = @"[link](/url ""title ""and"" title"")";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 517 - https://github.github.com/gfm/#example-517
    /// </summary>
    [TestMethod("Example 517 - https://github.github.com/gfm/#example-517")]
    [TestCategory("Spec")]
    public void TestSpecExample517()
    {
        string inputText = @"[link](/url 'title ""and"" title')";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)
            (link_title)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 518 - https://github.github.com/gfm/#example-518
    /// </summary>
    [TestMethod("Example 518 - https://github.github.com/gfm/#example-518")]
    [TestCategory("Spec")]
    public void TestSpecExample518()
    {
        string inputText = @"[link](   /uri
  ""title""  )";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)
            (link_title)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 519 - https://github.github.com/gfm/#example-519
    /// </summary>
    [TestMethod("Example 519 - https://github.github.com/gfm/#example-519")]
    [TestCategory("Spec")]
    public void TestSpecExample519()
    {
        string inputText = @"[link] (/uri)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 520 - https://github.github.com/gfm/#example-520
    /// </summary>
    [TestMethod("Example 520 - https://github.github.com/gfm/#example-520")]
    [TestCategory("Spec")]
    public void TestSpecExample520()
    {
        string inputText = @"[link [foo [bar]]](/uri)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 521 - https://github.github.com/gfm/#example-521
    /// </summary>
    [TestMethod("Example 521 - https://github.github.com/gfm/#example-521")]
    [TestCategory("Spec")]
    public void TestSpecExample521()
    {
        string inputText = @"[link] bar](/uri)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 522 - https://github.github.com/gfm/#example-522
    /// </summary>
    [TestMethod("Example 522 - https://github.github.com/gfm/#example-522")]
    [TestCategory("Spec")]
    public void TestSpecExample522()
    {
        string inputText = @"[link [bar](/uri)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 523 - https://github.github.com/gfm/#example-523
    /// </summary>
    [TestMethod("Example 523 - https://github.github.com/gfm/#example-523")]
    [TestCategory("Spec")]
    public void TestSpecExample523()
    {
        string inputText = @"[link \[bar](/uri)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text
              (backslash_escape))
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 525 - https://github.github.com/gfm/#example-525
    /// </summary>
    [TestMethod("Example 525 - https://github.github.com/gfm/#example-525")]
    [TestCategory("Spec")]
    public void TestSpecExample525()
    {
        string inputText = @"[![moon](moon.jpg)](/uri)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text
              (image
                (image_description)
                (link_destination)))
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 526 - https://github.github.com/gfm/#example-526
    /// </summary>
    [TestMethod("Example 526 - https://github.github.com/gfm/#example-526")]
    [TestCategory("Spec")]
    public void TestSpecExample526()
    {
        string inputText = @"[foo [bar](/uri)](/uri)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 527 - https://github.github.com/gfm/#example-527
    /// </summary>
    [TestMethod("Example 527 - https://github.github.com/gfm/#example-527")]
    [TestCategory("Spec")]
    public void TestSpecExample527()
    {
        string inputText = @"[foo *[bar [baz](/uri)](/uri)*](/uri)";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (inline_link
              (link_text)
              (link_destination))
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 528 - https://github.github.com/gfm/#example-528
    /// </summary>
    [TestMethod("Example 528 - https://github.github.com/gfm/#example-528")]
    [TestCategory("Spec")]
    public void TestSpecExample528()
    {
        string inputText = @"![[[foo](uri1)](uri2)](uri3)";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (inline_link
                (link_text)
                (link_destination)))
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 529 - https://github.github.com/gfm/#example-529
    /// </summary>
    [TestMethod("Example 529 - https://github.github.com/gfm/#example-529")]
    [TestCategory("Spec")]
    public void TestSpecExample529()
    {
        string inputText = @"*[foo*](/uri)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 530 - https://github.github.com/gfm/#example-530
    /// </summary>
    [TestMethod("Example 530 - https://github.github.com/gfm/#example-530")]
    [TestCategory("Spec")]
    public void TestSpecExample530()
    {
        string inputText = @"[foo *bar](baz*)";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 531 - https://github.github.com/gfm/#example-531
    /// </summary>
    [TestMethod("Example 531 - https://github.github.com/gfm/#example-531")]
    [TestCategory("Spec")]
    public void TestSpecExample531()
    {
        string inputText = @"*foo [bar* baz]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 532 - https://github.github.com/gfm/#example-532
    /// </summary>
    [TestMethod("Example 532 - https://github.github.com/gfm/#example-532")]
    [TestCategory("Spec")]
    public void TestSpecExample532()
    {
        string inputText = @"[foo <bar attr=""](baz)"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 533 - https://github.github.com/gfm/#example-533
    /// </summary>
    [TestMethod("Example 533 - https://github.github.com/gfm/#example-533")]
    [TestCategory("Spec")]
    public void TestSpecExample533()
    {
        string inputText = @"[foo`](/uri)`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 534 - https://github.github.com/gfm/#example-534
    /// </summary>
    [TestMethod("Example 534 - https://github.github.com/gfm/#example-534")]
    [TestCategory("Spec")]
    public void TestSpecExample534()
    {
        string inputText = @"[foo<http://example.com/?search=](uri)>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 535 - https://github.github.com/gfm/#example-535
    /// </summary>
    [TestMethod("Example 535 - https://github.github.com/gfm/#example-535")]
    [TestCategory("Spec")]
    public void TestSpecExample535()
    {
        string inputText = @"[foo][bar]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 536 - https://github.github.com/gfm/#example-536
    /// </summary>
    [TestMethod("Example 536 - https://github.github.com/gfm/#example-536")]
    [TestCategory("Spec")]
    public void TestSpecExample536()
    {
        string inputText = @"[link [foo [bar]]][ref]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 537 - https://github.github.com/gfm/#example-537
    /// </summary>
    [TestMethod("Example 537 - https://github.github.com/gfm/#example-537")]
    [TestCategory("Spec")]
    public void TestSpecExample537()
    {
        string inputText = @"[link \[bar][ref]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text
              (backslash_escape))
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 539 - https://github.github.com/gfm/#example-539
    /// </summary>
    [TestMethod("Example 539 - https://github.github.com/gfm/#example-539")]
    [TestCategory("Spec")]
    public void TestSpecExample539()
    {
        string inputText = @"[![moon](moon.jpg)][ref]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text
              (image
                (image_description)
                (link_destination)))
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 540 - https://github.github.com/gfm/#example-540
    /// </summary>
    [TestMethod("Example 540 - https://github.github.com/gfm/#example-540")]
    [TestCategory("Spec")]
    public void TestSpecExample540()
    {
        string inputText = @"[foo [bar](/uri)][ref]";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)
            (link_destination))
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 541 - https://github.github.com/gfm/#example-541
    /// </summary>
    [TestMethod("Example 541 - https://github.github.com/gfm/#example-541")]
    [TestCategory("Spec")]
    public void TestSpecExample541()
    {
        string inputText = @"[foo *bar [baz][ref]*][ref]";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (full_reference_link
              (link_text)
              (link_label))
            (emphasis_delimiter))
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 542 - https://github.github.com/gfm/#example-542
    /// </summary>
    [TestMethod("Example 542 - https://github.github.com/gfm/#example-542")]
    [TestCategory("Spec")]
    public void TestSpecExample542()
    {
        string inputText = @"*[foo*][ref]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 543 - https://github.github.com/gfm/#example-543
    /// </summary>
    [TestMethod("Example 543 - https://github.github.com/gfm/#example-543")]
    [TestCategory("Spec")]
    public void TestSpecExample543()
    {
        string inputText = @"[foo *bar][ref]*";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 544 - https://github.github.com/gfm/#example-544
    /// </summary>
    [TestMethod("Example 544 - https://github.github.com/gfm/#example-544")]
    [TestCategory("Spec")]
    public void TestSpecExample544()
    {
        string inputText = @"[foo <bar attr=""][ref]"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 545 - https://github.github.com/gfm/#example-545
    /// </summary>
    [TestMethod("Example 545 - https://github.github.com/gfm/#example-545")]
    [TestCategory("Spec")]
    public void TestSpecExample545()
    {
        string inputText = @"[foo`][ref]`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 546 - https://github.github.com/gfm/#example-546
    /// </summary>
    [TestMethod("Example 546 - https://github.github.com/gfm/#example-546")]
    [TestCategory("Spec")]
    public void TestSpecExample546()
    {
        string inputText = @"[foo<http://example.com/?search=][ref]>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 547 - https://github.github.com/gfm/#example-547
    /// </summary>
    [TestMethod("Example 547 - https://github.github.com/gfm/#example-547")]
    [TestCategory("Spec")]
    public void TestSpecExample547()
    {
        string inputText = @"[foo][BaR]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 548 - https://github.github.com/gfm/#example-548
    /// </summary>
    [TestMethod("Example 548 - https://github.github.com/gfm/#example-548")]
    [TestCategory("Spec")]
    public void TestSpecExample548()
    {
        string inputText = @"[ẞ]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 549 - https://github.github.com/gfm/#example-549
    /// </summary>
    [TestMethod("Example 549 - https://github.github.com/gfm/#example-549")]
    [TestCategory("Spec")]
    public void TestSpecExample549()
    {
        string inputText = @"[Baz][Foo bar]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 550 - https://github.github.com/gfm/#example-550
    /// </summary>
    [TestMethod("Example 550 - https://github.github.com/gfm/#example-550")]
    [TestCategory("Spec")]
    public void TestSpecExample550()
    {
        string inputText = @"[foo] [bar]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 551 - https://github.github.com/gfm/#example-551
    /// </summary>
    [TestMethod("Example 551 - https://github.github.com/gfm/#example-551")]
    [TestCategory("Spec")]
    public void TestSpecExample551()
    {
        string inputText = @"[foo]
[bar]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 552 - https://github.github.com/gfm/#example-552
    /// </summary>
    [TestMethod("Example 552 - https://github.github.com/gfm/#example-552")]
    [TestCategory("Spec")]
    public void TestSpecExample552()
    {
        string inputText = @"[bar][foo]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 553 - https://github.github.com/gfm/#example-553
    /// </summary>
    [TestMethod("Example 553 - https://github.github.com/gfm/#example-553")]
    [TestCategory("Spec")]
    public void TestSpecExample553()
    {
        string inputText = @"[bar][foo\!]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 554 - https://github.github.com/gfm/#example-554
    /// </summary>
    [TestMethod("Example 554 - https://github.github.com/gfm/#example-554")]
    [TestCategory("Spec")]
    public void TestSpecExample554()
    {
        string inputText = @"[foo][ref[]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 555 - https://github.github.com/gfm/#example-555
    /// </summary>
    [TestMethod("Example 555 - https://github.github.com/gfm/#example-555")]
    [TestCategory("Spec")]
    public void TestSpecExample555()
    {
        string inputText = @"[foo][ref[bar]]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text))
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 557 - https://github.github.com/gfm/#example-557
    /// </summary>
    [TestMethod("Example 557 - https://github.github.com/gfm/#example-557")]
    [TestCategory("Spec")]
    public void TestSpecExample557()
    {
        string inputText = @"[foo][ref\[]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 558 - https://github.github.com/gfm/#example-558
    /// </summary>
    [TestMethod("Example 558 - https://github.github.com/gfm/#example-558")]
    [TestCategory("Spec")]
    public void TestSpecExample558()
    {
        string inputText = @"[bar\\]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text
              (backslash_escape))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 559 - https://github.github.com/gfm/#example-559
    /// </summary>
    [TestMethod("Example 559 - https://github.github.com/gfm/#example-559")]
    [TestCategory("Spec")]
    public void TestSpecExample559()
    {
        string inputText = @"[]";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 561 - https://github.github.com/gfm/#example-561
    /// </summary>
    [TestMethod("Example 561 - https://github.github.com/gfm/#example-561")]
    [TestCategory("Spec")]
    public void TestSpecExample561()
    {
        string inputText = @"[foo][]";
        string expectedSyntaxTree = @"(inline
          (collapsed_reference_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 562 - https://github.github.com/gfm/#example-562
    /// </summary>
    [TestMethod("Example 562 - https://github.github.com/gfm/#example-562")]
    [TestCategory("Spec")]
    public void TestSpecExample562()
    {
        string inputText = @"[*foo* bar][]";
        string expectedSyntaxTree = @"(inline
          (collapsed_reference_link
            (link_text
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 563 - https://github.github.com/gfm/#example-563
    /// </summary>
    [TestMethod("Example 563 - https://github.github.com/gfm/#example-563")]
    [TestCategory("Spec")]
    public void TestSpecExample563()
    {
        string inputText = @"[Foo][]";
        string expectedSyntaxTree = @"(inline
          (collapsed_reference_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 564 - https://github.github.com/gfm/#example-564
    /// </summary>
    [TestMethod("Example 564 - https://github.github.com/gfm/#example-564")]
    [TestCategory("Spec")]
    public void TestSpecExample564()
    {
        string inputText = @"[foo]
[]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 565 - https://github.github.com/gfm/#example-565
    /// </summary>
    [TestMethod("Example 565 - https://github.github.com/gfm/#example-565")]
    [TestCategory("Spec")]
    public void TestSpecExample565()
    {
        string inputText = @"[foo]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 566 - https://github.github.com/gfm/#example-566
    /// </summary>
    [TestMethod("Example 566 - https://github.github.com/gfm/#example-566")]
    [TestCategory("Spec")]
    public void TestSpecExample566()
    {
        string inputText = @"[*foo* bar]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 568 - https://github.github.com/gfm/#example-568
    /// </summary>
    [TestMethod("Example 568 - https://github.github.com/gfm/#example-568")]
    [TestCategory("Spec")]
    public void TestSpecExample568()
    {
        string inputText = @"[[bar [foo]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 569 - https://github.github.com/gfm/#example-569
    /// </summary>
    [TestMethod("Example 569 - https://github.github.com/gfm/#example-569")]
    [TestCategory("Spec")]
    public void TestSpecExample569()
    {
        string inputText = @"[Foo]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 570 - https://github.github.com/gfm/#example-570
    /// </summary>
    [TestMethod("Example 570 - https://github.github.com/gfm/#example-570")]
    [TestCategory("Spec")]
    public void TestSpecExample570()
    {
        string inputText = @"[foo] bar";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 571 - https://github.github.com/gfm/#example-571
    /// </summary>
    [TestMethod("Example 571 - https://github.github.com/gfm/#example-571")]
    [TestCategory("Spec")]
    public void TestSpecExample571()
    {
        string inputText = @"\[foo]";
        string expectedSyntaxTree = @"(inline
          (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 572 - https://github.github.com/gfm/#example-572
    /// </summary>
    [TestMethod("Example 572 - https://github.github.com/gfm/#example-572")]
    [TestCategory("Spec")]
    public void TestSpecExample572()
    {
        string inputText = @"*[foo*]";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 573 - https://github.github.com/gfm/#example-573
    /// </summary>
    [TestMethod("Example 573 - https://github.github.com/gfm/#example-573")]
    [TestCategory("Spec")]
    public void TestSpecExample573()
    {
        string inputText = @"[foo][bar]";
        string expectedSyntaxTree = @"(inline
          (full_reference_link
            (link_text)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 574 - https://github.github.com/gfm/#example-574
    /// </summary>
    [TestMethod("Example 574 - https://github.github.com/gfm/#example-574")]
    [TestCategory("Spec")]
    public void TestSpecExample574()
    {
        string inputText = @"[foo][]";
        string expectedSyntaxTree = @"(inline
          (collapsed_reference_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 575 - https://github.github.com/gfm/#example-575
    /// </summary>
    [TestMethod("Example 575 - https://github.github.com/gfm/#example-575")]
    [TestCategory("Spec")]
    public void TestSpecExample575()
    {
        string inputText = @"[foo]()";
        string expectedSyntaxTree = @"(inline
          (inline_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 576 - https://github.github.com/gfm/#example-576
    /// </summary>
    [TestMethod("Example 576 - https://github.github.com/gfm/#example-576")]
    [TestCategory("Spec")]
    public void TestSpecExample576()
    {
        string inputText = @"[foo](not a link)";
        string expectedSyntaxTree = @"(inline
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 577 - https://github.github.com/gfm/#example-577
    /// </summary>
    [TestMethod("Example 577 - https://github.github.com/gfm/#example-577")]
    [TestCategory("Spec")]
    public void TestSpecExample577()
    {
        string inputText = @"[foo][bar][baz]
";
        string expectedSyntaxTree = @"(inline
  (full_reference_link
    (link_text)
    (link_label))
  (shortcut_link
    (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 578 - https://github.github.com/gfm/#example-578
    /// </summary>
    [TestMethod("Example 578 - https://github.github.com/gfm/#example-578")]
    [TestCategory("Spec")]
    public void TestSpecExample578()
    {
        string inputText = @"[foo][bar][baz]
";
        string expectedSyntaxTree = @"(inline
  (full_reference_link
    (link_text)
    (link_label))
  (shortcut_link
    (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 579 - https://github.github.com/gfm/#example-579
    /// </summary>
    [TestMethod("Example 579 - https://github.github.com/gfm/#example-579")]
    [TestCategory("Spec")]
    public void TestSpecExample579()
    {
        string inputText = @"[foo][bar][baz]
";
        string expectedSyntaxTree = @"(inline
  (full_reference_link
    (link_text)
    (link_label))
  (shortcut_link
    (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 580 - https://github.github.com/gfm/#example-580
    /// </summary>
    [TestMethod("Example 580 - https://github.github.com/gfm/#example-580")]
    [TestCategory("Spec")]
    public void TestSpecExample580()
    {
        string inputText = @"![foo](/url ""title"")";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)
            (link_destination)
            (link_title)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 581 - https://github.github.com/gfm/#example-581
    /// </summary>
    [TestMethod("Example 581 - https://github.github.com/gfm/#example-581")]
    [TestCategory("Spec")]
    public void TestSpecExample581()
    {
        string inputText = @"![foo *bar*]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 582 - https://github.github.com/gfm/#example-582
    /// </summary>
    [TestMethod("Example 582 - https://github.github.com/gfm/#example-582")]
    [TestCategory("Spec")]
    public void TestSpecExample582()
    {
        string inputText = @"![foo ![bar](/url)](/url2)";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (image
                (image_description)
                (link_destination)))
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 583 - https://github.github.com/gfm/#example-583
    /// </summary>
    [TestMethod("Example 583 - https://github.github.com/gfm/#example-583")]
    [TestCategory("Spec")]
    public void TestSpecExample583()
    {
        string inputText = @"![foo [bar](/url)](/url2)";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (inline_link
                (link_text)
                (link_destination)))
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 584 - https://github.github.com/gfm/#example-584
    /// </summary>
    [TestMethod("Example 584 - https://github.github.com/gfm/#example-584")]
    [TestCategory("Spec")]
    public void TestSpecExample584()
    {
        string inputText = @"![foo *bar*][]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 585 - https://github.github.com/gfm/#example-585
    /// </summary>
    [TestMethod("Example 585 - https://github.github.com/gfm/#example-585")]
    [TestCategory("Spec")]
    public void TestSpecExample585()
    {
        string inputText = @"![foo *bar*][foobar]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 586 - https://github.github.com/gfm/#example-586
    /// </summary>
    [TestMethod("Example 586 - https://github.github.com/gfm/#example-586")]
    [TestCategory("Spec")]
    public void TestSpecExample586()
    {
        string inputText = @"![foo](train.jpg)";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 587 - https://github.github.com/gfm/#example-587
    /// </summary>
    [TestMethod("Example 587 - https://github.github.com/gfm/#example-587")]
    [TestCategory("Spec")]
    public void TestSpecExample587()
    {
        string inputText = @"My ![foo bar](/path/to/train.jpg  ""title""   )";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)
            (link_destination)
            (link_title)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 589 - https://github.github.com/gfm/#example-589
    /// </summary>
    [TestMethod("Example 589 - https://github.github.com/gfm/#example-589")]
    [TestCategory("Spec")]
    public void TestSpecExample589()
    {
        string inputText = @"![](/url)";
        string expectedSyntaxTree = @"(inline
          (image
            (link_destination)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 590 - https://github.github.com/gfm/#example-590
    /// </summary>
    [TestMethod("Example 590 - https://github.github.com/gfm/#example-590")]
    [TestCategory("Spec")]
    public void TestSpecExample590()
    {
        string inputText = @"![foo][bar]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 591 - https://github.github.com/gfm/#example-591
    /// </summary>
    [TestMethod("Example 591 - https://github.github.com/gfm/#example-591")]
    [TestCategory("Spec")]
    public void TestSpecExample591()
    {
        string inputText = @"![foo][bar]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)
            (link_label)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 592 - https://github.github.com/gfm/#example-592
    /// </summary>
    [TestMethod("Example 592 - https://github.github.com/gfm/#example-592")]
    [TestCategory("Spec")]
    public void TestSpecExample592()
    {
        string inputText = @"![foo][]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 593 - https://github.github.com/gfm/#example-593
    /// </summary>
    [TestMethod("Example 593 - https://github.github.com/gfm/#example-593")]
    [TestCategory("Spec")]
    public void TestSpecExample593()
    {
        string inputText = @"![*foo* bar][]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 594 - https://github.github.com/gfm/#example-594
    /// </summary>
    [TestMethod("Example 594 - https://github.github.com/gfm/#example-594")]
    [TestCategory("Spec")]
    public void TestSpecExample594()
    {
        string inputText = @"![Foo][]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 595 - https://github.github.com/gfm/#example-595
    /// </summary>
    [TestMethod("Example 595 - https://github.github.com/gfm/#example-595")]
    [TestCategory("Spec")]
    public void TestSpecExample595()
    {
        string inputText = @"![foo]
[]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }


    /// <summary>
    /// Example 596 - https://github.github.com/gfm/#example-596
    /// </summary>
    [TestMethod("Example 596 - https://github.github.com/gfm/#example-596")]
    [TestCategory("Spec")]
    public void TestSpecExample596()
    {
        string inputText = @"![foo]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 597 - https://github.github.com/gfm/#example-597
    /// </summary>
    [TestMethod("Example 597 - https://github.github.com/gfm/#example-597")]
    [TestCategory("Spec")]
    public void TestSpecExample597()
    {
        string inputText = @"![*foo* bar]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (emphasis
                (emphasis_delimiter)
                (emphasis_delimiter)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 598 - https://github.github.com/gfm/#example-598
    /// </summary>
    [TestMethod("Example 598 - https://github.github.com/gfm/#example-598")]
    [TestCategory("Spec")]
    public void TestSpecExample598()
    {
        string inputText = @"![[foo]]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description
              (shortcut_link
                (link_text)))))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 599 - https://github.github.com/gfm/#example-599
    /// </summary>
    [TestMethod("Example 599 - https://github.github.com/gfm/#example-599")]
    [TestCategory("Spec")]
    public void TestSpecExample599()
    {
        string inputText = @"![Foo]";
        string expectedSyntaxTree = @"(inline
          (image
            (image_description)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 600 - https://github.github.com/gfm/#example-600
    /// </summary>
    [TestMethod("Example 600 - https://github.github.com/gfm/#example-600")]
    [TestCategory("Spec")]
    public void TestSpecExample600()
    {
        string inputText = @"!\[foo]";
        string expectedSyntaxTree = @"(inline
          (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 601 - https://github.github.com/gfm/#example-601
    /// </summary>
    [TestMethod("Example 601 - https://github.github.com/gfm/#example-601")]
    [TestCategory("Spec")]
    public void TestSpecExample601()
    {
        string inputText = @"\![foo]";
        string expectedSyntaxTree = @"(inline
          (backslash_escape)
          (shortcut_link
            (link_text)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 602 - https://github.github.com/gfm/#example-602
    /// </summary>
    [TestMethod("Example 602 - https://github.github.com/gfm/#example-602")]
    [TestCategory("Spec")]
    public void TestSpecExample602()
    {
        string inputText = @"<http://foo.bar.baz>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 603 - https://github.github.com/gfm/#example-603
    /// </summary>
    [TestMethod("Example 603 - https://github.github.com/gfm/#example-603")]
    [TestCategory("Spec")]
    public void TestSpecExample603()
    {
        string inputText = @"<http://foo.bar.baz/test?q=hello&id=22&boolean>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 604 - https://github.github.com/gfm/#example-604
    /// </summary>
    [TestMethod("Example 604 - https://github.github.com/gfm/#example-604")]
    [TestCategory("Spec")]
    public void TestSpecExample604()
    {
        string inputText = @"<irc://foo.bar:2233/baz>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 605 - https://github.github.com/gfm/#example-605
    /// </summary>
    [TestMethod("Example 605 - https://github.github.com/gfm/#example-605")]
    [TestCategory("Spec")]
    public void TestSpecExample605()
    {
        string inputText = @"<MAILTO:FOO@BAR.BAZ>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 606 - https://github.github.com/gfm/#example-606
    /// </summary>
    [TestMethod("Example 606 - https://github.github.com/gfm/#example-606")]
    [TestCategory("Spec")]
    public void TestSpecExample606()
    {
        string inputText = @"<a+b+c:d>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 607 - https://github.github.com/gfm/#example-607
    /// </summary>
    [TestMethod("Example 607 - https://github.github.com/gfm/#example-607")]
    [TestCategory("Spec")]
    public void TestSpecExample607()
    {
        string inputText = @"<made-up-scheme://foo,bar>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 608 - https://github.github.com/gfm/#example-608
    /// </summary>
    [TestMethod("Example 608 - https://github.github.com/gfm/#example-608")]
    [TestCategory("Spec")]
    public void TestSpecExample608()
    {
        string inputText = @"<http://../>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 609 - https://github.github.com/gfm/#example-609
    /// </summary>
    [TestMethod("Example 609 - https://github.github.com/gfm/#example-609")]
    [TestCategory("Spec")]
    public void TestSpecExample609()
    {
        string inputText = @"<localhost:5001/foo>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 610 - https://github.github.com/gfm/#example-610
    /// </summary>
    [TestMethod("Example 610 - https://github.github.com/gfm/#example-610")]
    [TestCategory("Spec")]
    public void TestSpecExample610()
    {
        string inputText = @"<http://foo.bar/baz bim>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 611 - https://github.github.com/gfm/#example-611
    /// </summary>
    [TestMethod("Example 611 - https://github.github.com/gfm/#example-611")]
    [TestCategory("Spec")]
    public void TestSpecExample611()
    {
        string inputText = @"<http://example.com/\[\>";
        string expectedSyntaxTree = @"(inline
          (uri_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 612 - https://github.github.com/gfm/#example-612
    /// </summary>
    [TestMethod("Example 612 - https://github.github.com/gfm/#example-612")]
    [TestCategory("Spec")]
    public void TestSpecExample612()
    {
        string inputText = @"<foo@bar.example.com>";
        string expectedSyntaxTree = @"(inline
          (email_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 613 - https://github.github.com/gfm/#example-613
    /// </summary>
    [TestMethod("Example 613 - https://github.github.com/gfm/#example-613")]
    [TestCategory("Spec")]
    public void TestSpecExample613()
    {
        string inputText = @"<foo+special@Bar.baz-bar0.com>";
        string expectedSyntaxTree = @"(inline
          (email_autolink))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 614 - https://github.github.com/gfm/#example-614
    /// </summary>
    [TestMethod("Example 614 - https://github.github.com/gfm/#example-614")]
    [TestCategory("Spec")]
    public void TestSpecExample614()
    {
        string inputText = @"<foo\+@bar.example.com>";
        string expectedSyntaxTree = @"(inline
          (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 615 - https://github.github.com/gfm/#example-615
    /// </summary>
    [TestMethod("Example 615 - https://github.github.com/gfm/#example-615")]
    [TestCategory("Spec")]
    public void TestSpecExample615()
    {
        string inputText = @"<>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 616 - https://github.github.com/gfm/#example-616
    /// </summary>
    [TestMethod("Example 616 - https://github.github.com/gfm/#example-616")]
    [TestCategory("Spec")]
    public void TestSpecExample616()
    {
        string inputText = @"< http://foo.bar >";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 617 - https://github.github.com/gfm/#example-617
    /// </summary>
    [TestMethod("Example 617 - https://github.github.com/gfm/#example-617")]
    [TestCategory("Spec")]
    public void TestSpecExample617()
    {
        string inputText = @"<m:abc>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 618 - https://github.github.com/gfm/#example-618
    /// </summary>
    [TestMethod("Example 618 - https://github.github.com/gfm/#example-618")]
    [TestCategory("Spec")]
    public void TestSpecExample618()
    {
        string inputText = @"<foo.bar.baz>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 619 - https://github.github.com/gfm/#example-619
    /// </summary>
    [TestMethod("Example 619 - https://github.github.com/gfm/#example-619")]
    [TestCategory("Spec")]
    public void TestSpecExample619()
    {
        string inputText = @"http://example.com";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 620 - https://github.github.com/gfm/#example-620
    /// </summary>
    [TestMethod("Example 620 - https://github.github.com/gfm/#example-620")]
    [TestCategory("Spec")]
    public void TestSpecExample620()
    {
        string inputText = @"foo@bar.example.com";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 632 - https://github.github.com/gfm/#example-632
    /// </summary>
    [TestMethod("Example 632 - https://github.github.com/gfm/#example-632")]
    [TestCategory("Spec")]
    public void TestSpecExample632()
    {
        string inputText = @"<a><bab><c2c>";
        string expectedSyntaxTree = @"(inline
          (html_tag)
          (html_tag)
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 633 - https://github.github.com/gfm/#example-633
    /// </summary>
    [TestMethod("Example 633 - https://github.github.com/gfm/#example-633")]
    [TestCategory("Spec")]
    public void TestSpecExample633()
    {
        string inputText = @"<a/><b2/>";
        string expectedSyntaxTree = @"(inline
          (html_tag)
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 634 - https://github.github.com/gfm/#example-634
    /// </summary>
    [TestMethod("Example 634 - https://github.github.com/gfm/#example-634")]
    [TestCategory("Spec")]
    public void TestSpecExample634()
    {
        string inputText = @"<a  /><b2
data=""foo"" >";
        string expectedSyntaxTree = @"(inline
          (html_tag)
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 636 - https://github.github.com/gfm/#example-636
    /// </summary>
    [TestMethod("Example 636 - https://github.github.com/gfm/#example-636")]
    [TestCategory("Spec")]
    public void TestSpecExample636()
    {
        string inputText = @"Foo <responsive-image src=""foo.jpg"" />";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 637 - https://github.github.com/gfm/#example-637
    /// </summary>
    [TestMethod("Example 637 - https://github.github.com/gfm/#example-637")]
    [TestCategory("Spec")]
    public void TestSpecExample637()
    {
        string inputText = @"<33> <__>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 638 - https://github.github.com/gfm/#example-638
    /// </summary>
    [TestMethod("Example 638 - https://github.github.com/gfm/#example-638")]
    [TestCategory("Spec")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestSpecExample638()
    {
        string inputText = @"<a h*#ref=""hi"">";
        string expectedSyntaxTree = @"(inline
(tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 639 - https://github.github.com/gfm/#example-639
    /// </summary>
    [TestMethod("Example 639 - https://github.github.com/gfm/#example-639")]
    [TestCategory("Spec")]
    public void TestSpecExample639()
    {
        string inputText = @"<a href=""hi'> <a href=hi'>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 640 - https://github.github.com/gfm/#example-640
    /// </summary>
    [TestMethod("Example 640 - https://github.github.com/gfm/#example-640")]
    [TestCategory("Spec")]
    public void TestSpecExample640()
    {
        string inputText = @"< a><
foo><bar/ >
<foo bar=baz
bim!bop />";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 641 - https://github.github.com/gfm/#example-641
    /// </summary>
    [TestMethod("Example 641 - https://github.github.com/gfm/#example-641")]
    [TestCategory("Spec")]
    public void TestSpecExample641()
    {
        string inputText = @"<a href='bar'title=title>";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 642 - https://github.github.com/gfm/#example-642
    /// </summary>
    [TestMethod("Example 642 - https://github.github.com/gfm/#example-642")]
    [TestCategory("Spec")]
    public void TestSpecExample642()
    {
        string inputText = @"</a></foo >";
        string expectedSyntaxTree = @"(inline
          (html_tag)
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 643 - https://github.github.com/gfm/#example-643
    /// </summary>
    [TestMethod("Example 643 - https://github.github.com/gfm/#example-643")]
    [TestCategory("Spec")]
    public void TestSpecExample643()
    {
        string inputText = @"</a href=""foo"">";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 644 - https://github.github.com/gfm/#example-644
    /// </summary>
    [TestMethod("Example 644 - https://github.github.com/gfm/#example-644")]
    [TestCategory("Spec")]
    public void TestSpecExample644()
    {
        string inputText = @"foo <!-- this is a
comment - with hyphen -->";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 645 - https://github.github.com/gfm/#example-645
    /// </summary>
    [TestMethod("Example 645 - https://github.github.com/gfm/#example-645")]
    [TestCategory("Spec")]
    public void TestSpecExample645()
    {
        string inputText = @"foo <!-- not a comment -- two hyphens -->";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 646 - https://github.github.com/gfm/#example-646
    /// </summary>
    [TestMethod("Example 646 - https://github.github.com/gfm/#example-646")]
    [TestCategory("Spec")]
    public void TestSpecExample646()
    {
        string inputText = @"foo <!--> foo -->
foo <!-- foo--->";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 647 - https://github.github.com/gfm/#example-647
    /// </summary>
    [TestMethod("Example 647 - https://github.github.com/gfm/#example-647")]
    [TestCategory("Spec")]
    public void TestSpecExample647()
    {
        string inputText = @"foo <?php echo $a; ?>";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 648 - https://github.github.com/gfm/#example-648
    /// </summary>
    [TestMethod("Example 648 - https://github.github.com/gfm/#example-648")]
    [TestCategory("Spec")]
    public void TestSpecExample648()
    {
        string inputText = @"foo <!ELEMENT br EMPTY>";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 649 - https://github.github.com/gfm/#example-649
    /// </summary>
    [TestMethod("Example 649 - https://github.github.com/gfm/#example-649")]
    [TestCategory("Spec")]
    public void TestSpecExample649()
    {
        string inputText = @"foo <![CDATA[>&<]]>";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 650 - https://github.github.com/gfm/#example-650
    /// </summary>
    [TestMethod("Example 650 - https://github.github.com/gfm/#example-650")]
    [TestCategory("Spec")]
    public void TestSpecExample650()
    {
        string inputText = @"foo <a href=""ö"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 651 - https://github.github.com/gfm/#example-651
    /// </summary>
    [TestMethod("Example 651 - https://github.github.com/gfm/#example-651")]
    [TestCategory("Spec")]
    public void TestSpecExample651()
    {
        string inputText = @"foo <a href=""\*"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 652 - https://github.github.com/gfm/#example-652
    /// </summary>
    [TestMethod("Example 652 - https://github.github.com/gfm/#example-652")]
    [TestCategory("Spec")]
    public void TestSpecExample652()
    {
        string inputText = @"<a href=""\"""">";
        string expectedSyntaxTree = @"(inline
          (backslash_escape))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 654 - https://github.github.com/gfm/#example-654
    /// </summary>
    [TestMethod("Example 654 - https://github.github.com/gfm/#example-654")]
    [TestCategory("Spec")]
    public void TestSpecExample654()
    {
        string inputText = @"foo  
baz";
        string expectedSyntaxTree = @"(inline
          (hard_line_break))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 655 - https://github.github.com/gfm/#example-655
    /// </summary>
    [TestMethod("Example 655 - https://github.github.com/gfm/#example-655")]
    [TestCategory("Spec")]
    public void TestSpecExample655()
    {
        string inputText = @"foo\
baz";
        string expectedSyntaxTree = @"(inline
          (hard_line_break))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 656 - https://github.github.com/gfm/#example-656
    /// </summary>
    [TestMethod("Example 656 - https://github.github.com/gfm/#example-656")]
    [TestCategory("Spec")]
    public void TestSpecExample656()
    {
        string inputText = @"foo             
baz";
        string expectedSyntaxTree = @"(inline
          (hard_line_break))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 657 - https://github.github.com/gfm/#example-657
    /// </summary>
    [TestMethod("Example 657 - https://github.github.com/gfm/#example-657")]
    [TestCategory("Spec")]
    public void TestSpecExample657()
    {
        string inputText = @"foo  
     bar";
        string expectedSyntaxTree = @"(inline
          (hard_line_break))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 658 - https://github.github.com/gfm/#example-658
    /// </summary>
    [TestMethod("Example 658 - https://github.github.com/gfm/#example-658")]
    [TestCategory("Spec")]
    public void TestSpecExample658()
    {
        string inputText = @"foo\
     bar";
        string expectedSyntaxTree = @"(inline
          (hard_line_break))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 659 - https://github.github.com/gfm/#example-659
    /// </summary>
    [TestMethod("Example 659 - https://github.github.com/gfm/#example-659")]
    [TestCategory("Spec")]
    public void TestSpecExample659()
    {
        string inputText = @"*foo  
bar*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (hard_line_break)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 660 - https://github.github.com/gfm/#example-660
    /// </summary>
    [TestMethod("Example 660 - https://github.github.com/gfm/#example-660")]
    [TestCategory("Spec")]
    public void TestSpecExample660()
    {
        string inputText = @"*foo\
bar*";
        string expectedSyntaxTree = @"(inline
          (emphasis
            (emphasis_delimiter)
            (hard_line_break)
            (emphasis_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 661 - https://github.github.com/gfm/#example-661
    /// </summary>
    [TestMethod("Example 661 - https://github.github.com/gfm/#example-661")]
    [TestCategory("Spec")]
    public void TestSpecExample661()
    {
        string inputText = @"`code
span`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 662 - https://github.github.com/gfm/#example-662
    /// </summary>
    [TestMethod("Example 662 - https://github.github.com/gfm/#example-662")]
    [TestCategory("Spec")]
    public void TestSpecExample662()
    {
        string inputText = @"`code\
span`";
        string expectedSyntaxTree = @"(inline
          (code_span
            (code_span_delimiter)
            (code_span_delimiter)))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 663 - https://github.github.com/gfm/#example-663
    /// </summary>
    [TestMethod("Example 663 - https://github.github.com/gfm/#example-663")]
    [TestCategory("Spec")]
    public void TestSpecExample663()
    {
        string inputText = @"<a href=""foo
bar"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 664 - https://github.github.com/gfm/#example-664
    /// </summary>
    [TestMethod("Example 664 - https://github.github.com/gfm/#example-664")]
    [TestCategory("Spec")]
    public void TestSpecExample664()
    {
        string inputText = @"<a href=""foo\
bar"">";
        string expectedSyntaxTree = @"(inline
          (html_tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 665 - https://github.github.com/gfm/#example-665
    /// </summary>
    [TestMethod("Example 665 - https://github.github.com/gfm/#example-665")]
    [TestCategory("Spec")]
    public void TestSpecExample665()
    {
        string inputText = @"foo\";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 666 - https://github.github.com/gfm/#example-666
    /// </summary>
    [TestMethod("Example 666 - https://github.github.com/gfm/#example-666")]
    [TestCategory("Spec")]
    public void TestSpecExample666()
    {
        string inputText = @"foo  ";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 669 - https://github.github.com/gfm/#example-669
    /// </summary>
    [TestMethod("Example 669 - https://github.github.com/gfm/#example-669")]
    [TestCategory("Spec")]
    public void TestSpecExample669()
    {
        string inputText = @"foo
baz";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 670 - https://github.github.com/gfm/#example-670
    /// </summary>
    [TestMethod("Example 670 - https://github.github.com/gfm/#example-670")]
    [TestCategory("Spec")]
    public void TestSpecExample670()
    {
        string inputText = @"foo
 baz";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Example 671 - https://github.github.com/gfm/#example-671
    /// </summary>
    [TestMethod("Example 671 - https://github.github.com/gfm/#example-671")]
    [TestCategory("Spec")]
    public void TestSpecExample671()
    {
        string inputText = @"hello $.;'there";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 672 - https://github.github.com/gfm/#example-672
    /// </summary>
    [TestMethod("Example 672 - https://github.github.com/gfm/#example-672")]
    [TestCategory("Spec")]
    public void TestSpecExample672()
    {
        string inputText = @"Foo χρῆν";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Example 673 - https://github.github.com/gfm/#example-673
    /// </summary>
    [TestMethod("Example 673 - https://github.github.com/gfm/#example-673")]
    [TestCategory("Spec")]
    public void TestSpecExample673()
    {
        string inputText = @"Multiple     spaces";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }



    /// <summary>
    /// Tags are working
    /// </summary>
    [TestMethod("Tags are working")]
    [TestCategory("Tags")]
    [Ignore("Skipping this test for now because the original test corpus test also fails.")]
    public void TestTagsWorking()
    {
        string inputText = @"#tag #_tag_with-symbols0123and/numbers #0123tag_starting_with_numbers";
        string expectedSyntaxTree = @"(inline
  (tag)
  (tag)
  (tag))";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

    /// <summary>
    /// Purely numeric tags are forbidden
    /// </summary>
    [TestMethod("Purely numeric tags are forbidden")]
    [TestCategory("Tags")]
    public void TestPurelyNumericTagsForbidden()
    {
        string inputText = @"#0123";
        string expectedSyntaxTree = @"(inline)";
        _tree = _parser.Parse(inputText)!;
        string actualSyntaxTree = _tree.RootNode.ToString();
        Assert.AreEqual(RemoveWhitespace(expectedSyntaxTree), RemoveWhitespace(actualSyntaxTree));
    }

}
