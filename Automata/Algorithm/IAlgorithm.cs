﻿namespace Automata.Algorithm;

public interface IAlgorithm
{
    string Name { get; }
    
    bool IsTaskable { get; }
}