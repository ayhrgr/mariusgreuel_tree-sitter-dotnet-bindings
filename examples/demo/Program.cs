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
