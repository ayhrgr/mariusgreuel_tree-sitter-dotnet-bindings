using TreeSitter;

{
    using var language = new Language("JavaScript");
    using var parser = new Parser(language);
    using var tree = parser.Parse("console.log('Hello World');")!;
    Console.WriteLine($"Root node: {tree.RootNode}");
}

{
    using var language = new Language("JavaScript");
    using var parser = new Parser(language);
    using var tree = parser.Parse("function one() { function two() {} }")!;
    using var query = new Query(language, "(function_declaration name: (identifier) @fn)");
    foreach (var capture in query.Execute(tree.RootNode).Captures)
    {
        Console.WriteLine($"Found function: {capture.Node.Text}");
    }
}

{
    using var language = new Language("tree-sitter-c-sharp", "tree_sitter_c_sharp");
    using var parser = new Parser(language);
    using var tree = parser.Parse("class Hello { static void Main() { System.Console.WriteLine(\"Hello, C#\"); } }")!;
    Console.WriteLine($"Root node: {tree.RootNode}");
    using var query = new Query(language, "(method_declaration name: (identifier) @fn)");
    foreach (var capture in query.Execute(tree.RootNode).Captures)
    {
        Console.WriteLine($"Found function: {capture.Node.Text}");
    }
}

{
    using var language = new Language("tree-sitter-c-sharp", "tree_sitter_c_sharp");
    using var parser = new Parser(language);
    using var tree = parser.Parse("class Hello { static void Main() { System.Console.WriteLine(\"Hello, C#\"); } }")!;
    Console.WriteLine($"Root node: {tree.RootNode}");
    using var query = new Query(language, "(method_declaration name: (identifier) @name body: (block) @body)");
    foreach (var capture in query.Execute(tree.RootNode).Captures)
    {
        Console.WriteLine($"Found function: {capture.Node.Text}");
    }
}

{
    // add tree-sitter-markdown
    using var language = new Language("markdown");
    using var parser = new Parser(language);
    using var tree = parser.Parse("# Title\n\n- Item 1\n- Item 2\n\nParagraph text.")!;
    Console.WriteLine($"Root node: {tree.RootNode}");   // Root node: (document (section (atx_heading (atx_h1_marker) heading_content: (inline)) (list (list_item (list_marker_minus) (paragraph (inline))) (list_item (list_marker_minus) (paragraph (inline) (block_continuation)))) (paragraph (inline))))
    using var query = new Query(language, @"
        (atx_heading (atx_h1_marker) @heading)
        (list_item (list_marker_minus) @listitem)
        (paragraph (inline) @paragraph)
        ");
    foreach (var capture in query.Execute(tree.RootNode).Captures)
    {
        Console.WriteLine($"{capture.Name}: {capture.Node.Text}");
    }
}

{
    // add tree-sitter-markdown-inline
    using var language = new Language("tree-sitter-markdown-inline", "tree_sitter_markdown_inline");
    using var parser = new Parser(language);
    using var tree = parser.Parse("This is **bold** and *italic* and [link](https://example.com).")!;
    Console.WriteLine($"Root node: {tree.RootNode}");   // Root node: (inline (strong_emphasis (emphasis_delimiter) (emphasis_delimiter) (emphasis_delimiter) (emphasis_delimiter)) (emphasis (emphasis_delimiter) (emphasis_delimiter)) (inline_link (link_text) (link_destination)))
    using var query = new Query(language, @"
        (strong_emphasis) @bold
        (emphasis) @italic
        (inline_link) @link
        ");
    foreach (var capture in query.Execute(tree.RootNode).Captures)
    {
        Console.WriteLine($"{capture.Name}: {capture.Node.Text}");
    }
}
