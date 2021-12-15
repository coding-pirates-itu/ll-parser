using System;
using System.Collections.Generic;
using System.Linq;


var Replacements = new Dictionary<Rules, TokenEntry[]>
{
    [Rules.SisF] = new[] { new TokenEntry(States.F) },
    [Rules.SisSplusF] = new[] { new TokenEntry(Tokens.Open),
                                new TokenEntry(States.S),
                                new TokenEntry(Tokens.Plus),
                                new TokenEntry(States.F),
                                new TokenEntry(Tokens.Close) },
    [Rules.FisA] = new[] { new TokenEntry(Tokens.A) }
};

var Transitions = new Dictionary<States, Dictionary<Tokens, Rules>>
{
    [States.S] = new Dictionary<Tokens, Rules> {
        [Tokens.Open] = Rules.SisSplusF,
        [Tokens.A] = Rules.SisF },
    [States.F] = new Dictionary<Tokens, Rules> {
        [Tokens.A] = Rules.FisA }
};

Console.Write("Input >");
var input = Console.ReadLine();
var stack = new Stack<TokenEntry>();
stack.Push(new TokenEntry(Tokens.End));
stack.Push(new TokenEntry(States.S));

foreach (var ch in input.Append('$'))
{
    var inputToken = ch switch
    {
        '(' => Tokens.Open,
        ')' => Tokens.Close,
        '+' => Tokens.Plus,
        'a' => Tokens.A,
        '$' => Tokens.End,
        _ => throw new ArgumentException($"Unknown input '{ch}'.")
    };

    var topToken = stack.Pop();
    if (topToken.Type == TokenTypes.Terminal)
    {
        if (topToken.Terminal != inputToken)
            throw new ArgumentException($"Unexpected input '{ch}', expected '{topToken.Terminal}'.");
    }
    else
    {
        do
        {
            if (!Transitions[topToken.NonTerminal].TryGetValue(inputToken, out var rule))
                throw new ArgumentException($"No rule for '{ch}' in state '{topToken.NonTerminal}'.");
            Console.WriteLine($"Using rule {rule}.");

            foreach (var repl in Replacements[rule].Reverse())
            {
                stack.Push(repl);
            }
            topToken = stack.Pop();
        }
        while (topToken.Type == TokenTypes.NonTerminal);

        if (topToken.Terminal != inputToken)
            throw new ArgumentException($"Unexpected input '{ch}', expected '{topToken.Terminal}'.");
    }
}

if (stack.Any())
    throw new ArgumentException("Incomplete input.");
Console.WriteLine("Done.");


public enum States
{
    S,
    F
}


public enum Tokens
{
    End,
    Open,
    Close,
    Plus,
    A
}


public enum Rules
{
    None,
    SisF,
    SisSplusF,
    FisA
}


public enum TokenTypes
{
    Terminal,
    NonTerminal
}


public class TokenEntry
{
    public TokenTypes Type { get; }

    public Tokens Terminal { get; }

    public States NonTerminal { get; }


    public TokenEntry(Tokens terminal)
    {
        Type = TokenTypes.Terminal;
        Terminal = terminal;
    }

    public TokenEntry(States nonTerminal)
    {
        Type = TokenTypes.NonTerminal;
        NonTerminal = nonTerminal;
    }

    public override string ToString() => Type == TokenTypes.Terminal ? Terminal.ToString() : NonTerminal.ToString();
}

