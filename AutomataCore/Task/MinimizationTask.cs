﻿using AutomataCore.Algorithm;
using AutomataCore.Automata;
using Infrastructure;

namespace AutomataCore.Task;

public class MinimizationTask : ITask
{
    private string description;
    private MinimizationAlgorithm algorithm;
    private HashSet<string> states;
    private HashSet<string> alphabet;
    public string Name { get; }
    
    public MinimizationTask()
    {
        algorithm = MinimizationAlgorithm.GetInstance();
        Name = algorithm.Name;
    }

    public ITask Configure(string description, int states, HashSet<string> alphabet)
    {
        this.description = description;
        this.states = Enumerable.Range(1, states).Select(x => x.ToString()).ToHashSet();
        this.alphabet = alphabet;
        return this;
    }

    public void Create(TexFile student, TexFile teacher)
    {
        WriteBoth(student, teacher, description);
        
        var randomAutomata = DFA.GetRandom(states, alphabet);
        var transformed = algorithm.Get(randomAutomata);

        while (!IsAppropriate(randomAutomata, transformed))
        {
            randomAutomata = DFA.GetRandom(states, alphabet);
            transformed = algorithm.Get(randomAutomata);
        }
        WriteBoth(student, teacher, randomAutomata.ConvertToTexFormat());
        
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
    }
    
    private static void WriteBoth(TexFile student, TexFile teacher, string text)
    {
        student.Write(text); 
        teacher.Write(text);
    }

    private static bool IsAppropriate(Automata.Automata source, Automata.Automata result)
    {
        return result.CountCompoundSets() > source.States.Count / 4
               && result.TerminateStates.Count > 1;
    }
}