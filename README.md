# AutomataViz

Калькулятор различных алгоритмов на конечных автоматах с возможностью визуализации.

## Общее описание

Проект посвящён разделу дискретной математики под названием "Теория автоматов". Основной объект изучения - конечные автоматы (КА) и регулярные языки.
КА бывают детерминированные (ДКА) и недетерминированные (НКА).
Кроме того, в теории автоматов существуют несколько (достаточно рутинных) алгоритмов, выполнение которых можно доверить машине, что является базовой идеей проекта. 

Сейчас с помощью приложения "AutomataViz" можно:
* визуализировать автомат (построить диаграмму переходов)
* построить минимальный (приведённый) ДКА
* узнать, распознаётся ли слово КА
* привести НКА к ДКА
* Построить ДКА по лямбда-НКА с помощью замыкания

## Технические особенности
Приложение разделено на слои согласно принципу слоистой архитектуры (Domain Driven Design). В приложении 4 слоя:
* *Infrastructure* - здесь лежат классы, которые могут быть использованы в контексте другой предметной области, например методы-расширения IEnumerable.
* *Domain* - сборка, содержащая классы предметной области - автоматы, и сервисы - алгоритмы.
* *Application* - здесь находится парсер, создающий автомат по текстовому вводу пользователя
* *AutomataUI / Api* - внешний слой приложения, содержащий GUI. Может быть легко заменён на WebAPI для веб-приложения.

Создать автомат можно с помощью класса AutomataBuilder. Этот класс реализован в стиле FluentApi:
```c#
var automata = AutomataBuilder.CreateAutomata()
            .SetStartState("0")
            .SetTerminateStates("4", "5", "6")
            .AddTransition("0", "a", "5")
            // more transitions
            .Build();
```

Все автоматы являются классами-наследниками абстрактного класса Automata.

У каждого класса-наслежника Automata определён конструктор с пятью параметрами:

```c#
 public DFA(HashSet<string> states,
        HashSet<string> alphabet,
        HashSet<Transition> transitions,
        string startState,
        HashSet<string> terminateStates)
```

Выполнить алгоритм можно путём создания экземпляра и вызова метода Get():
```c#
var algorithm = new MinimizationAlgorithm();
var minimizedDfa = algorithm.Get(dfa);
```

При разработке приложения использовался принцип инверсии зависимостей (DIP - Dependency Inversion Principle), и как следствие, приложение использует DI-контейнер Autofac.

Composition root (место сборки компонентов) находится в классе App.xaml.cs:
```c#
private IContainer ConfigureContainer()
{
  var builder = new ContainerBuilder();
  builder.RegisterType<AutomataParser>().As<IAutomataParser>();
  // more dependencies
  builder.RegisterType<MainWindow>().AsSelf();
  return builder.Build();
}
```
