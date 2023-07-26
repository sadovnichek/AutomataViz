﻿using Domain.Algorithm;
using Domain.Automatas;
using Infrastructure;

namespace Application.StudentTask;

public class MinimizationTask : IAutomataTask
{
    private string description;
    private readonly MinimizationAlgorithm algorithm;
    private HashSet<string> states;
    private HashSet<string> alphabet;
    public string Name { get; }
    
    public MinimizationTask()
    {
        algorithm = AlgorithmResolver.GetService<MinimizationAlgorithm>();
        Name = algorithm.Name;
    }

    public IAutomataTask Configure(string description, int states, HashSet<string> alphabet)
    {
        this.description = description;
        this.states = Enumerable.Range(1, states).Select(x => x.ToString()).ToHashSet();
        this.alphabet = alphabet;
        return this;
    }

    public void Create(TexFile student, TexFile teacher)
    {
        TexFile.WriteMany(description, student, teacher);
        
        var randomAutomata = DFA.GetRandom(states, alphabet);
        var transformed = algorithm.Get(randomAutomata);

        while (!IsAppropriate(randomAutomata, transformed))
        {
            randomAutomata = DFA.GetRandom(states, alphabet);
            transformed = algorithm.Get(randomAutomata);
        }
        TexFile.WriteMany(randomAutomata.ConvertToTexFormat(), student, teacher);
        
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
    }

    private static bool IsAppropriate(Automata source, Automata result)
    {
        return result.States.CountCompoundSets() > 1
               && source.GetUnreachableStates().Count <= 1
               && result.States.Count != result.States.CountCompoundSets();
    }
}